using AutoMapper;
using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Http;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Commands.ImportCards
{
    public class ImportCardsCommand : IRequest<Tuple<int, string[]>>
    {
        [JsonIgnore]
        public IFormFile File { get; set; }
    }

    public class ImportCardsCommandHandler : IRequestHandler<ImportCardsCommand, Tuple<int, string[]>>
    {
        private readonly IApplicationDbContext _context;

        public ImportCardsCommandHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
        {
            _context = context;
        }

        public async Task<Tuple<int, string[]>> Handle(ImportCardsCommand request, CancellationToken cancellationToken)
        {
            var defaultFailResult = Tuple.Create(-1, Array.Empty<string>());
            using (var reader = new StreamReader(request.File.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<ImportCardsRecord>().ToList();
                var invalidCardsList = new List<ImportCardsRecord>();
                var validCardsList = new List<ImportCardsRecord>();
                const int maxItemImport = 40000;
                if(records.Count > maxItemImport) return await Task.FromResult(defaultFailResult);
                foreach (var item in records)
                {
                    var cardEntity = ValidateCardInput(item);
                    if (cardEntity == null) invalidCardsList.Add(item);
                    else validCardsList.Add(item);
                }
                if (invalidCardsList.Any() || !validCardsList.Any()) return await Task.FromResult(defaultFailResult);

                var listCustomersWithoutDuplicate = validCardsList.GroupBy(n => n.MemberNo, (key, value) => new 
                {
                    Key = key,
                    Count = value.Count()
                }).Where(n => n.Count > 1);
                if(listCustomersWithoutDuplicate.Any()) return await Task.FromResult(defaultFailResult);

                var listMemberNos = validCardsList.GroupBy(n => n.MemberNo, (key, value) => key);
                var listCompanyCodes = validCardsList.GroupBy(n => n.CompanyCode, (key, value) => key);
                var listStoreCodes = validCardsList.GroupBy(n => n.StoreCode, (key, value) => key);

                var listMemberNosDb = _context.Cards.Where(n => !n.IsDeleted);
                var listCompaniesDb = _context.Companies.Where(n => !n.IsDeleted);
                var listStoresDb = _context.Stores.Where(n => !n.IsDeleted);

                var allCustomerNoNotExist = !listMemberNos.Intersect(listMemberNosDb.Select(n => n.MemberNo)).Any();
                if (!allCustomerNoNotExist) return await Task.FromResult(defaultFailResult);

                var isAllCompanyCodeExist = listCompanyCodes.Intersect(listCompaniesDb.Select(n => n.CompanyCode)).Count() == listCompanyCodes.Count();
                if (!isAllCompanyCodeExist) return await Task.FromResult(defaultFailResult);

                var isAllStoreCodeExist = listStoreCodes.Intersect(listStoresDb.Select(n => n.StoreCode)).Count() == listStoreCodes.Count();
                if(!isAllStoreCodeExist) return await Task.FromResult(defaultFailResult);

                var listItems = AddCompanyAndStoreCodeToEntity(validCardsList, listCompaniesDb.ToList(), listStoresDb.ToList());
                if(!listItems.Any() || listItems.Count != validCardsList.Count) return await Task.FromResult(defaultFailResult);

                await _context.Cards.AddRangeAsync(listItems);
                var result = await _context.SaveChangesAsync(cancellationToken);
                if (listItems.Count != result) 
                {
                    _context.Cards.RemoveRange(listItems);
                    await _context.SaveChangesAsync(cancellationToken);
                    return await Task.FromResult(defaultFailResult);
                }

                return Tuple.Create(result, listItems.Select(x=>x.MemberNo).ToArray());
            }
        }

        private List<Card> AddCompanyAndStoreCodeToEntity(List<ImportCardsRecord> source, IEnumerable<Company> companies, IEnumerable<Store> stores)
        {
            var result = new List<Card>();
            foreach (var item in source)
            {
                var expiratedAt = DateTime.Now;
                var companyItem = companies.FirstOrDefault(n => n.CompanyCode.Equals(item.CompanyCode));
                if (companyItem == null) continue;
                var storeItem = stores.FirstOrDefault(n => n.StoreCode.Equals(item.StoreCode));
                if (storeItem == null) continue;
                int.TryParse(item.No, out int no);
                DateTime.TryParseExact(item.ExpirationDate, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiratedAt);
                result.Add(new Card
                {
                    No = no,
                    MemberNo = item.MemberNo,
                    ExpiredAt = expiratedAt,
                    Status = CardStatus.Unissued,
                    CompanyId = companyItem.Id,
                    StoreId = storeItem.Id
                });
            }
            return result;
        }

        private Card ValidateCardInput(ImportCardsRecord card)
        {
            const string memberNoStartWith = "2";
            const int validMemberLength = 10;
            const int validExpirionDateLength = 6;
            const int validCompanyCodeLength = 4;
            const int validStpreCodeLength = 4;

            // Validate fix length of CustomerNo, ExpirateDate, CompanyCOde and StoreCode
            var expiratedAt = DateTime.Now;
            if (card == null) return null;
            var validNo = int.TryParse(card.No, out int no);
            var validCustomerNo = card.MemberNo.Length == validMemberLength && long.TryParse(card.MemberNo, out long customerNo) && card.MemberNo.StartsWith(memberNoStartWith);
            var validDate = card.ExpirationDate.Length == validExpirionDateLength && DateTime.TryParseExact(card.ExpirationDate, "yyyyMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiratedAt);
            var validCompanyCode = card.CompanyCode.Length == validCompanyCodeLength && int.TryParse(card.CompanyCode, out int companyCode);
            var validStoreCode = card.StoreCode.Length == validStpreCodeLength && int.TryParse(card.StoreCode, out int storeCode);
            if (!validNo || !validCustomerNo || !validDate || !validCompanyCode || !validStoreCode) return null;
            DateTime minDate = new DateTime(1990, 1, 1);
            DateTime maxDate = new DateTime(2101, 1, 1);
            if (expiratedAt < minDate || expiratedAt >= maxDate) return null;
            return new Card()
            {
                No = no,
                ExpiredAt = expiratedAt
            };

        }
    }
}

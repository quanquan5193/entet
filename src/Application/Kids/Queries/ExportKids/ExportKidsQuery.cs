using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.ExpressionExtension;
using mrs.Application.Common.Helpers.AzureKeyVaults;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Kids.Queries.ExportKids
{
    public class ExportKidsQuery : IRequest<ExportKidsVm>
    {
        public string MemberNo { get; set; }
        public string KidName { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCode { get; set; }
        public string DeviceNumber { get; set; }
        public DateTime? RegisterDateFrom { get; set; }
        public DateTime? RegisterDateTo { get; set; }
        public string PICStoreName { get; set; }
        public string SortBy { get; set; }
        public string SortDimension { get; set; }
    }

    public class KidCsvRecordProfile : Profile
    {
        private const string JAPANESE_SPACE = "　";
        private const string LIVE_WITH_PARENT_TEXT = "同居";
        private const string UN_LIVE_WITH_PARENT_TEXT = "別居";

        public KidCsvRecordProfile()
        {
            CreateMap<MemberKid, KidCsvRecord>()
                .ForMember(x => x.RegisterDate, y => y.MapFrom(z => z.CreatedAt))
                .ForMember(x => x.DateOfBirth, y => y.MapFrom(z => z.DateOfBirth))
                .ForMember(x => x.Name, y => y.MapFrom(z => z.LastName + JAPANESE_SPACE + z.FirstName))
                .ForMember(x => x.KanaName, y => y.MapFrom(z => z.FuriganaLastName + JAPANESE_SPACE + z.FuriganaFirstName))
                .ForMember(x => x.SexType, y => y.MapFrom(z => ((SexType)z.Sex).GetStringValue()))
                .ForMember(x => x.Email, y => y.MapFrom(z => z.Member.Email))
                .ForMember(x => x.Relationship, y => y.MapFrom(z => ((KidRelationshipEnum)z.RelationshipMember).GetStringValue()))
                .ForMember(x => x.IsLivingWithParent, y => y.MapFrom(z => z.IsLivingWithParent ? LIVE_WITH_PARENT_TEXT : UN_LIVE_WITH_PARENT_TEXT))
                .ForMember(x => x.ParentName, y => y.MapFrom(z => z.ParentLastName + JAPANESE_SPACE + z.ParentFirstName))
                .ForMember(x => x.KanaParentName, y => y.MapFrom(z => z.ParentFuriganaLastName + JAPANESE_SPACE + z.ParentFuriganaFirstName))
                .ForMember(x => x.CustomerNo, y => y.MapFrom(z => z.Member.MemberNo))
                .ForMember(x => x.Remark, y => y.MapFrom(z => z.Member.Remark))
                .ForMember(x => x.CompanyCode, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyCode))
                .ForMember(x => x.StoreCode, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.StoreCode))
                .ForMember(x => x.PICStoreName, y => y.MapFrom(z => z.Member.PICStore.PICName))
                .ForMember(x => x.DeviceCode, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault(w => w.ReceiptedTypeId == (int)RequestTypeEnum.Kid || w.ReceiptedTypeId == (int)RequestTypeEnum.New).Device.DeviceCode))
                .ForMember(x => x.RequestType, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault(w => w.ReceiptedTypeId == (int)RequestTypeEnum.Kid || w.ReceiptedTypeId == (int)RequestTypeEnum.New).RequestType.RequestTypeName));
        }
    }


    public class ExportKidsQueryHandler : IRequestHandler<ExportKidsQuery, ExportKidsVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICsvFileBuilder _fileBuilder;
        private readonly IMapper _mapper;
        private const string JAPANESE_SPACE = "　";
        private readonly IConfiguration _configuration;

        public ExportKidsQueryHandler(IApplicationDbContext context, IMapper mapper, IConfiguration configuration, ICsvFileBuilder fileBuilder)
        {
            _fileBuilder = fileBuilder;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ExportKidsVm> Handle(ExportKidsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<MemberKid> query = _context.MemberKids.AsQueryable();

            query = query.Where(x => x.Member.RequestsReceipteds.Any(y => y.ReceiptedTypeId == (int)RequestTypeEnum.Kid || y.ReceiptedTypeId == (int)RequestTypeEnum.New));

            if (!string.IsNullOrWhiteSpace(request.MemberNo))
                query = query.Where(x => x.Member.MemberNo.Contains(request.MemberNo));

            if (!string.IsNullOrWhiteSpace(request.KidName))
                query = query.Where(x =>
                (x.LastName + JAPANESE_SPACE + x.FirstName).Contains(request.KidName)
                || (x.FuriganaLastName + JAPANESE_SPACE + x.FuriganaFirstName).Contains(request.KidName)
                || (x.LastName + " " + x.FuriganaFirstName).Contains(request.KidName)
                || (x.FuriganaLastName + " " + x.FuriganaFirstName).Contains(request.KidName));

            if (!string.IsNullOrWhiteSpace(request.CompanyCode))
                query = query.Where(x => x.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyCode.Contains(request.CompanyCode));

            if (!string.IsNullOrWhiteSpace(request.StoreCode))
                query = query.Where(x => x.Member.RequestsReceipteds.FirstOrDefault().Store.StoreCode.Contains(request.StoreCode));

            if (!string.IsNullOrWhiteSpace(request.DeviceNumber))
                query = query.Where(x => x.Member.RequestsReceipteds.Any(x => x.Device.DeviceCode.Contains(request.DeviceNumber)));

            if (!string.IsNullOrWhiteSpace(request.PICStoreName))
                query = query.Where(x => x.Member.PICStore.PICName.Contains(request.PICStoreName));

            if (request.RegisterDateFrom.HasValue)
            {
                if (request.RegisterDateTo.HasValue && request.RegisterDateTo.Value.CompareTo(request.RegisterDateFrom.Value) > 0)
                {
                    query = query.Where(x => x.Member.RequestsReceipteds.Any(y => y.CreatedAt >= request.RegisterDateFrom && y.CreatedAt <= request.RegisterDateTo));
                }

                if (request.RegisterDateTo.HasValue && request.RegisterDateTo.Value.CompareTo(request.RegisterDateFrom.Value) == 0)
                {
                    query = query.Where(x => x.Member.RequestsReceipteds.Any(y => y.CreatedAt.Year == request.RegisterDateFrom.Value.Year && y.CreatedAt.Month == request.RegisterDateFrom.Value.Month && y.CreatedAt.Day == request.RegisterDateFrom.Value.Day));
                }
            }

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = query.OrderByCustom(request.SortBy + " " + request.SortDimension.ToUpper());
            }
            else
            {
                query = query.OrderByDescending(x => x.Id);
            }

            int maxDataCount = 0;
            if (int.TryParse(_configuration["ApplicationSetting:MaxRecordExport"], out maxDataCount))
                if (query.Count() > maxDataCount) throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("MoreThanRecord", maxDataCount.ToString()) });

            List<KidCsvRecord> records = await query.ProjectTo<KidCsvRecord>(_mapper.ConfigurationProvider).Take(maxDataCount).ToListAsync(cancellationToken);
            int indexNo = 1;
            foreach (var item in records)
            {
                item.No = indexNo.ToString();
                item.Email = await item.Email.ToDecryptStringAsync();
                indexNo++;
            }

            ExportKidsVm vm = new ExportKidsVm();
            vm.Content = _fileBuilder.BuildKidsFile(records);
            vm.ContentType = "text/csv";
            vm.FileName = $"{DateTime.Now.ToString("yyyymmdd_HHMMss")}.csv";

            return vm;
        }
    }
}

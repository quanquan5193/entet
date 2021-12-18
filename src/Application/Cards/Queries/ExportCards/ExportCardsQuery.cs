using AutoMapper;
using AutoMapper.QueryableExtensions;
using mrs.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mrs.Domain.Entities;
using System;
using System.Globalization;
using mrs.Domain.Enums;
using System.Collections.Generic;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.Cards.Queries.GetCards;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Exceptions;
using FluentValidation.Results;

namespace mrs.Application.Cards.Queries.ExportCards
{
    public class ExportCardsQuery : IRequest<ExportCardsVm>
    {
        public string MemberNo { get; set; }
        public string DeviceCode { get; set; }
        public string ExpiredAt { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCode { get; set; }
        public string Status { get; set; }
        public string AcceptFrom { get; set; }
        public string AcceptTo { get; set; }
        public string AcceptBy { get; set; }
    }

    public class ExportCardsQueryHandler : IRequestHandler<ExportCardsQuery, ExportCardsVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICsvFileBuilder _fileBuilder;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;

        public ExportCardsQueryHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder, IConfiguration configuration, IIdentityService identityService)
        {
            _context = context;
            _mapper = mapper;
            _fileBuilder = fileBuilder;
            _identityService = identityService;
            _configuration = configuration;
        }

        public class ExportCardsDtoProfile : Profile
        {
            public ExportCardsDtoProfile()
            {
                CreateMap<CardDto, CardRecord>()
                    .ForMember(a => a.CompanyCode, b => b.MapFrom(c => c.Company.CompanyCode))
                    .ForMember(a => a.StoreCode, b => b.MapFrom(c => c.Store.StoreCode));
            }
        }

        public async Task<ExportCardsVm> Handle(ExportCardsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<CardDto> query = _identityService.GetCardsWithUserQueryable();

            if (!string.IsNullOrEmpty(request.MemberNo)) query = query.Where(n => n.MemberNo.Contains(request.MemberNo));
            
            if (!string.IsNullOrEmpty(request.ExpiredAt))
            {
                DateTime expiratedAt = DateTime.Now;
                bool validDate = DateTime.TryParseExact(request.ExpiredAt, "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiratedAt);
                if (validDate) query = query.Where(n => n.ExpiredAt == expiratedAt);
            }
            if (!string.IsNullOrEmpty(request.CompanyCode)) query = query.Where(n => n.Company.CompanyCode.Contains(request.CompanyCode));
            if (!string.IsNullOrEmpty(request.StoreCode)) query = query.Where(n => n.Store.StoreCode.Contains(request.StoreCode));
            if (!string.IsNullOrEmpty(request.Status))
            {
                bool validStatus = Enum.TryParse(request.Status, out CardStatus status);
                if (validStatus) query = query.Where(n => n.Status == status);
            }
            if (!string.IsNullOrEmpty(request.AcceptFrom) && !string.IsNullOrEmpty(request.AcceptTo))
            {
                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                bool validDate = DateTime.TryParseExact(request.AcceptFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate)
                    && DateTime.TryParseExact(request.AcceptTo, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                if (validDate) query = query.Where(n => n.CreatedAt >= fromDate && n.CreatedAt < toDate.AddDays(1));
            }
            if (!string.IsNullOrEmpty(request.AcceptBy))
            {
                query = query.Where(n => n.CreatedByName.Contains(request.AcceptBy));
            }
            ExportCardsVm vm = new ExportCardsVm();
            int maxDataCount = 0;
            if (int.TryParse(_configuration["ApplicationSetting:MaxRecordExport"], out maxDataCount))
                if (query.Count() > maxDataCount) throw new ValidationException(new List<ValidationFailure>() { new ValidationFailure("MoreThanRecord", maxDataCount.ToString()) });
            query = query.OrderBy(n => n.CreatedAt).Take(maxDataCount);
            List<CardRecord> records = await query.ProjectTo<CardRecord>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            int indexNo = 1;
            foreach (var item in records)
            {
                item.No = indexNo.ToString();
                indexNo++;
            }

            vm.Content = _fileBuilder.BuilCardsFile(records);
            vm.ContentType = "text/csv";
            vm.FileName = $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv";

            return await Task.FromResult(vm);
        }
    }
}

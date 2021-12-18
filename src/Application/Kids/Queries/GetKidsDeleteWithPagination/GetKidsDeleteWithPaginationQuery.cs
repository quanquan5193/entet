using AutoMapper;
using AutoMapper.QueryableExtensions;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Common.Security;
using mrs.Domain.Entities;
using System;
using System.Globalization;
using mrs.Domain.Enums;
using mrs.Application.Common.ExpressionExtension;
using Microsoft.EntityFrameworkCore;
using mrs.Application.ApplicationUser.Queries.GetToken;
using System.Collections.Generic;
using mrs.Application.Kids.Queries.GetKidsWithPagination;
using mrs.Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;

namespace mrs.Application.Cards.Queries.GetKidsWithPagination
{
    public class GetKidsDeleteWithPaginationQuery : IRequest<PaginatedList<MemberKidDeleteDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 70000;
        public string SortType { get; set; } = "asc";
        public string SortBy { get; set; } = "Id";
        public string CompanyCode { get; set; } = "";
        public string StoreCode { get; set; } = "";
        public string DeviceCode { get; set; } = "";
        public string RegisteredFrom { get; set; } = "";
        public string RegisteredTo { get; set; } = "";
    }

    public class GetKidsDeleteWithPaginationQueryHandler : IRequestHandler<GetKidsDeleteWithPaginationQuery, PaginatedList<MemberKidDeleteDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetKidsDeleteWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public class CardForKidDtoProfile : Profile
        {
            private const string JAPANESE_SPACE = "　";
            public CardForKidDtoProfile()
            {
                CreateMap<MemberKid, MemberKidDeleteDto>()
                    .ForMember(x => x.ParentName, y => y.MapFrom(z => z.ParentLastName + JAPANESE_SPACE + z.ParentFirstName))
                    .ForMember(x => x.MemberNo, y => y.MapFrom(z => z.Member.MemberNo))
                    .ForMember(x => x.KidName, y => y.MapFrom(z => z.LastName + JAPANESE_SPACE + z.FirstName))
                    .ForMember(x => x.Store, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store))
                    .ForMember(x => x.Company, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.Company));

            }
        }

        public async Task<PaginatedList<MemberKidDeleteDto>> Handle(GetKidsDeleteWithPaginationQuery request, CancellationToken cancellationToken)
        {
            IQueryable<MemberKid> query = _context.MemberKids.Include(n => n.Member).ThenInclude(x => x.RequestsReceipteds).ThenInclude(n => n.Store).ThenInclude(n => n.Company).Where(n => !n.IsDeleted);
            
            if (!string.IsNullOrWhiteSpace(request.CompanyCode))
                query = query.Where(x => x.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyCode.Contains(request.CompanyCode));

            if (!string.IsNullOrWhiteSpace(request.StoreCode))
                query = query.Where(x => x.Member.RequestsReceipteds.FirstOrDefault().Store.StoreCode.Contains(request.StoreCode));

            if (!string.IsNullOrWhiteSpace(request.DeviceCode))
                query = query.Where(x => x.Member.RequestsReceipteds.Any(x => x.Device.DeviceCode.Contains(request.DeviceCode)));
            if (!string.IsNullOrEmpty(request.RegisteredFrom))
            {
                DateTime fromDate = DateTime.Now;
                bool validDate = DateTime.TryParseExact(request.RegisteredFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                if (validDate) query = query.Where(n => n.CreatedAt >= fromDate);
            }
            if (!string.IsNullOrEmpty(request.RegisteredTo))
            {
                DateTime toDate = DateTime.Now;
                bool validDate = DateTime.TryParseExact(request.RegisteredTo, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                if (validDate) query = query.Where(n => n.CreatedAt < toDate.AddDays(1));
            }
            if (!string.IsNullOrEmpty(request.SortBy)) {
                if(request.SortBy.Contains("storeCode")) query = request.SortType.Equals("asc")
                         ? query.OrderBy(n => n.Member.RequestsReceipteds.FirstOrDefault().Store.StoreCode)
                         : query.OrderByDescending(n => n.Member.RequestsReceipteds.FirstOrDefault().Store.StoreCode);
                else if(request.SortBy.Contains("companyCode")) query = request.SortType.Equals("asc")
                         ? query.OrderBy(n => n.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyCode)
                         : query.OrderByDescending(n => n.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyCode);
                else if (request.SortBy.Contains("KidName")) query = request.SortType.Equals("asc")
                          ? query.OrderBy(n => n.LastName).ThenBy(n => n.FirstName)
                          : query.OrderByDescending(n => n.LastName).ThenByDescending(n => n.FirstName);
                else if (request.SortBy.Contains("ParentName")) query = request.SortType.Equals("asc")
                          ? query.OrderBy(n => n.ParentLastName).ThenBy(n => n.ParentFirstName)
                          : query.OrderByDescending(n => n.ParentLastName).ThenByDescending(n => n.ParentFirstName);
                else if (request.SortBy.Contains("MemberNo")) query = request.SortType.Equals("asc")
                          ? query.OrderBy(n => n.Member.MemberNo)
                          : query.OrderByDescending(n => n.Member.MemberNo);
                else query = query.OrderByCustom(request.SortBy + " " + request.SortType.ToUpper());
            }
            else
            {
                query = query.OrderByDescending(n => n.Id);
            }

            int pageSize = query.Count();

            var records = await query.ProjectTo<MemberKidDeleteDto>(_mapper.ConfigurationProvider).PaginatedListAsync(request.PageNumber, pageSize > 0 ? pageSize : 1);

            return records;
        }
    }
}

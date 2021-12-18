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
using mrs.Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;

namespace mrs.Application.Cards.Queries.GetCardsWithPagination
{
    public class GetCardsDeleteWithPaginationQuery : IRequest<PaginatedList<CardDto>>
    {
        public string PageNumber { get; set; }
        public string PageSize { get; set; }
        public string SortType { get; set; }
        public string SortBy { get; set; }
        public string MemberNoFrom { get; set; }
        public string MemberNoTo { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCode { get; set; }
        public string RegisteredFrom { get; set; }
        public string RegisteredTo { get; set; }
        public string RegisteredBy { get; set; }
        public string Status { get; set; }
        public string ExpiredAt { get; set; }
    }

    public class GetCardsDeleteWithPaginationQueryHandler : IRequestHandler<GetCardsDeleteWithPaginationQuery, PaginatedList<CardDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;

        public GetCardsDeleteWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IConfiguration configuration, IIdentityService identityService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _configuration = configuration;
        }

        public async Task<PaginatedList<CardDto>> Handle(GetCardsDeleteWithPaginationQuery request, CancellationToken cancellationToken)
        {
            int pageNumber = 1;
            int.TryParse(request.PageNumber, out pageNumber);
            IQueryable<CardDto> query = _identityService.GetCardsWithUserQueryable();
            
            if (!string.IsNullOrEmpty(request.MemberNoFrom) && !string.IsNullOrEmpty(request.MemberNoTo))
            {
                bool validNumberFrom = long.TryParse(request.MemberNoFrom, out long memberFrom);
                bool validNumberTo = long.TryParse(request.MemberNoTo, out long memberTo);

                if (validNumberFrom && validNumberTo)
                {
                    query = query.Where(n => memberFrom <= Convert.ToInt64(n.MemberNo) && Convert.ToInt64(n.MemberNo) <= memberTo);
                }
                else
                {
                    return new PaginatedList<CardDto>(new List<CardDto>(), 0, 0, 0);
                }
            }
            if (!string.IsNullOrEmpty(request.CompanyCode)) query = query.Where(n => n.Company.CompanyCode.Contains(request.CompanyCode));
            if (!string.IsNullOrEmpty(request.StoreCode)) query = query.Where(n => n.Store.StoreCode.Contains(request.StoreCode));
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
            if (!string.IsNullOrEmpty(request.RegisteredBy)){
                IEnumerable<ApplicationUserDto> listUser = await _identityService.GetUsersId(request.RegisteredBy);
                query = query.Where(n => listUser.Select(u => u.Id).Contains(n.CreatedBy));
            }
            if (!string.IsNullOrEmpty(request.Status))
            {
                bool validStatus = Enum.TryParse(request.Status, out CardStatus status);
                if (validStatus) query = query.Where(n => n.Status == status);
            }
            if (!string.IsNullOrEmpty(request.ExpiredAt))
            {
                DateTime expiratedAt = DateTime.Now;
                const int inputOnlyYear = 4;
                if (request.ExpiredAt.Length == inputOnlyYear)
                {
                    bool validDate = DateTime.TryParseExact(request.ExpiredAt, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiratedAt);
                    if (validDate)
                    {
                        DateTime expiratedAtTo = expiratedAt.AddYears(1);
                        query = query.Where(n => n.ExpiredAt >= expiratedAt && n.ExpiredAt < expiratedAtTo);
                    }
                }
                else
                {
                    bool validDate = DateTime.TryParseExact(request.ExpiredAt, "yyyy/MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiratedAt);
                    if (validDate) query = query.Where(n => n.ExpiredAt == expiratedAt);
                }
            }
            if (!string.IsNullOrEmpty(request.SortBy)) {
                if(request.SortBy.Contains("storeCode")) query = request.SortType.Equals("asc")
                         ? query.OrderBy(n => n.Store.StoreCode)
                         : query.OrderByDescending(n => n.Store.StoreCode);
                else if(request.SortBy.Contains("companyCode")) query = request.SortType.Equals("asc")
                         ? query.OrderBy(n => n.Company.CompanyCode)
                         : query.OrderByDescending(n => n.Company.CompanyCode);
                else query = query.OrderByCustom(request.SortBy + " " + request.SortType.ToUpper());
            }
            else
            {
                query = query.OrderByDescending(n => n.CreatedAt);
            }

            int pageSize = query.Count();

            var records = await query.PaginatedListAsync(pageNumber, pageSize > 0 ? pageSize : 1);

            return records;
        }
    }
}

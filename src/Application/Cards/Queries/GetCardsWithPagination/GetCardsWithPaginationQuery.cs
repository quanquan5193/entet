using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Common.ExpressionExtension;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Queries.GetCardsWithPagination
{
    public class GetCardsWithPaginationQuery : IRequest<PaginatedList<CardDto>>
    {
        public string PageNumber { get; set; }
        public string PageSize { get; set; }
        public string SortType { get; set; }
        public string SortBy { get; set; }
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

    public class GetCardsWithPaginationQueryHandler : IRequestHandler<GetCardsWithPaginationQuery, PaginatedList<CardDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetCardsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<CardDto>> Handle(GetCardsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            int pageNumber = 1;
            int.TryParse(request.PageNumber, out pageNumber);
            int pageSize = 1;
            int.TryParse(request.PageSize, out pageSize);

            IQueryable<CardDto> query =  _identityService.GetCardsWithUserQueryable();

            if (!string.IsNullOrEmpty(request.MemberNo)) query = query.Where(n => n.MemberNo.Contains(request.MemberNo));
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

            // if logged in user has Company manager role, limit search data in his company
            if (_currentUserService.RoleLevel == RoleLevel.Level_6)
            {
                var companyId = 0;
                var store = await _identityService.GetStoreAsync(_currentUserService.UserId);
                if (store?.Company != null)
                {
                    companyId = store.Company.Id;
                }
                query = query.Where(c => c.CompanyId.HasValue && c.CompanyId.Value == companyId);
            }

            if (!string.IsNullOrEmpty(request.CompanyCode)) query = query.Where(n => n.Company.CompanyCode.Contains(request.CompanyCode));
            if (!string.IsNullOrEmpty(request.StoreCode)) query = query.Where(n => n.Store.StoreCode.Contains(request.StoreCode));
            if (!string.IsNullOrEmpty(request.Status))
            {
                bool validStatus = Enum.TryParse(request.Status, out CardStatus status);
                if (validStatus) query = query.Where(n => n.Status == status);
            }
            if (!string.IsNullOrEmpty(request.AcceptFrom))
            {
                DateTime fromDate = DateTime.Now;
                bool validDate = DateTime.TryParseExact(request.AcceptFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);
                if (validDate) query = query.Where(n => n.CreatedAt >= fromDate);
            }
            if (!string.IsNullOrEmpty(request.AcceptTo))
            {
                DateTime toDate = DateTime.Now;
                bool validDate = DateTime.TryParseExact(request.AcceptTo, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate);
                if (validDate) query = query.Where(n => n.CreatedAt < toDate.AddDays(1));
            }
            if (!string.IsNullOrEmpty(request.AcceptBy))
            {
                query = query.Where(n => n.CreatedByName.Contains(request.AcceptBy));
            }
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                if (request.SortBy.Contains("storeCode"))
                {
                    query = request.SortType.Equals("asc")
                         ? query.OrderBy(n => n.Store.StoreCode)
                         : query.OrderByDescending(n => n.Store.StoreCode);
                }
                else if (request.SortBy.Contains("companyCode"))
                {
                    query = request.SortType.Equals("asc")
                          ? query.OrderBy(n => n.Company.CompanyCode)
                          : query.OrderByDescending(n => n.Company.CompanyCode);
                }
                else
                {
                    query = query.OrderByCustom(request.SortBy + " " + request.SortType.ToUpper());
                }
            }
            else
            {
                query = query.OrderByDescending(n => n.CreatedAt);
            }

            var records = await query.PaginatedListAsync(pageNumber, pageSize);

            return records;
        }
    }
}

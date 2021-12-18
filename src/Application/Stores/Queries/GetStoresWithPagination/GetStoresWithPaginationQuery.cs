using AutoMapper;
using AutoMapper.QueryableExtensions;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mrs.Application.Stores.Queries.GetStores;
using Microsoft.EntityFrameworkCore;
using mrs.Domain.Entities;
using mrs.Application.Common.ExpressionExtension;
using System.Collections.Generic;
using mrs.Application.Common.Helpers;

namespace mrs.Application.Stores.Queries.GetStoresWithPagination
{
    public class GetStoresWithPaginationQuery : IRequest<PaginatedList<StoreDto>>
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string OrderBy { get; set; } = "Order";
        public string OrderType { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetStoresWithPaginationQueryProfile : Profile
    {
        public GetStoresWithPaginationQueryProfile()
        {
            CreateMap<Store, StoreDto>()
                .ForMember(a => a.CompanyId, b => b.MapFrom(c => c.Company.Id))
                .ForMember(a => a.CompanyCode, b => b.MapFrom(c => c.Company.CompanyCode))
                .ForMember(a => a.CompanyName, b => b.MapFrom(c => c.Company.CompanyName));
        }
    }

    public class GetStoresWithPaginationQueryHandler : IRequestHandler<GetStoresWithPaginationQuery, PaginatedList<StoreDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetStoresWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<StoreDto>> Handle(GetStoresWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Stores.Include(x => x.Company).Where(x => !x.IsDeleted && !x.Company.IsDeleted && x.Company.IsActive);

            #region check role
            IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);

            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId));
            }
            #endregion

            if (!string.IsNullOrEmpty(request.CompanyCode))
            {
                query = query.Where(x => x.Company.CompanyCode.Contains(request.CompanyCode));
            }
            if (!string.IsNullOrEmpty(request.CompanyName))
            {
                query = query.Where(x => x.Company.CompanyName.Contains(request.CompanyName));
            }
            if (!string.IsNullOrEmpty(request.StoreCode))
            {
                query = query.Where(x => x.StoreCode.Contains(request.StoreCode));
            }
            if (!string.IsNullOrEmpty(request.StoreName))
            {
                query = query.Where(x => x.StoreName.Contains(request.StoreName));
            }

            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = nameof(Store.Id);
            }
            if (string.IsNullOrEmpty(request.OrderType))
            {
                request.OrderType = "desc";
            }

            switch (request.OrderBy)
            {
                case "CompanyCode":
                    query = request.OrderType.Equals("asc") ? query.OrderBy(n => n.Company.CompanyCode) : query.OrderByDescending(n => n.Company.CompanyCode);
                    break;
                case "IsActive":
                    query = request.OrderType.Equals("asc") ? query.OrderBy(n => n.IsActive) : query.OrderByDescending(n => n.IsActive);
                    break;
                default:
                    query = query.OrderByCustom(request.OrderBy + " " + request.OrderType.ToUpper());
                    break;
            }

            return await query.ProjectTo<StoreDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}

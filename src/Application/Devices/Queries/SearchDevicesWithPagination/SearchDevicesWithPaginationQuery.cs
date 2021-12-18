using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.ExpressionExtension;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Queries.SearchDevicesWithPagination
{
    public class SearchDevicesWithPaginationQuery : IRequest<PaginatedList<SearchDevicesWithPaginationDto>>
    {
        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        public string DeviceCode { get; set; }

        public string OrderBy { get; set; } = "Id";

        public string OrderType { get; set; } = "DESC";

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    public class SearchDevicesWithPaginationProfile : Profile
    {
        const string activeStatus = "正常";
        const string dropedStatus = "無効";
        public SearchDevicesWithPaginationProfile()
        {
            CreateMap<Device, SearchDevicesWithPaginationDto>()
                .ForMember(x => x.CompanyCode, y => y.MapFrom(c => c.Store.Company.CompanyCode))
                .ForMember(x => x.CompanyName, y => y.MapFrom(c => c.Store.Company.CompanyName))
                .ForMember(x => x.NormalizedCompanyName, y => y.MapFrom(c => c.Store.Company.NormalizedCompanyName))
                .ForMember(x => x.StoreCode, y => y.MapFrom(c => c.Store.StoreCode))
                .ForMember(x => x.StoreName, y => y.MapFrom(c => c.Store.StoreName))
                .ForMember(x => x.NormalizedStoreName, y => y.MapFrom(c => c.Store.NormalizedStoreName))
                .ForMember(x => x.IsAutoLock, y => y.MapFrom(c => c.IsAutoLock ? activeStatus : dropedStatus))
                .ForMember(x => x.Status, y => y.MapFrom(c => c.IsActive ? activeStatus : dropedStatus));
        }
    }

    public class SearchDevicesWithPaginationQueryHandler : IRequestHandler<SearchDevicesWithPaginationQuery, PaginatedList<SearchDevicesWithPaginationDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public SearchDevicesWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }
        public async Task<PaginatedList<SearchDevicesWithPaginationDto>> Handle(SearchDevicesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Devices.Include(x => x.Store.Company).Where(x => !x.IsDeleted && x.Store.Company.IsActive && x.Store.IsActive);

            #region check role
            IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);

            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId));
            }
            #endregion

            if (!string.IsNullOrEmpty(request.CompanyCode))
            {
                query = query.Where(x => x.Store.Company.CompanyCode.Contains(request.CompanyCode));
            }
            if (!string.IsNullOrEmpty(request.CompanyName))
            {
                query = query.Where(x => x.Store.Company.CompanyName.Contains(request.CompanyName));
            }
            if (!string.IsNullOrEmpty(request.DeviceCode))
            {
                query = query.Where(x => x.DeviceCode.Contains(request.DeviceCode));
            }
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = nameof(Device.Id);
            }
            if (string.IsNullOrEmpty(request.OrderType))
            {
                request.OrderType = "desc";
            }

            switch (request.OrderBy)
            {
                case "CompanyCode":
                    query = request.OrderType.Equals("asc") ? query.OrderBy(n => n.Store.Company.CompanyCode) : query.OrderByDescending(n => n.Store.Company.CompanyCode);
                    break;
                case "NormalizedCompanyName":
                    query = request.OrderType.Equals("asc") ? query.OrderBy(n => n.Store.Company.NormalizedCompanyName) : query.OrderByDescending(n => n.Store.Company.NormalizedCompanyName);
                    break;
                case "StoreCode":
                    query = request.OrderType.Equals("asc") ? query.OrderBy(n => n.Store.StoreCode) : query.OrderByDescending(n => n.Store.StoreCode);
                    break;
                case "NormalizedStoreName":
                    query = request.OrderType.Equals("asc") ? query.OrderBy(n => n.Store.NormalizedStoreName) : query.OrderByDescending(n => n.Store.NormalizedStoreName);
                    break;
                case "Status":
                    query = request.OrderType.Equals("asc") ? query.OrderBy(n => n.IsActive) : query.OrderByDescending(n => n.IsActive);
                    break;
                default:
                    query = query.OrderByCustom(request.OrderBy + " " + request.OrderType.ToUpper());
                    break;
            }

            var listDeivceDto = await query.ProjectTo<SearchDevicesWithPaginationDto>(_mapper.ConfigurationProvider).PaginatedListAsync(request.PageNumber, request.PageSize);

            var index = 1;

            foreach (var item in listDeivceDto.Items)
            {
                item.No = index;
                index++;
            }

            return listDeivceDto;
        }
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Helpers.AzureKeyVaults;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Queries.SearchPendingRequestListWithPagination
{
    public class SearchPendingListWithPaginationQuery : IRequest<PaginatedList<PendingRequestListDto>>
    {
        public DateTime? StartCreateDate { get; set; }

        public DateTime? EndCreateDate { get; set; }

        public string MemberNo { get; set; }

        public int? RequestTypeId { get; set; }

        public int? PICStoreId { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int DeviceId { get; set; }
    }

    public class SearchPendingListProfile : Profile
    {
        public SearchPendingListProfile()
        {
            CreateMap<RequestsPending, PendingRequestListDto>()
                .ForMember(x => x.MemberNo, y => y.MapFrom(c => c.Member.MemberNo))
                .ForMember(x => x.OldMemberNo, y => y.MapFrom(c => c.Member.OldMemberNo))
                .ForMember(x => x.CustomerFirstName, y => y.MapFrom(c => c.Member.FirstName))
                .ForMember(x => x.CustomerLastName, y => y.MapFrom(c => c.Member.LastName))
                .ForMember(x => x.Remark, y => y.MapFrom(c => c.Member.Remark))
                .ForMember(x => x.RequestType, y => y.MapFrom(c => RequestTypeEnum.ReIssued.GetStringValue()))
                .ForMember(x=> x.PICName, y=>y.MapFrom(c=>c.Member.PICStore.PICName))
                ;
        }
    }

    public class SearchPendingListWIthPaginationQueryHandler : IRequestHandler<SearchPendingListWithPaginationQuery, PaginatedList<PendingRequestListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public SearchPendingListWIthPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }
        public async Task<PaginatedList<PendingRequestListDto>> Handle(SearchPendingListWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _context.RequestsPendings.Include(x => x.Member).Where(x => !x.IsDeleted);

            #region check role
            Device currentDevice = await _context.Devices.FirstOrDefaultAsync(n => !n.IsDeleted && n.Id == request.DeviceId);
            if (currentDevice == null) throw new NotFoundException("Device not found");
            var storeIds = new List<int>();
            var user = _identityService.GetUserDto(_currentUserService.UserId);
            if (user?.StoreId != null)
            {
                var userStore = await _context.Stores.Where(s => s.Id == user.StoreId.Value && !s.IsDeleted)
                    .Include(s => s.Company.Stores).FirstOrDefaultAsync();
                if (userStore != null)
                {
                    if (_currentUserService.RoleLevel == RoleLevel.Level_2)
                    {
                        storeIds.Add(user.StoreId.Value);
                    }
                    else if (_currentUserService.RoleLevel == RoleLevel.Level_3)
                    {
                        storeIds.AddRange(userStore.Company.Stores.Select(s => s.Id));
                    }
                }
            }
            query = query.Where(n => n.Member.RequestsPendings.FirstOrDefault() != null && n.Member.RequestsPendings.FirstOrDefault().StoreId != null && storeIds.Contains((int)n.Member.RequestsPendings.FirstOrDefault().StoreId));
            #endregion

            if (request.StartCreateDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt > request.StartCreateDate);
            }
            if (request.EndCreateDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt < request.EndCreateDate);
            }
            if (!string.IsNullOrWhiteSpace(request.MemberNo))
            {
                query = query.Where(x => x.Member.MemberNo.Contains(request.MemberNo));
            }
            if (request.RequestTypeId.HasValue && request.RequestTypeId != (int)RequestTypeEnum.ReIssued)
            {
                return null;
            }
            if (request.PICStoreId.HasValue)
            {
                query = query.Where(x => x.Member.PICStoreId == request.PICStoreId);
            }

            var listPending = await query.OrderByDescending(c => c.ReceiptedDatetime).ProjectTo<PendingRequestListDto>(_mapper.ConfigurationProvider).PaginatedListAsync(request.PageNumber, request.PageSize);

            foreach(var item in listPending.Items)
            {
                item.CustomerFirstName = await item.CustomerFirstName.ToDecryptStringAsync();
                item.CustomerLastName = await item.CustomerLastName.ToDecryptStringAsync();
                item.CustomerName = item.CustomerFirstName + " " + item.CustomerLastName;
            }    

            return listPending;
        }
    }
}

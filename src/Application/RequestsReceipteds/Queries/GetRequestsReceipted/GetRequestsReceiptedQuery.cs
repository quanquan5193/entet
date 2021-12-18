using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Helpers.AzureKeyVaults;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.RequestsReceipteds.Queries.GetRequestsReceipted
{
    public class GetRequestsReceiptedQuery : IRequest<RequestsReceiptedDetailsDto>
    {
        public int Id { get; set; }
    }

    public class GetRequestsReceiptedsWithPaginationQueryHandler : IRequestHandler<GetRequestsReceiptedQuery, RequestsReceiptedDetailsDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetRequestsReceiptedsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<RequestsReceiptedDetailsDto> Handle(GetRequestsReceiptedQuery request, CancellationToken cancellationToken)
        {
            #region check role
            IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);
            var query = _context.RequestsReceipteds.Where(x => !x.IsDeleted);
            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId));
            }
            else if (loggedInUserRoles.Contains(RoleLevel.Level_2) || loggedInUserRoles.Contains(RoleLevel.Level_3))
            {
                // get user's stores based on role
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

                query = query.Where(r => r.Device != null && storeIds.Contains(r.Device.StoreId));
            }
            #endregion

            var requestsReceipted = query.Where(r => r.Id == request.Id)
                .Include(x => x.Store)
                .ThenInclude(x => x.Company)
                .Include(x => x.Member.PICStore)
                .Include(x => x.Device.Store)
                .ThenInclude(n => n.Company)
                .ProjectTo<RequestsReceiptedDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefault();

            if (requestsReceipted == null)
            {
                throw new NotFoundException(nameof(requestsReceipted.Id), request.Id);
            }

            await AzureKeyVaultsHelper.DecryptMember(requestsReceipted.Member);
            return requestsReceipted;
        }
    }
}

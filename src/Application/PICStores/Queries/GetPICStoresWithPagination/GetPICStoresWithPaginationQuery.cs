using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.PICStores.Queries.GetPICStoresWithPagination
{
    public class GetPICStoresWithPaginationQuery : IRequest<PaginatedList<PICStoreDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string PICCode { get; set; }
        public string PICName { get; set; }
        public string OrderBy { get; set; }
        public string OrderType { get; set; }
        public int DeviceId { get; set; }
    }

    public class GetPICStoresWithPaginationQueryHandler : IRequestHandler<GetPICStoresWithPaginationQuery, PaginatedList<PICStoreDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetPICStoresWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<PICStoreDto>> Handle(GetPICStoresWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PICStores.Where(x => !x.IsDeleted && x.CreatedBy == _currentUserService.UserId);

            if (!string.IsNullOrEmpty(request.PICCode))
            {
                query = query.Where(r => r.PICCode.ToLower().Contains(request.PICCode.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.PICName))
            {
                query = query.Where(r => r.PICName.ToLower().Contains(request.PICName.ToLower().Trim()));
            }

            if (!string.IsNullOrEmpty(request.OrderBy) && !string.IsNullOrEmpty(request.OrderType) && new[] { "asc", "desc" }.Contains(request.OrderType.ToLower().Trim()))
            {
                var asc = request.OrderType.ToLower().Trim() == "asc";
                if (request.OrderBy.ToLower().Trim() == nameof(PICStoreDto.PICCode).ToLower())
                {
                    query = asc
                        ? query.OrderBy(x => x.PICCode)
                        : query.OrderByDescending(x => x.PICCode);
                }
                else if (request.OrderBy.ToLower().Trim() == nameof(PICStoreDto.RegistrationDate).ToLower())
                {
                    query = asc
                        ? query.OrderBy(x => x.CreatedAt)
                        : query.OrderByDescending(x => x.CreatedAt);
                }
                else if (request.OrderBy.ToLower().Trim() == nameof(PICStoreDto.Company).ToLower())
                {
                    // ignore
                }
                else if (request.OrderBy.ToLower().Trim() == nameof(PICStoreDto.Store).ToLower())
                {
                    // ignore
                }
                else if (request.OrderBy.ToLower().Trim() == nameof(PICStoreDto.PICName).ToLower())
                {
                    query = asc
                        ? query.OrderBy(x => x.PICName)
                        : query.OrderByDescending(x => x.PICName);
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.Id);
            }

            string storeName = "";
            string companyName = "";
            string normalizedCompanyName = "";
            string normalizedStoreName = "";
            ApplicationUserDto userInfo = _identityService.GetUserDto(_currentUserService.UserId);
            if (userInfo?.StoreId != null)
            {
                var store = await _context.Stores.Where(s => !s.IsDeleted && s.Id == userInfo.StoreId.Value)
                    .Include(s => s.Company).FirstOrDefaultAsync();
                if (store != null)
                {
                    if (store.IsActive && store.Company.IsActive){
                        storeName = store.StoreName;
                        companyName = store.Company.CompanyName;
                        normalizedCompanyName = store.Company.NormalizedCompanyName;
                        normalizedStoreName = store.NormalizedStoreName;
                    }
                    else
                    {
                        return new PaginatedList<PICStoreDto>(new List<PICStoreDto>(), 0, 1, request.PageSize);
                    }
                }
            }

            var picStores = await query.PaginatedListAsync(request.PageNumber, request.PageSize);
            var returnData = new List<PICStoreDto>();
            var index = (request.PageNumber - 1) * request.PageSize + 1;
            foreach (var item in picStores.Items)
            {
                var dto = new PICStoreDto
                {
                    Id = item.Id,
                    Index = index,
                    PICCode = item.PICCode,
                    RegistrationDate = item.CreatedAt,
                    Company = companyName,
                    NormalizedCompanyName  = normalizedCompanyName,
                    Store = storeName,
                    NormalizedStoreName = normalizedStoreName,
                    PICName = item.PICName
                };
                returnData.Add(dto);
                index++;
            }

            return new PaginatedList<PICStoreDto>(returnData, picStores.TotalCount, picStores.PageIndex, request.PageSize);
        }
    }
}

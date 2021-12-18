using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Queries.GetDevice
{
    public class GetDeviceByCodeQuery : IRequest<DeviceDto>
    {
        public int StoreId { get; set; }
        public string Code { get; set; }
    }

    public class GetDeviceByCodeQueryHandler : IRequestHandler<GetDeviceByCodeQuery, DeviceDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetDeviceByCodeQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<DeviceDto> Handle(GetDeviceByCodeQuery request, CancellationToken cancellationToken)
        {
            // get user's stores based on role
            var storeIds = await _identityService.GetStoreIdsAsync(_currentUserService.UserId, _currentUserService.RoleLevel);
            if (!storeIds.Contains(request.StoreId))
            {
                throw new NotFoundException(nameof(Device), request.Code);
            }

            // no need to check if device is active or not
            // client need this status to show message if it's inactive
            var device = await _context.Devices.Where(d => !d.IsDeleted && d.DeviceCode.ToLower() == request.Code.ToLower().Trim() && storeIds.Contains(d.StoreId) && d.Store.Company.IsActive)
                .Include(x => x.Store)
                .Include(x => x.Store.Company)
                .FirstOrDefaultAsync();
            if (device == null)
            {
                throw new NotFoundException(nameof(Device), request.Code);
            }

            var deviceDto = _mapper.Map<DeviceDto>(device);
            return deviceDto;
        }
    }
}

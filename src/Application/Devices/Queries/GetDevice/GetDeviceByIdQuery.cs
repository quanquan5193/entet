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
    public class GetDeviceByIdQuery : IRequest<DeviceDto>
    {
        public int StoreId { get; set; }
        public int Id { get; set; }
    }

    public class DeviceDtoProfile : Profile
    {
        public DeviceDtoProfile()
        {
            CreateMap<Device, DeviceDto>()
                .ForMember(x => x.Company, y => y.MapFrom(z => z.Store.Company));
        }
    }

    public class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, DeviceDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetDeviceByIdQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<DeviceDto> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
        {
            // no need to check if device is active or not
            // client need this status to show message if it's inactive
            var device = await _context.Devices.Where(d => !d.IsDeleted && d.Id == request.Id && d.Store.Company.IsActive)
                .Include(x => x.Store)
                .Include(x => x.Store.Company)
                .FirstOrDefaultAsync();
            if (device == null)
            {
                throw new NotFoundException(nameof(Device), request.Id);
            }

            // get user's stores based on role
            var storeIds = await _identityService.GetStoreIdsAsync(_currentUserService.UserId, _currentUserService.RoleLevel);
            if (!storeIds.Contains(device.StoreId))
            {
                throw new NotFoundException(nameof(Device), request.Id);
            }

            var deviceDto = _mapper.Map<DeviceDto>(device);
            return deviceDto;
        }
    }
}

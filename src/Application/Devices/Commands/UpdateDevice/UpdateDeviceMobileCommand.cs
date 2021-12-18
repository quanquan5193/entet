using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.UpdateDevice
{
    public class UpdateDeviceMobileCommand : IRequest
    {
        public int Id { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceMobileCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateDeviceCommandHandler(IApplicationDbContext context, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(UpdateDeviceMobileCommand request, CancellationToken cancellationToken)
        {
            var storeIds = await _identityService.GetStoreIdsAsync(_currentUserService.UserId, _currentUserService.RoleLevel);
            var entity = await _context.Devices.Where(d => !d.IsDeleted && d.IsActive && d.Id == request.Id)
                .FirstOrDefaultAsync();
            if (entity == null || !storeIds.Contains(entity.StoreId))
            {
                throw new NotFoundException(nameof(Device), request.Id);
            }


            entity.Lat = request.Lat;
            entity.Long = request.Long;
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

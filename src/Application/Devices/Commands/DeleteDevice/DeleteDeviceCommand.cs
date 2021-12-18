using MediatR;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.DeleteDevice
{
    public class DeleteDeviceCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        public DeleteDeviceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            var deviceEntity = _context.Devices.FirstOrDefault(x => x.Id == request.Id);

            if (deviceEntity == null)
            {
                throw new NotFoundException("ItemNotFounded");
            }

            if (deviceEntity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            deviceEntity.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(true);
        }
    }
}

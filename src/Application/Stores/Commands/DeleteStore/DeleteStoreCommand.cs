using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace mrs.Application.Stores.Commands.DeleteStore
{
    public class DeleteStoreCommand : IRequest
    {
        public int Id { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public DeleteStoreCommandHandler(IApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        public async Task<Unit> Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Stores
                .FirstOrDefaultAsync(l => l.Id == request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Store), request.Id);
            }

            if (entity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            if (await _identityService.IsStoreExistUser(request.Id) || await _context.Devices.AnyAsync(x => x.StoreId == request.Id && !x.IsDeleted))
            {
                throw new EntityDeletedException("StoreInUse");
            }

            var requestUpdateAt = request.UpdatedAt.HasValue ? ((DateTime)request.UpdatedAt).ToString("F") : null;
            var databaseUpdateAt = entity.UpdatedAt.HasValue ? ((DateTime)entity.UpdatedAt).ToString("F") : null;

            if (requestUpdateAt != databaseUpdateAt)
            {
                throw new DataChangedException("DataChanged");
            }

            entity.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

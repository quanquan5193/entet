using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace mrs.Application.Stores.Commands.UpdateStore
{
    public class UpdateStoreCommand : IRequest
    {
        public int Id { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string NormalizedStoreName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateStoreCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Stores.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Store), request.Id);
            }

            if (entity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            if (_context.Stores.Any(x => x.StoreCode.Equals(request.StoreCode) && x.Id != entity.Id && x.CompanyId == entity.CompanyId))
            {
                throw new DataExistedException("StoreCodeExisted");
            }

            if (_context.Stores.Any(x => x.Order == request.Order && x.Id != entity.Id && !x.IsDeleted))
            {
                throw new DataExistedException("OrderExisted");
            }

            var requestUpdateAt = request.UpdatedAt.HasValue ? ((DateTime)request.UpdatedAt).ToString("F") : null;
            var databaseUpdateAt = entity.UpdatedAt.HasValue ? ((DateTime)entity.UpdatedAt).ToString("F") : null;

            if (requestUpdateAt != databaseUpdateAt)
            {
                throw new DataChangedException("DataChanged");
            }

            entity.StoreCode = request.StoreCode;
            entity.StoreName = request.StoreName;
            entity.NormalizedStoreName = request.NormalizedStoreName;
            entity.IsActive = request.IsActive;
            entity.Order = request.Order;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

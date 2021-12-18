using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace mrs.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommand : IRequest
    {
        public int Id { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCompanyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var entity = _context.Companies.FirstOrDefault(x => x.Id == request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Company), request.Id);
            }

            if (entity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            var requestUpdateAt = request.UpdatedAt.HasValue ? ((DateTime)request.UpdatedAt).ToString("F") : null;
            var databaseUpdateAt = entity.UpdatedAt.HasValue ? ((DateTime)entity.UpdatedAt).ToString("F") : null;

            if (requestUpdateAt != databaseUpdateAt)
            {
                throw new DataChangedException("DataChanged");
            }

            if (_context.Stores.Any(x => x.CompanyId == request.Id && !x.IsDeleted))
            {
                throw new DataExistedException("StoreRegistered");
            }

            entity.IsDeleted = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace mrs.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommand : IRequest
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NormalizedCompanyName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCompanyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Companies.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Company), request.Id);
            }

            if (entity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            if (_context.Companies.Any(x => x.CompanyCode.Equals(request.CompanyCode) && x.Id != entity.Id))
            {
                throw new DataExistedException("CompanyCodeExisted");
            }

            if (_context.Companies.Any(x => x.Order == request.Order && x.Id != entity.Id))
            {
                throw new DataExistedException("OrderExisted");
            }

            var requestUpdateAt = request.UpdatedAt.HasValue ? ((DateTime)request.UpdatedAt).ToString("F") : null;
            var databaseUpdateAt = entity.UpdatedAt.HasValue ? ((DateTime)entity.UpdatedAt).ToString("F") : null;

            if (requestUpdateAt != databaseUpdateAt)
            {
                throw new DataChangedException("DataChanged");
            }

            entity.CompanyCode = request.CompanyCode;
            entity.CompanyName = request.CompanyName;
            entity.NormalizedCompanyName = request.NormalizedCompanyName;
            entity.Order = request.Order;
            entity.IsActive = request.IsActive;

            _context.Companies.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

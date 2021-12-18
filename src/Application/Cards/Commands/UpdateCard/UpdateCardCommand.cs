using MediatR;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Application.Companies.Queries.GetCompanies;
using mrs.Application.Stores.Queries.GetStores;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Commands.UpdateCard
{
    public class UpdateCardCommand : IRequest<string>
    {
        public int Id { get; set; }
        public CardStatus Status { get; set; }
        public FlatStoreDto Store { get; set; }
        public FlatCompanyDto Company { get; set; }
    }

    public class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand, string>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCardCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Cards.FindAsync(request.Id);

            if (entity == null || entity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            var company = _context.Companies.Find(request.Company.Id);
            if (company == null)
            {
                throw new NotFoundException("CompanyNotFound");
            }

            if (company.CompanyCode != request.Company.CompanyCode || request.Company.CompanyName != company.CompanyName)
            {
                throw new DataChangedException("DataChanged");
            }

            if (!company.IsActive)
            {
                throw new DataChangedException("IsNotActive");
            }

            var store = _context.Stores.Find(request.Store.Id);
            if (store == null)
            {
                throw new NotFoundException("CompanyNotFound");
            }
            if (store.StoreCode != request.Store.StoreCode || request.Store.StoreName != store.StoreName)
            {
                throw new DataChangedException("DataChanged");
            }
            if (!store.IsActive)
            {
                throw new DataChangedException("IsNotActive");
            }

            entity.Status = request.Status;
            entity.StoreId = request.Store.Id;
            entity.CompanyId = request.Company.Id;

            await _context.SaveChangesAsync(cancellationToken);

            return entity.MemberNo;
        }
    }
}

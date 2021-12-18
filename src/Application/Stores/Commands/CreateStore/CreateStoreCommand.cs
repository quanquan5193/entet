using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using mrs.Application.Common.Exceptions;
using System.Linq;

namespace mrs.Application.Stores.Commands.CreateStore
{
    public class CreateStoreCommand : IRequest<int>
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string NormalizedStoreName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public string CompanyCode { get; set; }
    }

    public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateStoreCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
        {
            var entity = new Store()
            {
                StoreCode = request.StoreCode,
                StoreName = request.StoreName,
                NormalizedStoreName = request.NormalizedStoreName,
                IsActive = request.IsActive,
                Order = request.Order
            };

            Company company = _context.Companies.FirstOrDefault(x => x.CompanyCode.Equals(request.CompanyCode) && !x.IsDeleted && x.IsActive
            );
            if (company == null) throw new NotFoundException(typeof(Company).Name, request.CompanyCode);
            company.Stores.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}

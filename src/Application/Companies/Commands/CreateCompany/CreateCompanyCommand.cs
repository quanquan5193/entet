using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommand : IRequest<int>
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NormalizedCompanyName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateCompanyCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var entity = new Company()
            {
                CompanyCode = request.CompanyCode,
                CompanyName = request.CompanyName,
                NormalizedCompanyName = request.NormalizedCompanyName,
                Order = request.Order,
                IsActive = request.IsActive
            };

            _context.Companies.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}

using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.PICStores.Commands.CreatePICStore
{
    public class CreatePICStoreCommand : IRequest<int>
    {
        public string PICCode { get; set; }
        public string PICName { get; set; }
    }

    public class CreatePICStoreCommandHandler : IRequestHandler<CreatePICStoreCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreatePICStoreCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreatePICStoreCommand request, CancellationToken cancellationToken)
        {
            var entity = new PICStore
            {
                PICCode = request.PICCode,
                PICName = request.PICName
            };

            _context.PICStores.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}

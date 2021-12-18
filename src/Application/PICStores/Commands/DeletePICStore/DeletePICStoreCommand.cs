using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.PICStores.Commands.DeletePICStore
{
    public class DeletePICStoreCommand : IRequest
    {
        public string PICCode { get; set; }
    }

    public class DeletePICStoreCommandHandler : IRequestHandler<DeletePICStoreCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DeletePICStoreCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(DeletePICStoreCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PICStores
                .Where(p => !p.IsDeleted &&
                            p.PICCode.ToLower() == request.PICCode.ToLower().Trim())
                .FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(PICStore), request.PICCode);
            }

            entity.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
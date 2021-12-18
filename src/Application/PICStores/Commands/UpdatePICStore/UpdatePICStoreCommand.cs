using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.PICStores.Commands.UpdatePICStore
{
    public class UpdatePICStoreCommand : IRequest
    {
        public int Id { get; set; }
        public string PICCode { get; set; }
        public string PICName { get; set; }
    }

    public class UpdatePICStoreCommandHandler : IRequestHandler<UpdatePICStoreCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UpdatePICStoreCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(UpdatePICStoreCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PICStores
                .Where(p => !p.IsDeleted && p.CreatedBy == _currentUserService.UserId && p.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(PICStore), request.PICCode);
            }

            entity.PICName = request.PICName;
            entity.PICCode = request.PICCode;
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

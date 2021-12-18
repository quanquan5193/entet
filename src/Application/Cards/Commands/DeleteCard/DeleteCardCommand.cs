using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Commands.DeleteCard
{
    public class DeleteCardCommand : IRequest<string>
    {
        public int Id { get; set; }
    }

    public class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand, string>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCardCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Cards
                .Where(l => l.Id == request.Id && !l.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            _context.Cards.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return request.Id.ToString();
        }
    }
}

using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Kids.Commands.DeleteKid
{
    public class DeleteKidCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteKidCommandHandler : IRequestHandler<DeleteKidCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteKidCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<bool> Handle(DeleteKidCommand request, CancellationToken cancellationToken)
        {
            MemberKid kid = await _context.MemberKids.FindAsync(request.Id);

            if (kid == null) return false;

            _context.MemberKids.Remove(kid);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}

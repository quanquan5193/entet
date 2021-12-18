using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.RequestsReceipteds.Commands
{
    public class DeleteHistoryCommand : IRequest
    {
        public int Id { get; set; }
    }

    public class DeleteHistoryCommandHandler : IRequestHandler<DeleteHistoryCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteHistoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteHistoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.RequestsReceipteds
                .Where(l => l.Id == request.Id && !l.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(RequestsReceipteds), request.Id);
            }

            var memberKids = await _context.MemberKids.Where(n => n.Member.Id == entity.MemberId).ToListAsync();
            if (memberKids.Any())
            {
                _context.MemberKids.RemoveRange(memberKids);
            }

            _context.RequestsReceipteds.Remove(entity);

            var member = await _context.Members.FirstOrDefaultAsync(n => n.Id == entity.MemberId);
            if (member != null)
            {
                var requestPendings = await _context.RequestsPendings.Where(n => n.MemberId == member.Id).ToListAsync();
                _context.RequestsPendings.RemoveRange(requestPendings);
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
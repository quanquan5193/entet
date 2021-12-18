using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Queries.CheckCardsAssigned
{
    public class CheckCardsAssignedQuery : IRequest<bool>
    {
        public string MemberNo { get; set; }
    }

    public class CheckCardsAssignedQueryHandler : IRequestHandler<CheckCardsAssignedQuery, bool>
    {
        private readonly IApplicationDbContext _context;
        public CheckCardsAssignedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CheckCardsAssignedQuery request, CancellationToken cancellationToken)
        {
            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);
            if (card == null)
            {
                return true;
            }
            if (card.Status != CardStatus.Unissued)
            {
                return true;
            }

            return false;
        }
    }
}

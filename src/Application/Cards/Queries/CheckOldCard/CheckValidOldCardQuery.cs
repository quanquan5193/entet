using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Queries.CheckOldCard
{
    public class CheckValidOldCardQuery : IRequest<string>
    {
        public string MemberNo { get; set; }
    }

    public class CheckValidOldCardQueryHandler : IRequestHandler<CheckValidOldCardQuery, string>
    {
        private readonly IApplicationDbContext _context;

        public CheckValidOldCardQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CheckValidOldCardQuery request, CancellationToken cancellationToken)
        {
            if (request.MemberNo.Length != 10) return CardCheckStatusCode.InvalidLength;

            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);

            if (card != null && card.Status == CardStatus.Unissued) return CardCheckStatusCode.ErrorCardStatus;

            return CardCheckStatusCode.OK;
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Queries.CheckCardDeleted
{
    public class CheckCardDeletedQuery : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class CheckCardDeletedQueryHandler : IRequestHandler<CheckCardDeletedQuery, bool>
    {
        private readonly IApplicationDbContext _context;
        public CheckCardDeletedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CheckCardDeletedQuery request, CancellationToken cancellationToken)
        {
            Card card = await _context.Cards.FindAsync(request.Id);
            if (card == null)
            {
                return true;
            }
            if (card.IsDeleted)
            {
                return true;
            }

            return false;
        }
    }
}

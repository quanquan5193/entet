using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Queries.CheckCardForReissueLostCard
{
    public class CheckCardForReissueLostCardQuery : IRequest<string>
    {
        public string MemberNo { get; set; }
    }

    public class CheckCardForReissueLostCardHandler : IRequestHandler<CheckCardForReissueLostCardQuery, string>
    {
        private readonly IApplicationDbContext _context;

        public CheckCardForReissueLostCardHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CheckCardForReissueLostCardQuery request, CancellationToken cancellationToken)
        {
            if (request.MemberNo.Length != 10) return CardCheckStatusCode.InvalidLength;
         
            if (request.MemberNo[0] != '0' && request.MemberNo[0] != '2')
            {
                return CardCheckStatusCode.ErrorCardStatus;
            }

            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);

            if (card != null && card.Status == CardStatus.Unissued) return CardCheckStatusCode.ErrorCardStatus;

            return CardCheckStatusCode.OK;
        }
    }
}


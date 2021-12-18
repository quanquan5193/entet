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

namespace mrs.Application.Cards.Queries.CheckCardForPointMigration
{
    public class CheckCardPointMigrationGivePointQuery : IRequest<string>
    {
        public string MemberNo { get; set; }
        public string MemberNoReceivePoint { get; set; }
    }

    public class CheckCardPointMigrationGivePointHandler : IRequestHandler<CheckCardPointMigrationGivePointQuery, string>
    {
        private readonly IApplicationDbContext _context;

        public CheckCardPointMigrationGivePointHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CheckCardPointMigrationGivePointQuery request, CancellationToken cancellationToken)
        {
            if (request.MemberNo.Length != 10) return CardCheckStatusCode.InvalidLength;

            if(request.MemberNoReceivePoint[0] == '0' && request.MemberNo[0] == '2')
            {
                return CardCheckStatusCode.InvalidBetweenGiveAndReceiveCard_1;
            }

            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);
            if (request.MemberNoReceivePoint[0] == '1')
            {
                if(request.MemberNo[0] != '0' && request.MemberNo[0] != '2')
                {
                    return CardCheckStatusCode.InvalidBetweenGiveAndReceiveCard_2;
                }
                if (card != null && card.Status != CardStatus.Issued)
                {
                    return CardCheckStatusCode.InvalidBetweenGiveAndReceiveCard_2;
                }
            }


            if (request.MemberNo[0] == '3' || request.MemberNo[0] == '6' || request.MemberNo[0] == '7' || request.MemberNo[0] == '8')
            {
                return CardCheckStatusCode.InvalidBetweenGiveAndReceiveCard_3;
            }

            if (request.MemberNo[0] == '1')
            {
                return CardCheckStatusCode.InvalidBetweenGiveAndReceiveCard_4;
            }

            return CardCheckStatusCode.OK;
        }
    }
}

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
    public class CheckCardPointMigrationReceivePointQuery : IRequest<string>
    {
        public string MemberNo { get; set; }
    }

    public class CheckCardPointMigrationReceivePointHandler : IRequestHandler<CheckCardPointMigrationReceivePointQuery, string>
    {
        private readonly IApplicationDbContext _context;

        public CheckCardPointMigrationReceivePointHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CheckCardPointMigrationReceivePointQuery request, CancellationToken cancellationToken)
        {
            if (request.MemberNo.Length != 10) return CardCheckStatusCode.InvalidLength;
            if (request.MemberNo[0] != '0' && request.MemberNo[0] != '1' && request.MemberNo[0] != '2')
            {
                return CardCheckStatusCode.ErrorCardStatus;
            }

            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);

            if (card != null && card.Status != CardStatus.Issued) return CardCheckStatusCode.ErrorCardStatus;

            return CardCheckStatusCode.OK;
        }
    }
}



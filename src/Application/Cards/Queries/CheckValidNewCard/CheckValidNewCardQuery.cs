using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Queries.CheckValidNewCard
{
    public class CheckValidNewCardQuery : IRequest<string>
    {
        public string MemberNo { get; set; }
    }
    public class CheckValidNewCardQueryHandler : IRequestHandler<CheckValidNewCardQuery, string>
    {
        private readonly IApplicationDbContext _context;
        private const string BarcodeThirdDigitFormat = "2";

        public CheckValidNewCardQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CheckValidNewCardQuery request, CancellationToken cancellationToken)
        {
            if (request.MemberNo.Length != 10) return CardCheckStatusCode.InvalidLength;
            if (!request.MemberNo[0].ToString().Equals(BarcodeThirdDigitFormat)) return CardCheckStatusCode.FirstLetterNotEqual2;

            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);

            if (card == null) return CardCheckStatusCode.NotExistCardInDatabase;
            if (card.Status != CardStatus.Unissued) return CardCheckStatusCode.ErrorCardStatus;
            //if (card.ExpiredAt.Year <= DateTime.Now.Year && card.ExpiredAt.Month <= DateTime.Now.Month) return CardCheckStatusCode.CardExpired;

            return CardCheckStatusCode.OK;
        }
    }
}

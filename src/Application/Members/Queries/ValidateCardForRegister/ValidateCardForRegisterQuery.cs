using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.ValidateCardForRegister
{
    public class ValidateCardForRegisterQuery : IRequest<CardValidationDto>
    {
        public string MemberNo { get; set; }
    }

    public class ValidateCardForRegisterQueryHandler : IRequestHandler<ValidateCardForRegisterQuery, CardValidationDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private static char BarcodeThirdDigitFormat = '2';

        public ValidateCardForRegisterQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _identityService = identityService;
        }

        public async Task<CardValidationDto> Handle(ValidateCardForRegisterQuery request, CancellationToken cancellationToken)
        {
            CardValidationDto result = new CardValidationDto()
            {
                IsValidated = false,
                Message = String.Empty
            };

            // Following card validation in step 1 Scan Barcode screen. The third digit of barcode must be 2
            if (!request.MemberNo[0].Equals(BarcodeThirdDigitFormat))
            {
                result.Message = "お客様番号のフォーマットが正しくありません。";
                return result;
            }

            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);

            if (card == null)
            {
                result.Message = "お客様番号は存在していません。";
                return result;
            }

            if (card.Status != CardStatus.Unissued)
            {
                result.Message = "会員登録済みです";
                return result;
            }

            result.IsValidated = true;
            return result;
        }
    }
}

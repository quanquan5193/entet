using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Kids.Commands.UpdateKid
{
    public class UpdateKidCommandValidator : AbstractValidator<UpdateKidCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private static readonly char[] MEMBERNO_START_DIGITS = { '0', '1', '2', '3', '6', '8' };


        public UpdateKidCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _identityService = identityService;

            RuleFor(x => x.MemberNo)
                .Length(10).When(x => !string.IsNullOrWhiteSpace(x.MemberNo))
                    .WithMessage("MemberNo length must be 10");
            RuleFor(x => x.Email)
                 .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                     .WithMessage("Email is wrong format");
        }

        /// <summary>
        /// Check start digit
        /// </summary>
        /// <param name="memberNo"></param>
        /// <returns></returns>
        public bool IsStartWithDiditTwo(string memberNo)
        {
            return MEMBERNO_START_DIGITS.Contains(memberNo[0]);
        }

        /// <summary>
        /// Check card assign store
        /// </summary>
        /// <param name="memberNo"></param>
        /// <returns></returns>
        public async Task<bool> IsCardAssignStore(string memberNo, CancellationToken cancellationToken)
        {
            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(memberNo) && !x.IsDeleted);

            if (card == null) return false;

            return card.StoreId != null;
        }

    }
}

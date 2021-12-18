using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Commands.RegisterKidsClub
{
    public class RegisterKidsClubCommandValidator : AbstractValidator<RegisterKidsClubCommand>
    {
        public IApplicationDbContext _context { get; set; }

        public RegisterKidsClubCommandValidator(IApplicationDbContext context)
        {
            _context = context;
            RuleFor(x => x.MemberNo)
                .NotEmpty()
                    .WithMessage("MemberNo is required")
                .Length(10)
                    .WithMessage("MemberNo length must be 10")
                .MustAsync(CheckCardStatus)
                    .WithMessage("Card is not issued");
            RuleFor(x => x.DeviceId)
                .NotEmpty()
                    .WithMessage("DeviceCode is required")
                .MustAsync(IsDeviceExist)
                    .WithMessage("Device is not exist in database");
            RuleFor(x => x.MemberKids)
                .NotEmpty()
                    .WithMessage("MemberKids is required");
        }

        /// <summary>
        /// Check device exist
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> IsDeviceExist(int deviceId, CancellationToken cancellationToken)
        {
            return await _context.Devices.AnyAsync(x => x.Id == deviceId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckCardStatus(string memberNo, CancellationToken cancellationToken)
        {
            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(memberNo));

            if (card == null)
                return true;

            if (card.Status == CardStatus.Issued)
                return true;

            return false;
        }
    }
}

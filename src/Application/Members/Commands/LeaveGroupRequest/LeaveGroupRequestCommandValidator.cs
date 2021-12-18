using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Commands.LeaveGroupRequest
{
    public class LeaveGroupRequestCommandValidator : AbstractValidator<LeaveGroupRequestCommand>
    {
        private readonly IApplicationDbContext _context;

        public LeaveGroupRequestCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.DeviceId)
                .NotEmpty()
                    .WithMessage("DeviceId is required")
                .MustAsync(IsDeviceExist)
                    .WithMessage("Device is not exist in database");
            RuleFor(x => x.MemberNo)
                .NotEmpty()
                    .WithMessage("MemberNo is required")
                .Length(10)
                    .WithMessage("MemberNo length must be 10");
            RuleFor(x => x.FirstName)
                .NotEmpty()
                    .WithMessage("FirstName is required");
            RuleFor(x => x.LastName)
                .NotEmpty()
                    .WithMessage("LastName is required");
            RuleFor(x => x.FuriganaLastName)
                .NotEmpty()
                    .WithMessage("FuriganaLastName is required");
            RuleFor(x => x.FuriganaFirstName)
                .NotEmpty()
                    .WithMessage("FuriganaFirstName is required");
            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                    .WithMessage("DateOfBirth is required");
            RuleFor(x => x.MobilePhone)
                .NotEmpty()
                    .WithMessage("MobilePhone is required");
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
    }
}

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Commands.UpdateMemberNo
{
    public class UpdateMemberNoCommandValidator : AbstractValidator<UpdateMemberNoCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateMemberNoCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.OldMemberNo)
                .NotEmpty()
                    .WithMessage("OldMemberNo is required")
                .Length(10)
                    .WithMessage("OldMemberNo length must be 10");
            RuleFor(x => x.NewMemberNo)
                .NotEmpty()
                    .WithMessage("NewMemberNo is required")
                .Length(10)
                    .WithMessage("NewMemberNo length must be 10");
            RuleFor(x => x.DeviceId)
                .NotEmpty()
                    .WithMessage("DeviceCode is required")
                .MustAsync(IsDeviceExist)
                    .WithMessage("Device is not exist in database");
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

using mrs.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.CreateDevice
{
    public class CreateDeviceCommandValidator : AbstractValidator<CreateDeviceCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateDeviceCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.DeviceCode)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(20).WithMessage("Title must not exceed 200 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
            RuleFor(x => x.CompanyCode)
                .MustAsync(CheckCompanyFlag).WithMessage("hiddenFlagCompany");
        }

        public async Task<bool> BeUniqueTitle(string deviceCode, CancellationToken cancellationToken)
        {
            return await _context.Devices
                .AllAsync(l => l.DeviceCode != deviceCode);
        }

        private async Task<bool> CheckCompanyFlag(string code, CancellationToken arg2)
        {
            return await _context.Companies.AnyAsync(x => x.CompanyCode.Equals(code) && x.IsActive);
        }
    }
}

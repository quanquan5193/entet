using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ApplicationUser.Command.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _context;

        public CreateUserCommandValidator(IIdentityService identityService, IApplicationDbContext context)
        {
            _identityService = identityService;
            _context = context;

            RuleFor(x => x.UserName)
                .MustAsync(CheckUserNameExist).When(x => !string.IsNullOrWhiteSpace(x.UserName))
                .WithMessage("createFailLoginID");
            RuleFor(x => x.CompanyCode)
                .MustAsync(CheckCompanyFlag).WithMessage("hiddenFlagCompany");
        }

        private async Task<bool> CheckCompanyFlag(string code, CancellationToken arg2)
        {
            return await _context.Companies.AnyAsync(x => x.CompanyCode.Equals(code) && x.IsActive);
        }

        public async Task<bool> CheckUserNameExist(string loginID, CancellationToken cancellationToken)
        {
            return !await _identityService.IsUserNameExist(loginID);
        }
    }
}

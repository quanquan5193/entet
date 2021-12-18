using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ApplicationUser.Command.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _context;

        public UpdateUserCommandValidator(IIdentityService identityService, IApplicationDbContext context)
        {
            _identityService = identityService;
            _context = context;

            RuleFor(x => x)
                .MustAsync(CheckUserNameExist).When(x => !string.IsNullOrWhiteSpace(x.UserName))
                .WithMessage("updateFailLoginID")
                .MustAsync(CheckOldDataChange).When(x => !string.IsNullOrWhiteSpace(x.UserName))
                .WithMessage("updateFailMasterDataChange");
            RuleFor(x => x.CompanyCode)
                .MustAsync(CheckCompanyFlag).WithMessage("hiddenFlagCompany");
        }

        public async Task<bool> CheckUserNameExist(UpdateUserCommand model, CancellationToken cancellationToken)
        {
            if (model.UserName == model.OldUserName)
            {
                return true;
            }
            return !await _identityService.IsUserNameExist(model.UserName);
        }

        public async Task<bool> CheckOldDataChange(UpdateUserCommand model, CancellationToken cancellationToken)
        {
            return await _identityService.IsCompanyStoreNotChange(model.Id, model.OldCompanyCode,model.OldStoreCode);
        }

        private async Task<bool> CheckCompanyFlag(string code, CancellationToken arg2)
        {
            return await _context.Companies.AnyAsync(x => x.CompanyCode.Equals(code) && x.IsActive);
        }
    }
}

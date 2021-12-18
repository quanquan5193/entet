using mrs.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCompanyCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.CompanyCode)
                .NotEmpty().WithMessage("CompanyCode is required.")
                .MaximumLength(4).MinimumLength(4).WithMessage("CompanyCode must 4 characters.");
            RuleFor(v => v.CompanyName)
                .NotEmpty().WithMessage("CompanyName is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(v => v.NormalizedCompanyName)
                .NotEmpty().WithMessage("NormalizedCompanyName is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(v => v.Order)
                .LessThanOrEqualTo(1000000).WithMessage("Order must less than 1,000,000");
        }

        public async Task<bool> BeUniqueTitle(string code, CancellationToken cancellationToken)
        {

            return await _context.Companies
                .AllAsync(l => l.CompanyCode != code);
        }

        public async Task<bool> BeUniqueOrder(int order, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AllAsync(l => l.Order != order);
        }
    }
}

using mrs.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateCompanyCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.CompanyCode)
                .NotEmpty().WithMessage("CompanyCode is required.")
                .MaximumLength(4).WithMessage("Company Code must not exceed 4 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("CompanyCodeExisted");
            RuleFor(v => v.CompanyName)
                .NotEmpty().WithMessage("CompanyName is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(v => v.NormalizedCompanyName)
                .NotEmpty().WithMessage("NormalizedCompanyName is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(v => v.Order)
                .LessThan(1000000).WithMessage("Order must less than 1,000,000")
                .MustAsync(BeUniqueOrder).WithMessage("OrderExisted");
        }

        public async Task<bool> BeUniqueTitle(string code, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .AllAsync(l => l.CompanyCode != code);
        }

        public async Task<bool> BeUniqueOrder(int order, CancellationToken cancellationToken)
        {
            var check = await _context.Companies
                .AnyAsync(l => l.Order == order && !l.IsDeleted);
            return !check;
        }
    }
}

using mrs.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Stores.Commands.CreateStore
{
    public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateStoreCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.StoreCode)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(v => v.StoreName)
                .NotEmpty().WithMessage("StoreName is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(v => v.NormalizedStoreName)
                .NotEmpty().WithMessage("NormalizedStoreName is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
            RuleFor(v => v.Order)
                .LessThan(1000000).WithMessage("Order must less than 1,000,000")
                .MustAsync(BeUniqueOrder).WithMessage("OrderExisted");
            RuleFor(v => v.CompanyCode)
                .NotEmpty().WithMessage("CompanyCode is required.")
                .MustAsync(CheckCompanyFlag).WithMessage("hiddenFlagCompany");
            RuleFor(v => v)
                .MustAsync(BeUniqueTitleInCompany).WithMessage("StoreCodeExisted");
        }

        private async Task<bool> CheckCompanyFlag(string code, CancellationToken arg2)
        {
            return await _context.Companies.AnyAsync(x => x.CompanyCode.Equals(code) && x.IsActive);
        }

        public async Task<bool> BeUniqueTitleInCompany(CreateStoreCommand store, CancellationToken cancellationToken)
        {
            var check = await _context.Stores.Include(n => n.Company)
                .AnyAsync(l => l.StoreCode == store.StoreCode && l.Company.CompanyCode == store.CompanyCode);
            return !check;
        }

        public async Task<bool> BeUniqueOrder(int order, CancellationToken cancellationToken)
        {
            var check = await _context.Stores.AnyAsync(l => l.Order == order && !l.IsDeleted);
            return !check;
        }
    }
}

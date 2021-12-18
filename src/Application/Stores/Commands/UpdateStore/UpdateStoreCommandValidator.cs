using mrs.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Stores.Commands.UpdateStore
{
    public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateStoreCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.StoreCode)
                .NotEmpty().WithMessage("StoreCode is required.")
                .MaximumLength(4).MinimumLength(4).WithMessage("StoreCode must 4 characters.");
        }
    }
}

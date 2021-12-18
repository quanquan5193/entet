using mrs.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Commands.CreateCard
{
    public class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateCardCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(v => v.MemberNo)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .MustAsync(BeUniqueTitle).WithMessage("The specified title already exists.");
        }

        public async Task<bool> BeUniqueTitle(string memberNo, CancellationToken cancellationToken)
        {
            return await _context.Cards
                .AllAsync(l => l.MemberNo != memberNo);
        }
    }
}

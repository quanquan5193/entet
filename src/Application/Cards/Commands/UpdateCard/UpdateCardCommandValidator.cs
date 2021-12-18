using mrs.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Commands.UpdateCard
{
    public class UpdateCardCommandValidator : AbstractValidator<UpdateCardCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCardCommandValidator(IApplicationDbContext context)
        {
            _context = context;

        }
    }
}

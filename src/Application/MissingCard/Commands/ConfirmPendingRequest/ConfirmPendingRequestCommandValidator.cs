using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Commands.ConfirmPendingRequest
{
    public class ConfirmPendingRequestCommandValidator : AbstractValidator<ConfirmPendingRequestCommand>
    {
        public ConfirmPendingRequestCommandValidator()
        {
            RuleFor(x => x.OldMemberNo)
                .NotEmpty()
                    .WithMessage("OldMemberNo is required")
                .Length(10)
                    .WithMessage("OldMemberNo must be 10");
        }
    }
}

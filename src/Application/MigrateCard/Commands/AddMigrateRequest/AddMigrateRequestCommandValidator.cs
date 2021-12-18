using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.MigrateCard.Commands.AddMigrateRequest
{
    public class AddMigrateRequestCommandValidator : AbstractValidator<AddMigrateRequestCommand>
    {
        public AddMigrateRequestCommandValidator()
        {
            RuleFor(x => x.MemberNo)
                .NotEmpty()
                    .WithMessage("MemberNo is required")
                .Length(10)
                    .WithMessage("MemberNo length must be 10");

            RuleFor(x => x.OldMemberNo)
                .NotEmpty()
                    .WithMessage("OldMemberNo is required")
                .Length(10)
                    .WithMessage("OldMemberNo length must be 10");
        }
    }
}

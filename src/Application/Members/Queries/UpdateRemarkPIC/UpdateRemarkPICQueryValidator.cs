using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.UpdateRemarkPIC
{
    public class UpdateRemarkPICQueryValidator : AbstractValidator<UpdateRemarkPICQuery>
    {
        public UpdateRemarkPICQueryValidator()
        {
            RuleFor(v => v.RequestId)
                .NotEmpty().WithMessage("RequestId is required.");
        }
    }
}

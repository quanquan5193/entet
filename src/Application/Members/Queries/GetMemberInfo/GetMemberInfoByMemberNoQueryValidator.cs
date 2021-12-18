using FluentValidation;
using mrs.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.GetMemberInfo
{
    public class GetMemberInfoByMemberNoQueryValidator : AbstractValidator<GetMemberInfoByMemberNoQuery>
    {
        public GetMemberInfoByMemberNoQueryValidator()
        {
            RuleFor(v => v.MemberNo)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
        }
    }
}

using FluentValidation;

namespace mrs.Application.Members.Queries.ValidateCardForRegister
{
    public class ValidateCardForRegisterQueryValidator : AbstractValidator<ValidateCardForRegisterQuery>
    {
        public ValidateCardForRegisterQueryValidator()
        {
            RuleFor(x => x.MemberNo)
                .NotEmpty()
                .WithMessage("Barcode is required");
        }
    }
}

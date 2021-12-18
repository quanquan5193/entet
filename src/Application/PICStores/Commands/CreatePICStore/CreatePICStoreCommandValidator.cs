﻿using FluentValidation;

namespace mrs.Application.PICStores.Commands.CreatePICStore
{
    public class CreatePICStoreCommandValidator : AbstractValidator<CreatePICStoreCommand>
    {
        public CreatePICStoreCommandValidator()
        {
            RuleFor(v => v.PICCode)
                .NotEmpty().WithMessage("PIC code is required.")
                .MaximumLength(6).WithMessage("PIC code must not exceed 6 characters.");

            RuleFor(v => v.PICName)
                .NotEmpty().WithMessage("PIC name is required.")
                .MaximumLength(50).WithMessage("PIC name must not exceed 50 characters.");
        }
    }
}

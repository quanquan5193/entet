using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Commands.UpdateMember
{
    public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateMemberCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Member.DeviceId)
                .NotEmpty()
                    .WithMessage("DeviceId is required")
                .MustAsync(IsDeviceExist)
                    .WithMessage("Device is not exist in database");
            RuleFor(x => x.Member.MemberNo)
                .NotEmpty()
                    .WithMessage("MemberNo is required")
                .Length(10)
                    .WithMessage("MemberNo length must be 10");
                //.MustAsync(CheckMemberNoExist).When(x => x.Member != null)
                //    .WithMessage("Cards is not exist");
            RuleFor(x => x.Member.FirstName)
                .NotEmpty()
                    .WithMessage("FirstName is required");
            RuleFor(x => x.Member.LastName)
                .NotEmpty()
                    .WithMessage("LastName is required");
            RuleFor(x => x.Member.FuriganaFirstName)
                .NotEmpty()
                    .WithMessage("FuriganaFirstName is required");
            RuleFor(x => x.Member.FuriganaLastName)
                .NotEmpty().WithMessage("FuriganaLastName is required");
            RuleFor(x => x.Member.Street)
                .NotEmpty().WithMessage("Street is required");
            RuleFor(x => x.Member.District)
                .NotEmpty().WithMessage("District is required");
            RuleFor(x => x.Member.Province)
                .NotEmpty().WithMessage("Province is required");
            RuleFor(x => (int)x.Member.Sex)
                .NotEmpty()
                    .WithMessage("Sex is required")
                .LessThanOrEqualTo(3)
                    .WithMessage("Sex must be 1 2 3")
                .GreaterThanOrEqualTo(1)
                    .WithMessage("Sex must be 1 2 3");
            RuleFor(x => x.Member.FixedPhone)
                .NotEmpty().When(x => string.IsNullOrEmpty(x.Member.MobilePhone))
                    .WithMessage("FixedPhone is required")
                .MinimumLength(10).When(x => string.IsNullOrEmpty(x.Member.MobilePhone))
                    .WithMessage("FixedPhone length must be 10-15 digits")
                .MaximumLength(15).When(x => string.IsNullOrEmpty(x.Member.MobilePhone))
                    .WithMessage("FixedPhone length must be 10-15 digits");
            RuleFor(x => x.Member.MobilePhone)
                .NotEmpty().When(x => string.IsNullOrEmpty(x.Member.FixedPhone))
                    .WithMessage("MobilePhone is required")
                .MinimumLength(10).When(x => string.IsNullOrEmpty(x.Member.FixedPhone))
                    .WithMessage("MobilePhone length must be 10-15 digits")
                .MaximumLength(15).When(x => string.IsNullOrEmpty(x.Member.FixedPhone))
                    .WithMessage("MobilePhone length must be 10-15 digits");
            RuleFor(x => x.Member.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Member.Email))
                    .WithMessage("Email is wrong format");
            RuleFor(x => x.Member.DateOfBirth)
                .NotEmpty()
                    .WithMessage("Date of birth is required")
                .LessThanOrEqualTo(DateTime.Now)
                    .WithMessage($"Date of birth must less than {DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}");
        }

        /// <summary>
        /// Check Barcode exists in the database
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberNoExist(string memberNo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(memberNo) || memberNo.Length != 10)
            {
                return true;
            }

            return await _context.Cards.AnyAsync(x => x.MemberNo.Equals(memberNo) && !x.IsDeleted);
        }

        /// <summary>
        /// Check device exist
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> IsDeviceExist(int deviceId, CancellationToken cancellationToken)
        {
            return await _context.Devices.AnyAsync(x => x.Id == deviceId);
        }
    }
}

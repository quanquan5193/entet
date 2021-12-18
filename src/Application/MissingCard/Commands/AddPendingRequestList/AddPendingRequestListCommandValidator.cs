using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Commands.AddPendingRequestList
{
    public class AddPendingRequestListCommandValidator : AbstractValidator<AddPendingRequestListCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;

        public AddPendingRequestListCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService, IIdentityService identityService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _identityService = identityService;

            RuleFor(x => x.MemberNo)
                .NotEmpty()
                    .WithMessage("MemberNo is required")
                .Length(10)
                    .WithMessage("MemberNo length must be 10")
                .MustAsync(CheckMemberNoExist)
                    .WithMessage("Cards is not exist")
                .MustAsync(CheckMemberNoStatus)
                    .WithMessage("会員登録済みです。")
                .MustAsync(CheckMemberNoAssigned)
                    .WithMessage("Card has been assigned");
            RuleFor(x => x.FirstName)
                .NotEmpty()
                    .WithMessage("FirstName is required");
            RuleFor(x => x.LastName)
                .NotEmpty()
                    .WithMessage("LastName is required");
            RuleFor(x => x.FuriganaFirstName)
                .NotEmpty()
                    .WithMessage("FuriganaFirstName is required");
            RuleFor(x => x.FuriganaLastName)
                .NotEmpty().WithMessage("FuriganaLastName is required");
            RuleFor(x => (int)x.Sex)
                .NotEmpty()
                    .WithMessage("Sex is required")
                .LessThanOrEqualTo(3)
                    .WithMessage("Sex must be 1 2 3")
                .GreaterThanOrEqualTo(1)
                    .WithMessage("Sex must be 1 2 3");
            RuleFor(x => x.FixedPhone)
                .NotEmpty().When(x => string.IsNullOrEmpty(x.MobilePhone))
                    .WithMessage("FixedPhone or MobilePhone is required")
                .MinimumLength(10).When(x => string.IsNullOrEmpty(x.MobilePhone))
                    .WithMessage("FixedPhone length must be 10-15 digits")
                .MaximumLength(15).When(x => string.IsNullOrEmpty(x.MobilePhone))
                    .WithMessage("FixedPhone length must be 10-15 digits");
            RuleFor(x => x.MobilePhone)
                .NotEmpty().When(x => string.IsNullOrEmpty(x.FixedPhone))
                    .WithMessage("FixedPhone or MobilePhone is required")
                .MinimumLength(10).When(x => string.IsNullOrEmpty(x.FixedPhone))
                    .WithMessage("MobilePhone length must be 10-15 digits")
                .MaximumLength(15).When(x => string.IsNullOrEmpty(x.FixedPhone))
                    .WithMessage("MobilePhone length must be 10-15 digits");
            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                    .WithMessage("Email is wrong format");
            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                    .WithMessage("Date of birth is required")
                .LessThanOrEqualTo(DateTime.Now)
                    .WithMessage($"Date of birth must less than {DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}");
            RuleFor(x => x.ZipcodeId)
                .Length(7)
                .WithMessage("ZipcodeId length must be 7");
        }

        /// <summary>
        /// Check Barcode exists in the database
        /// </summary>
        /// <param name="memberNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberNoExist(string memberNo, CancellationToken cancellationToken)
        {
            return await _context.Cards.AnyAsync(x => x.MemberNo.Equals(memberNo));
        }

        /// <summary>
        /// Check card status is unissued
        /// </summary>
        /// <param name="memberNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberNoStatus(string memberNo, CancellationToken cancellationToken)
        {
            return await _context.Cards.AnyAsync(x => x.MemberNo.Equals(memberNo) && x.Status == CardStatus.Unissued);

        }

        /// <summary>
        /// Check card's Store
        /// </summary>
        /// <param name="memberNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberNoStore(string memberNo, CancellationToken cancellationToken)
        {
            Store store = await _identityService.GetStoreAsync(_currentUserService.UserId);
            return await _context.Cards.AnyAsync(x => x.MemberNo.Equals(memberNo) && x.StoreId == store.Id);

        }

        /// <summary>
        /// Check card has been assigned
        /// </summary>
        /// <param name="memberNo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CheckMemberNoAssigned(string memberNo, CancellationToken cancellationToken)
        {
            return await _context.Cards.AnyAsync(x => x.MemberNo.Equals(memberNo) && x.Status == CardStatus.Unissued);
        }
    }
}

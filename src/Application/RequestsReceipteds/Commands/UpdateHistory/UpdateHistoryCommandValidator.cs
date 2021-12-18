using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.RequestsReceipteds.Commands.UpdateHistory
{
    public class UpdateHistoryCommandValidator : AbstractValidator<UpdateHistoryCommand>
    {
        private readonly IApplicationDbContext _context;
        public UpdateHistoryCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x).Must(IsValidMemberNo).WithMessage("memberNo");
            RuleFor(x => x).Must(IsValidOldMemberNo).WithMessage("oldMemberNo");
        }

        private bool IsValidMemberNo(UpdateHistoryCommand arg)
        {
            RequestsReceipted requestsReceipted = _context.RequestsReceipteds.Include(x => x.Member).Where(x => x.Id == arg.Id).FirstOrDefault();

            if (arg.MemberInfo.MemberNo.Equals(requestsReceipted.Member.MemberNo))
            {
                return true;
            }

            Card card = _context.Cards.FirstOrDefault(x => x.MemberNo.Equals(arg.MemberInfo.MemberNo) && !x.IsDeleted);
            string memberNo = arg.MemberInfo.MemberNo;

            switch (requestsReceipted.ReceiptedTypeId)
            {
                case (int)RequestTypeEnum.New:
                    if (!memberNo[0].Equals('2') || card == null || card.Status != CardStatus.Unissued) return false;
                    else return true;

                case (int)RequestTypeEnum.Switch:
                    if (!memberNo[0].Equals('2') || card == null || card.Status != CardStatus.Unissued) return false;
                    else return true;

                case (int)RequestTypeEnum.ReIssued:
                    if (!memberNo[0].Equals('2') || card == null || card.Status != CardStatus.Unissued) return false;
                    else return true;

                case (int)RequestTypeEnum.ChangeCard:
                    if (!(memberNo[0].Equals('0') || memberNo[0].Equals('2'))) return false;
                    else if (card != null && card.Status == CardStatus.Unissued) return false;
                    else return true;

                case (int)RequestTypeEnum.LeaveGroup:
                    if (!(memberNo[0].Equals('0') || memberNo[0].Equals('2'))) return false;
                    else if (card != null && card.Status == CardStatus.Unissued) return false;
                    else return true;

                case (int)RequestTypeEnum.PMigrate:
                    if (!(memberNo[0].Equals('0') || memberNo[0].Equals('1') || memberNo[0].Equals('2'))) return false;
                    else if (card != null && card.Status != CardStatus.Issued) return false;
                    else return true;

                case (int)RequestTypeEnum.Kid:
                    if (memberNo[0].Equals('4') || memberNo[0].Equals('5') || memberNo[0].Equals('7')) return false;
                    else if (memberNo[0].Equals('0') && memberNo[1].Equals('9')) return false;
                    else if (card != null && card.Status != CardStatus.Issued) return false;
                    else return true;

                default:
                    return true;
            }
        }

        private bool IsValidOldMemberNo(UpdateHistoryCommand arg)
        {
            RequestsReceipted requestsReceipted = _context.RequestsReceipteds.Include(x => x.Member).Where(x => x.Id == arg.Id).FirstOrDefault();
            string oldMemberNo = arg.MemberInfo.OldMemberNo;
            Card card = _context.Cards.FirstOrDefault(x => x.MemberNo.Equals(oldMemberNo) && !x.IsDeleted);
            if (oldMemberNo != null && oldMemberNo.Equals(requestsReceipted.Member.OldMemberNo ?? ""))
            {
                return true;
            }

            if (requestsReceipted.ReceiptedTypeId == (int)RequestTypeEnum.Switch)
            {
                switch (requestsReceipted.ReceiptedTypeDetail)
                {
                    case RequestTypeDetail.SwitchCard_PointToPrepaid:
                        string digit34 = $"{oldMemberNo[0]}{oldMemberNo[1]}";
                        if (!int.TryParse(digit34, out int parsedDigit)) return false;
                        else if (parsedDigit >= 9 || parsedDigit < 0) return false;
                        else return true;

                    case RequestTypeDetail.SwitchCard_MagneticFailure:
                        if (!oldMemberNo[0].Equals('2')) return false;
                        else if (card != null && card.Status == CardStatus.Unissued) return false;
                        else return true;

                    case RequestTypeDetail.SwitchCard_ExpirationDateRenewal:
                        if (!oldMemberNo[0].Equals('2')) return false;
                        else if (card != null && card.Status == CardStatus.Unissued) return false;
                        else return true;

                    default:
                        return true;
                }
            }

            return true;
        }
    }
}

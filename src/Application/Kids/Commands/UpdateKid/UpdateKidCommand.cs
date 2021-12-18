using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers.AzureKeyVaults;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Kids.Commands.UpdateKid
{
    public class UpdateKidCommand : IRequest<UpdateKidResultDto>
    {
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public KidRelationshipEnum RelationshipMember { get; set; }
        public string Email { get; set; }
        public int? PICStoreId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public SexType Sex { get; set; }
        public bool IsLivingWithParent { get; set; }
        public string Remark { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }

    public class UpdateKidCommandHandler : IRequestHandler<UpdateKidCommand, UpdateKidResultDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateKidCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateKidResultDto> Handle(UpdateKidCommand request, CancellationToken cancellationToken)
        {
            MemberKid kid = await _context.MemberKids
                .Include(x => x.Member)
                .Include(x => x.Member.PICStore)
                .Include(x => x.Member.RequestsReceipteds)
                .ThenInclude(n => n.Device)
                .Include(x => x.Member.RequestsReceipteds)
                .ThenInclude(n => n.RequestType)
                .Include(x => x.Member.RequestsReceipteds)
                .ThenInclude(n => n.Store)
                .ThenInclude(n => n.Company)
                .FirstOrDefaultAsync(x => x.Id == request.Id);
            if (kid == null)
            {
                throw new NotFoundException(nameof(MemberKid),request.Id);
            }

            if (kid.Member.RequestsReceipteds.Any() 
                && (!kid.Member.RequestsReceipteds.First().Store.IsActive || !kid.Member.RequestsReceipteds.First().Store.Company.IsActive))
            {
                return new UpdateKidResultDto() { IsSuccess = false, MessageCode = "editFailIsNotActive" };
            }

            PICStore picUser = _context.PICStores.FirstOrDefault(n => n.Id == request.PICStoreId && !n.IsDeleted);
            if (picUser == null)
            {
                return new UpdateKidResultDto() { IsSuccess = false, MessageCode = "dataChanged" };
            }

            if ((request.UpdatedAt == null && kid.UpdatedAt != null) || (request.UpdatedAt != null && kid.UpdatedAt != null && !((DateTime)kid.UpdatedAt).ToString("F").Equals(((DateTime)request.UpdatedAt).ToString("F"))))
            {
                return new UpdateKidResultDto() { IsSuccess = false, MessageCode = "editFailDataChange" };
            }

            if (!kid.Member.MemberNo.Equals(request.MemberNo))
            {
                Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);

                if (card != null)
                {
                    if (card.MemberNo.StartsWith("2") && card.Status == CardStatus.Issued)
                        return new UpdateKidResultDto() { IsSuccess = false, MessageCode = "cardInvalid" };
                    if(card.Status == CardStatus.Unissued)
                    {
                        card.Status = CardStatus.Issued;
                        _context.Cards.Update(card);
                    }    
                    
                }

                kid.Member.OldMemberNo = kid.Member.MemberNo;
                kid.Member.MemberNo = request.MemberNo;
            }

            List<MemberKid> memberKids = await _context.MemberKids.Where(n => n.Member.Id == kid.Member.Id).ToListAsync();
            foreach (var kidMember in memberKids)
            {
                kidMember.ParentFirstName = request.ParentFirstName;
                kidMember.ParentLastName = request.ParentLastName;
            }
            _context.MemberKids.UpdateRange(memberKids);

            kid.Member.Email = await request.Email.ToEncryptStringAsync();
            kid.ParentFirstName = request.ParentFirstName;
            kid.ParentLastName = request.ParentLastName;
            kid.RelationshipMember = (int)request.RelationshipMember;
            kid.Member.PICStoreId = request.PICStoreId;
            kid.FirstName = request.FirstName;
            kid.LastName = request.LastName;
            kid.FuriganaFirstName = request.FuriganaFirstName;
            kid.FuriganaLastName = request.FuriganaLastName;
            kid.DateOfBirth = request.DateOfBirth;
            kid.Sex = (int)request.Sex;
            kid.IsLivingWithParent = request.IsLivingWithParent;
            kid.Remark = request.Remark;

            _context.MemberKids.Update(kid);

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdateKidResultDto() { IsSuccess = true, MessageCode = "" };
        }
    }
}

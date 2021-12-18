using System;
using System.Linq;
using MediatR;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Application.Members.Queries.GetMemberInfo;
using mrs.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mrs.Domain.Enums;
using mrs.Application.Common.Helpers.AzureKeyVaults;

namespace mrs.Application.RequestsReceipteds.Commands.UpdateHistory
{
    public class UpdateHistoryCommand : IRequest
    {
        public int Id { get; set; }
        public int? PICStoreId { get; set; }
        public string Remark { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public MemberInfoDto MemberInfo { get; set; }
    }

    public class UpdateHistoryCommandHandler : IRequestHandler<UpdateHistoryCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateHistoryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateHistoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.RequestsReceipteds.Include(x => x.Member)
                .Where(x => !x.IsDeleted && x.Id == request.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new NotFoundException(nameof(RequestsReceipted), request.Id);
            }

            var requestUpdateAt = entity.UpdatedAt.HasValue ? ((DateTime)entity.UpdatedAt).ToString("F") : null;
            var databaseUpdateAt = request.UpdatedAt.HasValue ? ((DateTime)request.UpdatedAt).ToString("F") : null;

            if (requestUpdateAt != databaseUpdateAt)
            {
                throw new DataChangedException("DataChanged");
            }

            if (entity.IsDeleted)
            {
                throw new EntityDeletedException("EntityDeleted");
            }

            entity.Member.PICStoreId = request.PICStoreId;
            entity.Member.Remark = request.Remark;
            entity.Member.FuriganaFirstName = request.MemberInfo.FuriganaFirstName;
            entity.Member.FuriganaLastName = request.MemberInfo.FuriganaLastName;
            entity.Member.FirstName = request.MemberInfo.FirstName;
            entity.Member.LastName = request.MemberInfo.LastName;
            entity.Member.FixedPhone = request.MemberInfo.FixedPhone;
            entity.Member.MobilePhone = request.MemberInfo.MobilePhone;
            entity.Member.DateOfBirth = request.MemberInfo.DateOfBirth;
            entity.Member.Sex = Enum.Parse<SexType>(request.MemberInfo.Sex.ToString());
            entity.Member.IsRegisterAdvertisement = request.MemberInfo.IsRegisterAdvertisement;
            entity.Member.IsNetMember = request.MemberInfo.IsNetMember;
            entity.Member.Province = request.MemberInfo.Province;
            entity.Member.District = request.MemberInfo.City;
            entity.Member.Street = request.MemberInfo.Address;
            entity.Member.BuildingName = request.MemberInfo.BuildingName;
            entity.Member.Email = request.MemberInfo.Email;
            entity.Member.MemberNo = request.MemberInfo.MemberNo;
            entity.Member.OldMemberNo = request.MemberInfo.OldMemberNo;
            entity.Member.ZipcodeId = request.MemberInfo.Zipcode;
            entity.Member.IsUpdateInformation = request.MemberInfo.IsUpdateInformation;

            // Update Card Status
            Card card = _context.Cards.FirstOrDefault(n => n.MemberNo == request.MemberInfo.MemberNo);
            if (card != null && card.Status == CardStatus.Unissued)
            {
                card.Status = CardStatus.Issued;
                _context.Cards.Update(card);
            }

            //encrypt data of columns in member
            await AzureKeyVaultsHelper.EncryptMember(entity.Member);

            _context.RequestsReceipteds.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

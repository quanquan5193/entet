using AutoMapper;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Commands.AddPendingRequestList
{
    public class AddPendingRequestListCommand : IRequest<int>
    {
        public string MemberNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public SexType Sex { get; set; }
        public string ZipcodeId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public bool IsRegisterAdvertisement { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FixedPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string BuildingName { get; set; }
        public DateTime ReceiptedDatetime { get; set; }
        public int? StoreId { get; set; }
        public int DeviceId { get; set; }
        public int? PICStoreId { get; set; }
        public int GMT { get; set; }
    }

    public class RegisterLostCardProfile : Profile
    {
        public RegisterLostCardProfile()
        {
            CreateMap<AddPendingRequestListCommand, Member>();
            CreateMap<AddPendingRequestListCommand, RequestsPending>();
            CreateMap<AddPendingRequestListCommand, RequestsReceipted>();
        }
    }

    public class RegisterLostCardCommandHandler : IRequestHandler<AddPendingRequestListCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public RegisterLostCardCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> Handle(AddPendingRequestListCommand request, CancellationToken cancellationToken)
        {
            Device deviceEntity = _context.Devices.FirstOrDefault(x => x.Id == request.DeviceId);

            if (deviceEntity == null)
            {
                throw new NotFoundException(nameof(Device), request.DeviceId);
            }

            Card cardEntity = _context.Cards.FirstOrDefault(x => x.MemberNo.Equals(request.MemberNo) && x.Status == CardStatus.Unissued && !x.IsDeleted);

            if (cardEntity == null)
            {
                throw new NotFoundException(nameof(Card), request.MemberNo);
            }

            Store storeEntity = _context.Stores.Include(x => x.Company).FirstOrDefault(x => x.Id == request.StoreId);

            if (storeEntity == null)
            {
                throw new NotFoundException(nameof(Store), request.StoreId);
            }
            var storedCode = storeEntity.StoreCode;
            var companyCode = storeEntity.Company?.CompanyCode;
            var randomDigit = new Random().Next(100000, 1000000);
            var requestCode = string.Format("{0}{1}{2}{3}{4}", DateTime.Now.AddHours(request.GMT).ToString("yyMMddHHmmss"), storedCode, companyCode, deviceEntity.DeviceCode, randomDigit);

            cardEntity.Status = CardStatus.Issued;

            request.ReceiptedDatetime = DateTime.Now;

            Member memberEntity = _mapper.Map<AddPendingRequestListCommand, Member>(request);
            memberEntity.IsNetMember = !string.IsNullOrEmpty(memberEntity.Email);
            RequestsPending requestsPendingEntity = _mapper.Map<AddPendingRequestListCommand, RequestsPending>(request);
            requestsPendingEntity.Member = memberEntity;
            requestsPendingEntity.DeviceId = deviceEntity.Id;
            requestsPendingEntity.RequestCode = requestCode;

            //encrypt data of columns in member
            await AzureKeyVaultsHelper.EncryptMember(memberEntity);

            RequestsReceipted requestsReceiptedEntity = _mapper.Map<AddPendingRequestListCommand, RequestsReceipted>(request);
            requestsReceiptedEntity.ReceiptedTypeId = (int)RequestTypeEnum.ReIssued;
            requestsReceiptedEntity.DeviceId = deviceEntity.Id;
            requestsReceiptedEntity.CardId = cardEntity?.Id;
            requestsReceiptedEntity.RequestCode = requestCode;
            requestsReceiptedEntity.StoreId = request.StoreId;
            requestsReceiptedEntity.Member = memberEntity;

            _context.RequestsReceipteds.Add(requestsReceiptedEntity);
            _context.RequestsPendings.Add(requestsPendingEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return memberEntity.Id;
        }
    }
}

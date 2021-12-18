using AutoMapper;
using MediatR;
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

namespace mrs.Application.MigrateCard.Commands.AddMigrateRequest
{
    public class AddMigrateRequestCommand : IRequest<int>
    {
        public string OldMemberNo { get; set; }

        public string MemberNo { get; set; }

        public int StoreId { get; set; }
        
        public int DeviceId { get; set; }
        public int GMT { get; set; }
    }

    public class AddMigrateRequestCommandHandler : IRequestHandler<AddMigrateRequestCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public AddMigrateRequestCommandHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(AddMigrateRequestCommand request, CancellationToken cancellationToken)
        {
            var deviceEntity = _context.Devices.FirstOrDefault(x => x.Id == request.DeviceId);

            if (deviceEntity == null)
            {
                throw new NotFoundException(nameof(Device), request.DeviceId);
            }

            bool oldMemberNoValid = ValidCard(_context, request.OldMemberNo);
            bool memberNoValid = ValidCard(_context, request.MemberNo);

            if (oldMemberNoValid == false || memberNoValid == false)
            {
                throw new NotFoundException(nameof(Card), request.DeviceId);
            }

            Member memberEntity = new Member();
            memberEntity.MemberNo = request.MemberNo;
            memberEntity.OldMemberNo = request.OldMemberNo;

            Card cardEntity = _context.Cards.FirstOrDefault(x => x.MemberNo == memberEntity.MemberNo && !x.IsDeleted);
            Store storeEntity = await _identityService.GetStoreAsync(_currentUserService.UserId);
            string storedCode = storeEntity.StoreCode;
            string companyCode = storeEntity.Company?.CompanyCode;
            int randomDigit = new Random().Next(100000, 1000000);

            //encrypt data of columns in member
            await AzureKeyVaultsHelper.EncryptMember(memberEntity);

            RequestsReceipted requestsReceiptedEntity = new RequestsReceipted();
            requestsReceiptedEntity.ReceiptedDatetime = DateTime.Now;
            requestsReceiptedEntity.ReceiptedTypeId = (int)RequestTypeEnum.PMigrate;
            requestsReceiptedEntity.DeviceId = deviceEntity.Id;
            requestsReceiptedEntity.Member = memberEntity;
            requestsReceiptedEntity.CardId = cardEntity?.Id;
            requestsReceiptedEntity.StoreId = request.StoreId;
            requestsReceiptedEntity.RequestCode = string.Format("{0}{1}{2}{3}{4}", DateTime.Now.AddHours(request.GMT).ToString("yyMMddHHmmss"), storedCode, companyCode, deviceEntity.DeviceCode, randomDigit.ToString());

            _context.RequestsReceipteds.Add(requestsReceiptedEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return memberEntity.Id;
        }

        private bool ValidCard(IApplicationDbContext context, string memberNo)
        {
            Card cardEntity = _context.Cards.FirstOrDefault(x => x.MemberNo == memberNo && !x.IsDeleted);
            if (cardEntity == null)
            {
                return true;
            }
            else if (cardEntity.Status != CardStatus.Unissued)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

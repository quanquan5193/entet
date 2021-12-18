using AutoMapper;
using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using mrs.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Helpers.AzureKeyVaults;

namespace mrs.Application.Members.Commands.UpdateMemberNo
{
    public class UpdateMemberNoCommand : IRequest<int>
    {
        public string OldMemberNo { get; set; }
        public string NewMemberNo { get; set; }
        public int DeviceId { get; set; }
        public bool IsUpdateInformation { get; set; }
        public RequestTypeEnum UpdateType { get; set; }
        public RequestTypeDetail RequestTypeDetail { get; set; }
        public int StoreId { get; set; }
        public int GMT { get; set; }
    }

    public class UpdateMemberNoCommandHandler : IRequestHandler<UpdateMemberNoCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateMemberNoCommandHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(UpdateMemberNoCommand request, CancellationToken cancellationToken)
        {
            Card newCard = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.NewMemberNo) && !x.IsDeleted);
            if (newCard == null) throw new NotFoundException(nameof(Card), request.NewMemberNo);
            newCard.Status = CardStatus.Issued;

            Member member = new Member();
            member.OldMemberNo = request.OldMemberNo;
            member.MemberNo = request.NewMemberNo;
            member.IsUpdateInformation = request.IsUpdateInformation;

            Device device = _context.Devices.FirstOrDefault(x => x.Id == request.DeviceId);
            if (device == null)
            {
                throw new NotFoundException(nameof(Device), request.DeviceId);
            }

            Store storeEntity = await _identityService.GetStoreAsync(_currentUserService.UserId);

            string storedCode = storeEntity.StoreCode;
            string companyCode = storeEntity.Company?.CompanyCode;
            int randomDigit = new Random().Next(100000, 1000000);

            //encrypt data of columns in member
            await AzureKeyVaultsHelper.EncryptMember(member);

            RequestsReceipted requestsReceipted = new RequestsReceipted()
            {
                ReceiptedDatetime = DateTime.Now,
                ReceiptedTypeId = (int)request.UpdateType,
                CardId = newCard?.Id,
                DeviceId = device.Id,
                IsDeleted = false,
                StoreId = request.StoreId,
                Member = member,
                RequestCode = string.Format("{0}{1}{2}{3}{4}", DateTime.Now.AddHours(request.GMT).ToString("yyMMddHHmmss"), storedCode, companyCode, device.DeviceCode, randomDigit.ToString()),
                ReceiptedTypeDetail = request.RequestTypeDetail
            };

            _context.RequestsReceipteds.Add(requestsReceipted);
            await _context.SaveChangesAsync(cancellationToken);

            return member.Id;
        }
    }
}

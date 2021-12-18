using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Helpers.AzureKeyVaults;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Commands.LeaveGroupRequest
{
    public class LeaveGroupRequestCommand : IRequest<int>
    {
        public string MemberNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MobilePhone { get; set; }
        public int StoreId { get; set; }
        public int DeviceId { get; set; }
        public int GMT { get; set; }
    }

    public class LeaveGroupRequestCommandHandler : IRequestHandler<LeaveGroupRequestCommand, int>
    {
        public IApplicationDbContext _context { get; set; }
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public LeaveGroupRequestCommandHandler(IApplicationDbContext context, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(LeaveGroupRequestCommand request, CancellationToken cancellationToken)
        {
            Member member = new Member();
            member.FirstName = request.FirstName;
            member.LastName = request.LastName;
            member.FuriganaFirstName = request.FuriganaFirstName;
            member.FuriganaLastName = request.FuriganaLastName;
            member.DateOfBirth = request.DateOfBirth;
            member.MobilePhone = request.MobilePhone;
            member.FixedPhone = request.MobilePhone;
            member.MemberNo = request.MemberNo;
            member.IsAgreeGetOutMember = true;

            Device device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == request.DeviceId);
            Card card = await _context.Cards.FirstOrDefaultAsync(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);
            Store storeEntity = await _identityService.GetStoreAsync(_currentUserService.UserId);

            string storedCode = storeEntity.StoreCode;
            string companyCode = storeEntity.Company?.CompanyCode;
            int randomDigit = new Random().Next(100000, 1000000);

            //encrypt data of columns in member
            await AzureKeyVaultsHelper.EncryptMember(member);

            RequestsReceipted requestsReceipted = new RequestsReceipted()
            {
                ReceiptedDatetime = DateTime.Now,
                ReceiptedTypeId = (int)RequestTypeEnum.LeaveGroup,
                CardId = card?.Id,
                DeviceId = device.Id,
                IsDeleted = false,
                StoreId = request.StoreId,
                Member = member,
                RequestCode = string.Format("{0}{1}{2}{3}{4}", DateTime.Now.AddHours(request.GMT).ToString("yyMMddHHmmss"), storedCode, companyCode, device.DeviceCode, randomDigit.ToString())
            };

            _context.RequestsReceipteds.Add(requestsReceipted);
            await _context.SaveChangesAsync(cancellationToken);

            return member.Id;
        }
    }
}

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
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Commands.RegisterKidsClub
{
    public class RegisterKidsClubCommand : IRequest<int>
    {
        public string MemberNo { get; set; }
        public string Email { get; set; }
        public int StoreId { get; set; }
        public int DeviceId { get; set; }
        public List<RegisterKidDto> MemberKids { get; set; }
        public int GMT { get; set; }
    }

    public class RegisterKidDto
    {
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string ParentFuriganaFirstName { get; set; }
        public string ParentFuriganaLastName { get; set; }
        public int RelationshipMember { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public SexType Sex { get; set; }
        public bool IsLivingWithParent { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class RegisterMemberProfile : Profile
    {
        public RegisterMemberProfile()
        {
            CreateMap<RegisterKidDto, MemberKid>();
        }
    }

    public class RegisterKidsClubCommandHandler : IRequestHandler<RegisterKidsClubCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public RegisterKidsClubCommandHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(RegisterKidsClubCommand request, CancellationToken cancellationToken)
        {

            List<MemberKid> memberKids = _mapper.Map<List<RegisterKidDto>, List<MemberKid>>(request.MemberKids);

            Member member = new();
            member.Email = request.Email;
            member.IsNetMember = !string.IsNullOrEmpty(request.Email);
            member.MemberNo = request.MemberNo;
            member.FirstName = memberKids.FirstOrDefault()?.ParentFirstName;
            member.LastName = memberKids.FirstOrDefault()?.ParentLastName;
            member.FuriganaFirstName = memberKids.FirstOrDefault()?.ParentFuriganaFirstName;
            member.FuriganaLastName = memberKids.FirstOrDefault()?.ParentFuriganaLastName;
            member.IsRegisterKidClub = true;

            foreach (var kid in memberKids)
            {
                member.MemberKids.Add(kid);
            }

            Device device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == request.DeviceId);

            if (device == null)
            {
                throw new NotFoundException(nameof(Device), request.DeviceId);
            }

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
                ReceiptedTypeId = (int)RequestTypeEnum.Kid,
                DeviceId = device.Id,
                CardId = card?.Id,
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

using AutoMapper;
using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using mrs.Application.Members.Commands.UpdateMemberNo;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers.AzureKeyVaults;

namespace mrs.Application.Members.Commands.UpdateMember
{
    public class UpdateMemberCommand : IRequest<int>
    {
        public UpdateMemberNoCommand MemberObject { get; set; }
        public MemberCommand Member { get; set; }
        public RequestTypeEnum UpdateType { get; set; }
        public RequestTypeDetail RequestTypeDetail { get; set; }
        public int StoreId { get; set; }
        public int GMT { get; set; }
    }

    public class MemberCommand
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
        public string BuildingName { get; set; }
        public bool IsRegisterAdvertisement { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FixedPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public int DeviceId { get; set; }
        public bool IsUpdateInformation { get; set; }
        public IList<MemberKidDto> MemberKids { get; set; }
    }

    public class MemberKidDto
    {
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string ParentFuriganaFirstName { get; set; }
        public string ParentFuriganaLastName { get; set; }
        public string RelaionshipMember { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public SexType Sex { get; set; }
        public bool IsLivingWithParent { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class UpdateMemberProfile : Profile
    {
        public UpdateMemberProfile()
        {
            CreateMap<MemberCommand, Member>();
            CreateMap<MemberKidDto, MemberKid>();
        }
    }

    public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateMemberCommandHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
        {
            Member member = _mapper.Map<Member>(request.Member);
            member.IsRegisterKidClub = member.MemberKids.Count > 0;
            member.IsNetMember = !string.IsNullOrEmpty(request.Member.Email);
            member.IsUpdateInformation = request.MemberObject.IsUpdateInformation;

            if (!string.IsNullOrWhiteSpace(request.MemberObject.NewMemberNo))
            {
                member.OldMemberNo = request.MemberObject.OldMemberNo;
                member.MemberNo = request.MemberObject.NewMemberNo;

                Card newCard = _context.Cards.FirstOrDefault(x => x.MemberNo.Equals(request.MemberObject.NewMemberNo));
                newCard.Status = CardStatus.Issued;

                _context.Cards.Update(newCard);
            }

            Device device = _context.Devices.FirstOrDefault(x => x.Id == request.Member.DeviceId);
            if (device == null)
            {
                throw new NotFoundException(nameof(Device), request.Member.DeviceId);
            }
            string memberNoInCard = request.MemberObject.NewMemberNo;
            if (request.MemberObject.IsUpdateInformation)
            {
                memberNoInCard = request.Member.MemberNo;
            }
            Card card = _context.Cards.FirstOrDefault(x => x.MemberNo.Equals(memberNoInCard) && !x.IsDeleted);
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
                DeviceId = device.Id,
                CardId = card?.Id,
                IsDeleted = false,
                StoreId = request.StoreId,
                Member = member,
                RequestCode = string.Format("{0}{1}{2}{3}{4}", DateTime.Now.AddHours(request.GMT).ToString("yyMMddHHmmss"), storedCode, companyCode, device.DeviceCode, randomDigit.ToString()),
                ReceiptedTypeDetail = request.RequestTypeDetail == 0 ? request.MemberObject.RequestTypeDetail : request.RequestTypeDetail
            };

            _context.RequestsReceipteds.Add(requestsReceipted);
            await _context.SaveChangesAsync(cancellationToken);
            return member.Id;
        }
    }
}

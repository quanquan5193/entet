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
using mrs.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Helpers.AzureKeyVaults;

namespace mrs.Application.Members.Commands.RegisterMember
{
    public class RegisterMemberCommand : IRequest<int>
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
        public string Remark { get; set; }
        public string BuildingName { get; set; }
        public IList<MemberKidDto> MemberKids { get; set; }
        public int StoreId { get; set; }
        public int DeviceId { get; set; }
        public int GMT { get; set; }
    }

    public class MemberKidDto
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
            CreateMap<RegisterMemberCommand, Member>();
            CreateMap<MemberKidDto, MemberKid>();
        }
    }

    public class RegisterMemberCommandHandler : IRequestHandler<RegisterMemberCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public RegisterMemberCommandHandler(IApplicationDbContext context, IIdentityService identityService, ICurrentUserService currentUserService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(RegisterMemberCommand request, CancellationToken cancellationToken)
        {
            Member entity = _mapper.Map<RegisterMemberCommand, Member>(request);
            entity.IsRegisterKidClub = entity.MemberKids.Count > 0;
            entity.IsNetMember = !string.IsNullOrEmpty(request.Email);

            Card card = _context.Cards.FirstOrDefault(x => x.MemberNo.Equals(request.MemberNo) && !x.IsDeleted);
            Device device = _context.Devices.FirstOrDefault(x => x.Id == request.DeviceId);
            Store storeEntity = _context.Stores.Include(x => x.Company).FirstOrDefault(x => x.Id == request.StoreId);

            if (device == null)
            {
                throw new NotFoundException(nameof(Device), request.DeviceId);
            }

            if (storeEntity == null)
            {
                throw new NotFoundException(nameof(Store), request.StoreId);
            }

            string storedCode = storeEntity.StoreCode;
            string companyCode = storeEntity.Company?.CompanyCode;
            int randomDigit = new Random().Next(100000, 1000000);

            //encrypt data of columns in member
            await AzureKeyVaultsHelper.EncryptMember(entity);

            RequestsReceipted requestsReceipted = new RequestsReceipted()
            {
                ReceiptedDatetime = DateTime.Now,
                ReceiptedTypeId = (int)RequestTypeEnum.New,
                CardId = card?.Id,
                DeviceId = device.Id,
                IsDeleted = false,
                StoreId = request.StoreId,
                RequestCode = string.Format("{0}{1}{2}{3}{4}", DateTime.Now.AddHours(request.GMT).ToString("yyMMddHHmmss"), storedCode, companyCode, device.DeviceCode, randomDigit.ToString()),
                Member = entity
            };

            card.Status = CardStatus.Issued;
            _context.Cards.Update(card);
            _context.RequestsReceipteds.Add(requestsReceipted);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}

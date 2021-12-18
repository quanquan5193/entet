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

namespace mrs.Application.MissingCard.Commands.GetPendingRequestDetail
{
    public class GetPendingRequestDetailCommand : IRequest<PendingRequestDetailDto>
    {
        public int Id { get; set; }
    }

    public class GetPendingCardDetailProfile : Profile
    {
        public GetPendingCardDetailProfile()
        {
            CreateMap<RequestsPending, PendingRequestDetailDto>()
                .ForMember(x => x.MemberNo, y => y.MapFrom(c => c.Member.MemberNo))
                .ForMember(x => x.FirstName, y => y.MapFrom(c => c.Member.FirstName))
                .ForMember(x => x.LastName, y => y.MapFrom(c => c.Member.LastName))
                .ForMember(x => x.FuriganaFirstName, y => y.MapFrom(c => c.Member.FuriganaFirstName))
                .ForMember(x => x.FuriganaLastName, y => y.MapFrom(c => c.Member.FuriganaLastName))
                .ForMember(x => x.ZipcodeId, y => y.MapFrom(c => c.Member.ZipcodeId))
                .ForMember(x => x.Province, y => y.MapFrom(c => c.Member.Province))
                .ForMember(x => x.District, y => y.MapFrom(c => c.Member.District))
                .ForMember(x => x.Street, y => y.MapFrom(c => c.Member.Street))
                .ForMember(x => x.IsRegisterAdvertisement, y => y.MapFrom(c => c.Member.IsRegisterAdvertisement))
                .ForMember(x => x.DateOfBirth, y => y.MapFrom(c => c.Member.DateOfBirth))
                .ForMember(x => x.FixedPhone, y => y.MapFrom(c => c.Member.FixedPhone))
                .ForMember(x => x.MobilePhone, y => y.MapFrom(c => c.Member.MobilePhone))
                .ForMember(x => x.Email, y => y.MapFrom(c => c.Member.Email))
                .ForMember(x => x.BuildingName, y => y.MapFrom(c => c.Member.BuildingName))
                .ForMember(x => x.ReceiptedDatetime, y => y.MapFrom(c => c.ReceiptedDatetime))
                .ForMember(x => x.StoreId, y => y.MapFrom(c => c.StoreId))
                .ForMember(x => x.DeviceCode, y => y.MapFrom(c => c.Device.DeviceCode))
                .ForMember(x => x.PICStoreId, y => y.MapFrom(c => c.Member.PICStoreId))
                .ForMember(x=>x.Sex, y=>y.MapFrom(c=>c.Member.Sex))
                ;
        }
    }

    public class GetPendingCardDetailCommandHandler : IRequestHandler<GetPendingRequestDetailCommand, PendingRequestDetailDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetPendingCardDetailCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PendingRequestDetailDto> Handle(GetPendingRequestDetailCommand request, CancellationToken cancellationToken)
        {
            var requestPending = _context.RequestsPendings.Include(x => x.Member).Include(x => x.Device).FirstOrDefault(x => x.Id == request.Id && x.IsDeleted == false);

            if (requestPending == null)
            {
                throw new NotFoundException(nameof(RequestsPending), request.Id);
            }

            var result = _mapper.Map<RequestsPending, PendingRequestDetailDto>(requestPending);

            result.FirstName = await result.FirstName.ToDecryptStringAsync();
            result.LastName = await result.LastName.ToDecryptStringAsync();
            result.FuriganaFirstName = await result.FuriganaFirstName.ToDecryptStringAsync();
            result.FuriganaLastName = await result.FuriganaLastName.ToDecryptStringAsync();
            result.Email = await result.Email.ToDecryptStringAsync();
            result.Province = await result.Province.ToDecryptStringAsync();
            result.District = await result.District.ToDecryptStringAsync();
            result.Street = await result.Street.ToDecryptStringAsync();
            result.BuildingName = await result.BuildingName.ToDecryptStringAsync();
            result.FixedPhone = await result.FixedPhone.ToDecryptStringAsync();
            result.MobilePhone = await result.MobilePhone.ToDecryptStringAsync();

            return result;
        }
    }
}

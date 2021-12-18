using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.GetDevice
{
    public class GetDeviceCommand : IRequest<GetDeviceDto>
    {
        public int Id { get; set; }
    }

    public class GetDeviceCommandProfile : Profile
    {
        public GetDeviceCommandProfile()
        {
            CreateMap<Device, GetDeviceDto>()
                .ForMember(x => x.CompanyId, y => y.MapFrom(c => c.Store.CompanyId))
                ;
        }
    }

    public class GetDeviceCommandHandler : IRequestHandler<GetDeviceCommand, GetDeviceDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;

        public GetDeviceCommandHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<GetDeviceDto> Handle(GetDeviceCommand request, CancellationToken cancellationToken)
        {
            var deviceEntity = _context.Devices.Include(x => x.Store.Company).FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);

            if (deviceEntity == null)
            {
                throw new NotFoundException();
            }

            var deviceReturn = _mapper.Map<Device, GetDeviceDto>(deviceEntity);
            var userCreate = _identityService.GetUserDto(deviceEntity.CreatedBy);
            var userUpdate = _identityService.GetUserDto(deviceEntity.UpdatedBy);
            deviceReturn.CreatedByUserName = userCreate.UserName;
            deviceReturn.CreatedByFullName = userCreate.FullName;
            deviceReturn.UpdatedByUserName = userUpdate?.UserName;
            deviceReturn.UpdatedByFullName = userUpdate?.FullName;

            return deviceReturn;
        }
    }
}

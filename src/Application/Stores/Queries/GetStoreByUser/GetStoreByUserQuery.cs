using AutoMapper;
using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Stores.Queries.GetStoreByUser
{
    public class GetStoreByUserQuery : IRequest<StoreByUserDto>
    {
        public string UserId { get; set; } = null;
    }

    public class StoreByUserDtoProfile : Profile
    {
        public StoreByUserDtoProfile()
        {
            CreateMap<Store, StoreByUserDto>();
        }
    }

    public class GetStoreByUserQueryHandler : IRequestHandler<GetStoreByUserQuery, StoreByUserDto>
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetStoreByUserQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService,IMapper mapper)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<StoreByUserDto> Handle(GetStoreByUserQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == null)
            {
                request.UserId = _currentUserService.UserId;
            }

            Store store = await _identityService.GetStoreAsync(request.UserId);

            return _mapper.Map<Store, StoreByUserDto>(store);
        }
    }
}

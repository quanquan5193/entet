using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.PersonIncharge
{
    public class GetListPersonInchargeQuery : IRequest<List<PersonInchargeDto>>
    {
        public int StoreId { get; set; }
    }

    public class PersonInchargeProfile : Profile
    {
        public PersonInchargeProfile()
        {
            CreateMap<PICStore, PersonInchargeDto>();
        }
    }

    public class GetListPersonInchargeQueryHandler : IRequestHandler<GetListPersonInchargeQuery, List<PersonInchargeDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        public GetListPersonInchargeQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }
        public async Task<List<PersonInchargeDto>> Handle(GetListPersonInchargeQuery request, CancellationToken cancellationToken)
        {
            var listPersonIncharge = await _context.PICStores.Where(x => x.CreatedBy == _currentUserService.UserId && !x.IsDeleted).OrderBy(c => c.PICName).ProjectToListAsync<PersonInchargeDto>(_mapper.ConfigurationProvider);
            return listPersonIncharge;
        }
    }
}

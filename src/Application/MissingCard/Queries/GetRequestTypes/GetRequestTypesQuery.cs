using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Queries.GetRequestTypes
{
    public class GetRequestTypesQuery : IRequest<List<RequestTypeDto>>
    {

    }

    public class GetClassifyRequestProfile : Profile
    {
        public GetClassifyRequestProfile()
        {
            CreateMap<RequestType, RequestTypeDto>();
        }
    }

    public class GetClassifyRequestQueryHandler : IRequestHandler<GetRequestTypesQuery, List<RequestTypeDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetClassifyRequestQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RequestTypeDto>> Handle(GetRequestTypesQuery request, CancellationToken cancellationToken)
        {
            return await _context.RequestTypes.ProjectTo<RequestTypeDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        }
    }
}

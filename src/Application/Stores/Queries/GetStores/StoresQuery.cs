using AutoMapper;
using AutoMapper.QueryableExtensions;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Security;
using mrs.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Stores.Queries.GetStores
{
    public class StoresQuery : IRequest<StoresVm>
    {
    }

    public class GetStoresQueryHandler : IRequestHandler<StoresQuery, StoresVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetStoresQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<StoresVm> Handle(StoresQuery request, CancellationToken cancellationToken)
        {
            return new StoresVm
            {
                Lists = await _context.Stores
                    .AsNoTracking()
                    .ProjectTo<FlatStoreDto>(_mapper.ConfigurationProvider)
                    .Where(a => !a.IsDeleted && a.IsActive)
                    .OrderByDescending(t => t.Id)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}

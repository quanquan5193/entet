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

namespace mrs.Application.Cards.Queries.GetCards
{
    public class GetCardsQuery : IRequest<CardsVm>
    {
    }

    public class GetCardsQueryHandler : IRequestHandler<GetCardsQuery, CardsVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCardsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CardsVm> Handle(GetCardsQuery request, CancellationToken cancellationToken)
        {
            return new CardsVm
            {
                //PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                //    .Cast<PriorityLevel>()
                //    .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                //    .ToList(),

                //Lists = await _context.TodoLists
                //    .AsNoTracking()
                //    .ProjectTo<CardDto>(_mapper.ConfigurationProvider)
                //    .OrderBy(t => t.UpdatedAt)
                //    .ToListAsync(cancellationToken)
            };
        }
    }
}

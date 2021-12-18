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

namespace mrs.Application.Companies.Queries.GetCompanies
{
    public class CompaniesQuery : IRequest<CompaniesVm>
    {
    }

    public class GetCompaniesQueryHandler : IRequestHandler<CompaniesQuery, CompaniesVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompaniesQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CompaniesVm> Handle(CompaniesQuery request, CancellationToken cancellationToken)
        {
            return new CompaniesVm
            {
                Lists = await _context.Companies
                    .AsNoTracking()
                    .ProjectTo<FlatCompanyDto>(_mapper.ConfigurationProvider)
                    .Where(a => !a.IsDeleted && a.IsActive)
                    .OrderByDescending(t => t.Id)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}

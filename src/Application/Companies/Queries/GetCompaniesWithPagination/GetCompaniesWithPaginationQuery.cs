using AutoMapper;
using AutoMapper.QueryableExtensions;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mrs.Application.Companies.Queries.GetCompanies;
using mrs.Application.Common.ExpressionExtension;
using mrs.Domain.Entities;

namespace mrs.Application.Companies.Queries.GetCompaniesWithPagination
{
    public class GetCompaniesWithPaginationQuery : IRequest<PaginatedList<CompanyDto>>
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string OrderBy { get; set; } = "Order";
        public string OrderType { get; set; } = "desc";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetCompaniesWithPaginationQueryHandler : IRequestHandler<GetCompaniesWithPaginationQuery, PaginatedList<CompanyDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCompaniesWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<CompanyDto>> Handle(GetCompaniesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Companies.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.CompanyCode))
            {
                query = query.Where(x => x.CompanyCode.Contains(request.CompanyCode));
            }
            if (!string.IsNullOrEmpty(request.CompanyName))
            {
                query = query.Where(x => x.CompanyName.Contains(request.CompanyName));
            }

            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = nameof(Company.Id);
            }
            if (string.IsNullOrEmpty(request.OrderType))
            {
                request.OrderType = "desc";
            }

            return await query.OrderByCustom(request.OrderBy + " " + request.OrderType.ToUpper())
                .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}

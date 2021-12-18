using AutoMapper;
using MediatR;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Companies.Commands.GetDetailCompany
{
    public class GetDetailCompanyCommand : IRequest<GetDetailCompanyDto>
    {
        public int Id { get; set; }
    }

    public class GetDetailCompanyCommandHandler : IRequestHandler<GetDetailCompanyCommand, GetDetailCompanyDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetDetailCompanyCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetDetailCompanyDto> Handle(GetDetailCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = _context.Companies.FirstOrDefault(x => !x.IsDeleted && x.Id == request.Id);
            if (company == null)
            {
                throw new NotFoundException(nameof(Company), request.Id);
            }

            var companyDto = _mapper.Map<GetDetailCompanyDto>(company);

            return companyDto;
        }
    }
}

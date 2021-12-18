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

namespace mrs.Application.Companies.Commands.GetDetailCompanyWithCode
{
    public class GetDetailCompanyWithCodeCommand : IRequest<GetDetailCompanyWithCodeDto>
    {
        public string CompanyCode { get; set; }
    }

    public class GetDetailCompanyWithCodeCommandProfile : Profile
    {
        public GetDetailCompanyWithCodeCommandProfile()
        {
            CreateMap<Company, GetDetailCompanyWithCodeDto>();
        }
    }

    public class GetDetailCompanyWithCodeCommandHandler : IRequestHandler<GetDetailCompanyWithCodeCommand, GetDetailCompanyWithCodeDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetDetailCompanyWithCodeCommandHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<GetDetailCompanyWithCodeDto> Handle(GetDetailCompanyWithCodeCommand request, CancellationToken cancellationToken)
        {
            var company = _context.Companies.FirstOrDefault(x => !x.IsDeleted && x.CompanyCode.Equals(request.CompanyCode));
            if (company == null)
            {
                throw new NotFoundException(nameof(Company), request.CompanyCode);
            }

            var companyDto = _mapper.Map<GetDetailCompanyWithCodeDto>(company);

            return companyDto;
        }
    }
}

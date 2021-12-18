using AutoMapper;
using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Members.Queries.PersonIncharge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.PersonInchargeCMS
{
    public class GetListPersonInchargeCMSQuery : IRequest<List<PersonInchargeDto>>
    {
        public string CreatedBy { get; set; }
    }
    

    public class GetListPersonInchargeCMSQueryHandler : IRequestHandler<GetListPersonInchargeCMSQuery, List<PersonInchargeDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetListPersonInchargeCMSQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<PersonInchargeDto>> Handle(GetListPersonInchargeCMSQuery request, CancellationToken cancellationToken)
        {
            List<PersonInchargeDto> listPersonIncharge = await _context.PICStores.Where(x => x.CreatedBy == request.CreatedBy && !x.IsDeleted).OrderBy(c => c.PICName).ProjectToListAsync<PersonInchargeDto>(_mapper.ConfigurationProvider);
            return listPersonIncharge;
        }
    }
}

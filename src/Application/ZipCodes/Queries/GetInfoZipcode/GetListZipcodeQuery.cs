using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ZipCodes.Queries.GetInfoZipcode
{
    public class GetListZipcodeQuery : IRequest<ZipcodeDto>
    {
        public string Zipcode { get; set; }
    }

    public class ZipcodeProfile : Profile
    {
        public ZipcodeProfile()
        {
            CreateMap<ZipCode, ZipcodeDto>();
        }
    }

    public class GetListZipcode : IRequestHandler<GetListZipcodeQuery, ZipcodeDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetListZipcode(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ZipcodeDto> Handle(GetListZipcodeQuery request, CancellationToken cancellationToken)
        {
            var zipcodeEntity = await _context.ZipCodes.FirstOrDefaultAsync(x => x.Zipcode == request.Zipcode);
            var zipcodeDto = zipcodeEntity == null ? null : _mapper.Map<ZipcodeDto>(zipcodeEntity);
            return zipcodeDto;
        }
    }
}

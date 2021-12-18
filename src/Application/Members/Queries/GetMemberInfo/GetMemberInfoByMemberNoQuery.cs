using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Interfaces;
using mrs.Application.ZipCodes.Queries;
using mrs.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.GetMemberInfo
{
    public class GetMemberInfoByMemberNoQuery : IRequest<MemberInfoDto>
    {
        public string MemberNo { get; set; }
    }

    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<Member, MemberInfoDto>();
        }
    }

    public class GetMemberInfoByMemberNoQueryHandler : IRequestHandler<GetMemberInfoByMemberNoQuery, MemberInfoDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetMemberInfoByMemberNoQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<MemberInfoDto> Handle(GetMemberInfoByMemberNoQuery request, CancellationToken cancellationToken)
        {
            var memberEntity = await _context.Members.FirstOrDefaultAsync(x => x.MemberNo == request.MemberNo);
            var memberDto = memberEntity == null ? null : _mapper.Map<MemberInfoDto>(memberEntity);
            return memberDto;
        }
    }
}

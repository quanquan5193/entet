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

namespace mrs.Application.Cards.Queries.CheckBarcodeExist
{
    public class CheckBarcodeExistQuery : IRequest<bool>
    {
        public string MemberNo { get; set; }
    }

    public class GetCardsQueryHandler : IRequestHandler<CheckBarcodeExistQuery, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCardsQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> Handle(CheckBarcodeExistQuery request, CancellationToken cancellationToken)
        {
            var existedCard = await _context.Cards.FirstOrDefaultAsync(n => !n.IsDeleted && n.MemberNo.Equals(request.MemberNo));
            return existedCard != null;
        }
    }
}

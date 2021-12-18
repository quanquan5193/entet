using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Cards.Queries.GetCard
{
    public class GetCardQuery : IRequest<CardDto>
    {
        [Required]
        public int Id { get; set; }
    }

    public class GetCardQueryHandler : IRequestHandler<GetCardQuery, CardDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetCardQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<CardDto> Handle(GetCardQuery request, CancellationToken cancellationToken)
        {
            // if logged in user has Company manager role, limit data in his company
            var companyId = 0;
            if (_currentUserService.RoleLevel == RoleLevel.Level_6)
            {
                var store = await _identityService.GetStoreAsync(_currentUserService.UserId);
                if (store?.Company != null)
                {
                    companyId = store.Company.Id;
                }
            }

            var cardItem = await _context.Cards.AsNoTracking()
                    .ProjectTo<CardDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(a => a.Id == request.Id);
            if (cardItem == null || (_currentUserService.RoleLevel == RoleLevel.Level_6 && cardItem.CompanyId.HasValue && cardItem.CompanyId.Value != companyId))
            {
                throw new NotFoundException(typeof(Card).Name, request.Id);
            }
            cardItem.Createder = _identityService.GetUserDto(cardItem.CreatedBy);
            cardItem.Updateder = _identityService.GetUserDto(cardItem.UpdatedBy);
            return cardItem;
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Helpers.AzureKeyVaults;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Kids.Queries.GetKid
{
    public class GetKidQuery : IRequest<KidDetailDto>
    {
        public int Id { get; set; }
    }

    public class KidDetailDtoProfile : Profile
    {
        private const string LIVE_WITH_PARENT_TEXT = "同居";
        private const string UN_LIVE_WITH_PARENT_TEXT = "別居";

        public KidDetailDtoProfile()
        {
            CreateMap<MemberKid, KidDetailDto>()
                .ForMember(x => x.MemberNo, y => y.MapFrom(z => z.Member.MemberNo))
                .ForMember(x => x.Remark, y => y.MapFrom(z => z.Remark))
                .ForMember(x => x.RegisterKidClubDate, y => y.MapFrom(z => z.CreatedAt))
                .ForMember(x => x.SexTypeText, y => y.MapFrom(z => ((SexType)z.Sex).GetStringValue()))
                .ForMember(x => x.RelationshipMemberText, y => y.MapFrom(z => ((KidRelationshipEnum)z.RelationshipMember).GetStringValue()))
                .ForMember(x => x.IsLivingWithParentText, y => y.MapFrom(z => z.IsLivingWithParent ? LIVE_WITH_PARENT_TEXT : UN_LIVE_WITH_PARENT_TEXT))
                .ForMember(x => x.PICStoreId, y => y.MapFrom(z => z.Member.PICStoreId))
                .ForMember(x => x.PICStoreName, y => y.MapFrom(z => z.Member.PICStore.PICName))
                .ForMember(x => x.CompanyId, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.Company.Id))
                .ForMember(x => x.CompanyCode, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyCode))
                .ForMember(x => x.CompanyName, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyName))
                .ForMember(x => x.StoreId, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.Id))
                .ForMember(x => x.StoreCode, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.StoreCode))
                .ForMember(x => x.StoreName, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.StoreName))
                .ForMember(x => x.Email, y => y.MapFrom(z => z.Member.Email))
                .ForMember(x => x.CreatedBy, y=> y.MapFrom(z=>z.CreatedBy))
                .ForMember(x => x.DeviceCode, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault(w => w.ReceiptedTypeId == (int)RequestTypeEnum.Kid || w.ReceiptedTypeId == (int)RequestTypeEnum.New).Device.DeviceCode))
                .ForMember(x => x.RequestType, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault(w => w.ReceiptedTypeId == (int)RequestTypeEnum.Kid || w.ReceiptedTypeId == (int)RequestTypeEnum.New).ReceiptedTypeId))
                .ForMember(x => x.RequestTypeText, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault(w => w.ReceiptedTypeId == (int)RequestTypeEnum.Kid || w.ReceiptedTypeId == (int)RequestTypeEnum.New).RequestType.RequestTypeName))
                .ForMember(x => x.IsEnableEdit, y => y.MapFrom(z => z.Member.RequestsReceipteds.FirstOrDefault().Store.IsActive && z.Member.RequestsReceipteds.FirstOrDefault().Store.Company.IsActive));
        }
    }

    public class GetKidQueryHandler : IRequestHandler<GetKidQuery, KidDetailDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetKidQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<KidDetailDto> Handle(GetKidQuery request, CancellationToken cancellationToken)
        {
            IQueryable<MemberKid> query = _context.MemberKids
                .Include(x => x.Member)
                .Include(x => x.Member.PICStore)
                .Include(x => x.Member.RequestsReceipteds)
                .ThenInclude(n => n.Device)
                .Include(x => x.Member.RequestsReceipteds)
                .ThenInclude(n => n.RequestType)
                .Include(x => x.Member.RequestsReceipteds)
                .ThenInclude(n => n.Store)
                .ThenInclude(n => n.Company);

            query = query.Where(x => x.Id == request.Id);

            #region check role
            IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);

            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.Member.RequestsReceipteds.Any(x => x.Device.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId)));
            }
            #endregion
            MemberKid kid = await query.FirstOrDefaultAsync();
            if(kid == null)
            {
                throw new NotFoundException(nameof(MemberKid), request.Id);
            }

            kid.Member.Email = await kid.Member.Email.ToDecryptStringAsync();



            return _mapper.Map<MemberKid, KidDetailDto>(kid);
        }
    }
}

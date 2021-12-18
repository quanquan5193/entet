using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.ExpressionExtension;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Kids.Queries.GetKidsPaging
{
    public class GetKidPagingQuery : IRequest<PaginatedList<KidDto>>
    {
        public string MemberNo { get; set; }
        public string KidName { get; set; }
        public string CompanyCode { get; set; }
        public string StoreCode { get; set; }
        public string DeviceNumber { get; set; }
        public DateTime? RegisterDateFrom { get; set; }
        public DateTime? RegisterDateTo { get; set; }
        public string PICStoreName { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string SortBy { get; set; }
        public string SortDimension { get; set; }
    }

    public class KidDtoProfile : Profile
    {
        private const string JAPANESE_SPACE = "　";
        public KidDtoProfile()
        {
            CreateMap<MemberKid, KidDto>()
                .ForMember(x => x.MemberNo, y => y.MapFrom(z => z.Member.MemberNo))
                .ForMember(x => x.PICStoreId, y => y.MapFrom(z => z.Member.PICStoreId))
                .ForMember(x => x.PICStoreName, y => y.MapFrom(z => z.Member.PICStore.PICName))
                .ForMember(x => x.ParentName, y => y.MapFrom(z => z.ParentLastName + JAPANESE_SPACE + z.ParentFirstName))
                .ForMember(x => x.ParentFuriganaName, y => y.MapFrom(z => z.ParentFuriganaLastName + JAPANESE_SPACE + z.ParentFuriganaFirstName))
                .ForMember(x => x.FuriganaName, y => y.MapFrom(z => z.FuriganaLastName + JAPANESE_SPACE + z.FuriganaFirstName))
                .ForMember(x => x.FullName, y => y.MapFrom(z => z.LastName + JAPANESE_SPACE + z.FirstName))
                .ForMember(x => x.CreatedAt, y => y.MapFrom(z => z.CreatedAt));
        }
    }

    public class GetKidPagingQueryHandler : IRequestHandler<GetKidPagingQuery, PaginatedList<KidDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private const string JAPANESE_SPACE = "　";

        public GetKidPagingQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<PaginatedList<KidDto>> Handle(GetKidPagingQuery request, CancellationToken cancellationToken)
        {
            IQueryable<MemberKid> query = _context.MemberKids.Include(n => n.Member).ThenInclude(n => n.PICStore).AsQueryable();

            #region check role
            IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);

            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.Member.RequestsReceipteds.Any(x => x.Device.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId)));
            }
            #endregion

            query = query.Where(x => x.Member.RequestsReceipteds.Any(y => y.ReceiptedTypeId == (int)RequestTypeEnum.Kid || y.ReceiptedTypeId == (int)RequestTypeEnum.New));

            if (!string.IsNullOrWhiteSpace(request.MemberNo))
                query = query.Where(x => x.Member.MemberNo.Contains(request.MemberNo));

            if (!string.IsNullOrWhiteSpace(request.KidName))
                query = query.Where(x =>
                (x.LastName + JAPANESE_SPACE + x.FirstName).Contains(request.KidName)
                || (x.FuriganaLastName + JAPANESE_SPACE + x.FuriganaFirstName).Contains(request.KidName)
                || (x.LastName + " " + x.FirstName).Contains(request.KidName)
                || (x.FuriganaLastName + " " + x.FuriganaFirstName).Contains(request.KidName));

            if (!string.IsNullOrWhiteSpace(request.CompanyCode))
                query = query.Where(x => x.Member.RequestsReceipteds.FirstOrDefault().Store.Company.CompanyCode.Contains(request.CompanyCode));

            if (!string.IsNullOrWhiteSpace(request.StoreCode))
                query = query.Where(x => x.Member.RequestsReceipteds.FirstOrDefault().Store.StoreCode.Contains(request.StoreCode));

            if (!string.IsNullOrWhiteSpace(request.DeviceNumber))
                query = query.Where(x => x.Member.RequestsReceipteds.Any(x => x.Device.DeviceCode.Contains(request.DeviceNumber)));

            if (!string.IsNullOrWhiteSpace(request.PICStoreName))
                query = query.Where(x => x.Member.PICStore.PICName.Contains(request.PICStoreName));

            if (request.RegisterDateFrom.HasValue)
            {
                DateTime dateFromSearch = ((DateTime)request.RegisterDateFrom);
                query = query.Where(x => x.CreatedAt >= dateFromSearch);
            }

            if (request.RegisterDateTo.HasValue)
            {
                DateTime dateToSearch = ((DateTime)request.RegisterDateTo).AddDays(1);
                query = query.Where(x => x.CreatedAt < dateToSearch);
            }

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {

                if (request.SortBy.Equals("MemberNo"))
                {
                    query = request.SortDimension.Equals("asc") ? query.OrderBy(n => n.Member.MemberNo) : query.OrderByDescending(n => n.Member.MemberNo);
                } 
                else if (request.SortBy.Equals("PicStoreName"))
                {
                    query = request.SortDimension.Equals("asc") ? query.OrderBy(n => n.Member.PICStore.PICName) : query.OrderByDescending(n => n.Member.PICStore.PICName);
                } 
                else
                {
                    query = query.OrderByCustom(request.SortBy + " " + request.SortDimension.ToUpper());
                }
            }
            else
            {
                query = query.OrderByDescending(x => x.Id);
            }

            var result = await query
                .ProjectTo<KidDto>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.Page, request.Size);

            return result;
        }
    }
}

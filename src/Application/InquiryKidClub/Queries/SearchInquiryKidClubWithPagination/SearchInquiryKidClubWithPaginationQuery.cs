using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.ExpressionExtension;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.InquiryKidClub.Queries.SearchInquiryKidClubWithPagination
{
    public class SearchInquiryKidClubWithPaginationQuery : IRequest<PaginatedList<InquiryKidClubDto>>
    {
        public string MemberNo { get; set; }

        public string KidName { get; set; }

        public int? PICStore { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string OrderBy { get; set; } = "Id";

        public string OrderType { get; set; } = "desc";

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int DeviceId { get; set; }
    }

    public class SearchInquiryKidClubWithPaginationHandler : IRequestHandler<SearchInquiryKidClubWithPaginationQuery, PaginatedList<InquiryKidClubDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public SearchInquiryKidClubWithPaginationHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<InquiryKidClubDto>> Handle(SearchInquiryKidClubWithPaginationQuery request, CancellationToken cancellationToken)
        {
            request.StartDate = request.StartDate != null ? ((DateTime)request.StartDate).ToLocalTime() : null;
            request.EndDate = request.EndDate != null ? ((DateTime)request.EndDate).ToLocalTime() : null;
            var query = _context.MemberKids.Include(x => x.Member.PICStore).Include(x => x.Member.RequestsReceipteds).Where(x => !x.IsDeleted);

            #region check role
            Device currentDevice = await _context.Devices.FirstOrDefaultAsync(n => !n.IsDeleted && n.Id == request.DeviceId);
            if (currentDevice == null) throw new NotFoundException("Device not found");
            var storeIds = new List<int>();
            var user = _identityService.GetUserDto(_currentUserService.UserId);
            if (user?.StoreId != null)
            {
                var userStore = await _context.Stores.Where(s => s.Id == user.StoreId.Value && !s.IsDeleted)
                    .Include(s => s.Company.Stores).FirstOrDefaultAsync();
                if (userStore != null)
                {
                    if (_currentUserService.RoleLevel == RoleLevel.Level_2)
                    {
                        storeIds.Add(user.StoreId.Value);
                    }
                    else if (_currentUserService.RoleLevel == RoleLevel.Level_3)
                    {
                        storeIds.AddRange(userStore.Company.Stores.Select(s => s.Id));
                    }
                }
            }
            query = query.Where(n => n.Member.RequestsReceipteds.FirstOrDefault() != null && n.Member.RequestsReceipteds.FirstOrDefault().StoreId != null && storeIds.Contains((int)n.Member.RequestsReceipteds.FirstOrDefault().StoreId));
            #endregion

            if (!string.IsNullOrWhiteSpace(request.MemberNo))
            {
                query = query.Where(x => x.Member.MemberNo.Contains(request.MemberNo));
            }
            if (!string.IsNullOrWhiteSpace(request.KidName))
            {
                request.KidName = Regex.Replace(request.KidName, @"\s+", "");
                query = query.Where(x => (x.FirstName + x.LastName).ToLower().Contains(request.KidName.ToLower()));
            }
            if (request.PICStore.HasValue)
            {
                query = query.Where(x => x.Member.PICStoreId == request.PICStore);
            }
            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.Member.RequestsReceipteds.OrderByDescending(c => c.Id).FirstOrDefault().ReceiptedDatetime > request.StartDate);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.Member.RequestsReceipteds.OrderByDescending(c => c.Id).FirstOrDefault().ReceiptedDatetime < request.EndDate);
            }

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                query = query.OrderByCustom(request.OrderBy + " " + request.OrderType.ToUpper());
            }
            else
            {
                query = query.OrderByDescending(x => x.Id);
            }

            var listInquiryKid = await query.Select(x => new { Kid = x, RequestReceipted = x.Member.RequestsReceipteds.OrderByDescending(c => c.Id).FirstOrDefault() }).PaginatedListAsync(request.PageNumber, request.PageSize);
            var returnData = new List<InquiryKidClubDto>();

            var index = (request.PageNumber - 1) * request.PageSize + 1;
            foreach (var item in listInquiryKid.Items)
            {
                var dto = new InquiryKidClubDto();
                dto.No = index;
                dto.Id = item.Kid.Id;
                dto.MemberNo = item.Kid.Member?.MemberNo;
                dto.ReceiptedDatetime = item.RequestReceipted?.ReceiptedDatetime;
                dto.GuardianName = item.Kid.ParentLastName + "　" + item.Kid.ParentFirstName;
                dto.KidName = item.Kid.LastName + "　" + item.Kid.FirstName;
                dto.PICName = item.Kid.Member?.PICStore?.PICName;
                dto.Remark = item.Kid.Member?.Remark;

                returnData.Add(dto);
                index++;
            }

            return new PaginatedList<InquiryKidClubDto>(returnData, listInquiryKid.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}

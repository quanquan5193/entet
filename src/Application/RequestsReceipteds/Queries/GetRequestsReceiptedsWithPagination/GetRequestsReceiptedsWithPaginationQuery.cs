using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.ExpressionExtension;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Helpers.AzureKeyVaults;
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

namespace mrs.Application.RequestsReceipteds.Queries.GetRequestsReceiptedsWithPagination
{
    public class GetRequestsReceiptedsWithPaginationQuery : IRequest<PaginatedList<RequestsReceiptedDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string MemberNo { get; set; }
        public int? RequestType { get; set; }
        public string Company { get; set; }
        public string Store { get; set; }
        public string Device { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CreatedBy { get; set; }
        public string OrderBy { get; set; } = "ReceiptedDatetime";
        public string OrderType { get; set; } = "DESC";
        public bool IsMobileRequest { get; set; }
        public int? PICStoreId { get; set; }
    }

    public class GetRequestsReceiptedsWithPaginationQueryHandler : IRequestHandler<GetRequestsReceiptedsWithPaginationQuery, PaginatedList<RequestsReceiptedDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetRequestsReceiptedsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<RequestsReceiptedDto>> Handle(GetRequestsReceiptedsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            if (request.IsMobileRequest)
            {
                request.FromDate = request.FromDate != null ? ((DateTime)request.FromDate).ToLocalTime() : null;
                request.ToDate = request.ToDate != null ? ((DateTime)request.ToDate).ToLocalTime() : null;
            }
            else
            {
                request.FromDate = request.FromDate != null ? ((DateTime)request.FromDate) : null;
                request.ToDate = request.ToDate != null ? new DateTime(request.ToDate.Value.Year, request.ToDate.Value.Month, request.ToDate.Value.Day, 23, 59, 59) : null;
            }


            var query = _context.RequestsReceipteds
                .Include(x => x.Member)
                .Include(x => x.Member.PICStore)
                .Include(x => x.RequestType)
                .Include(x => x.Device)
                .Include(x => x.Store)
                .Include(x => x.Store.Company)
                .Where(x => !x.IsDeleted);

            #region check role

            if (request.IsMobileRequest)
            {
                // get user's stores based on role
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

                query = query.Where(r => r.Device != null && r.StoreId != null && storeIds.Contains((int)r.StoreId));
            }
            else
            {
                IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
                Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);
                if (loggedInUserRoles.Contains(RoleLevel.Level_6))
                {
                    query = query.Where(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId));
                }
            }
            
            #endregion

            if (!string.IsNullOrEmpty(request.MemberNo))
            {
                query = query.Where(r => r.Member.MemberNo.Contains(request.MemberNo));
            }
            if (request.RequestType.HasValue)
            {
                query = query.Where(r => r.RequestType.Id == request.RequestType);
            }
            if (!string.IsNullOrEmpty(request.Company))
            {
                query = query.Where(r => r.Store.Company.CompanyCode.ToLower().Contains(request.Company.ToLower()));
            }
            if (!string.IsNullOrEmpty(request.Store) && !request.IsMobileRequest)
            {
                query = query.Where(r => r.Store.StoreCode.ToLower().Contains(request.Store.ToLower()));
            }
            //if (!string.IsNullOrEmpty(request.Device))
            //{
            //    query = query.Where(r => r.Device.DeviceCode.ToLower().Contains(request.Device.ToLower()));
            //}
            if (request.FromDate.HasValue)
            {
                query = query.Where(r => r.ReceiptedDatetime >= request.FromDate);
            }
            if (request.ToDate.HasValue)
            {
                query = query.Where(r => r.ReceiptedDatetime <= request.ToDate);
            }
            if (!string.IsNullOrEmpty(request.CreatedBy))
            {
                query = query.Where(r => r.Member.PICStore.PICName.ToLower().Contains(request.CreatedBy.ToLower()));
            }
            if (request.PICStoreId.HasValue)
            {
                query = query.Where(r => r.Member.PICStoreId == request.PICStoreId);
            }
            if (string.IsNullOrEmpty(request.OrderBy))
            {
                request.OrderBy = nameof(RequestsReceipted.ReceiptedDatetime);
            }
            if (string.IsNullOrEmpty(request.OrderType))
            {
                request.OrderType = "desc";
            }

            #region Comment
            //if (!string.IsNullOrEmpty(request.OrderBy) && !string.IsNullOrEmpty(request.OrderType.ToUpper()) && new[] { "asc", "desc" }.Contains(request.OrderType.ToLower().Trim()))
            //{
            //    var asc = request.OrderType.ToLower().Trim() == "asc";
            //    switch (request.OrderBy.Trim())
            //    {
            //        case nameof(RequestsReceiptedDto.ReceiptedDatetime):
            //            query = asc
            //                ? query.OrderBy(x => x.ReceiptedDatetime)
            //                : query.OrderByDescending(x => x.ReceiptedDatetime);
            //            break;
            //        case nameof(RequestsReceiptedDto.RequestTypeName):
            //            query = asc
            //                ? query.OrderBy(x => x.RequestType.RequestTypeName)
            //                : query.OrderByDescending(x => x.RequestType.RequestTypeName);
            //            break;
            //        case nameof(RequestsReceiptedDto.MemberNo):
            //            query = asc
            //                ? query.OrderBy(x => x.Member.MemberNo)
            //                : query.OrderByDescending(x => x.Member.MemberNo);
            //            break;
            //        case nameof(RequestsReceiptedDto.MemberName):
            //            query = asc
            //                ? query.OrderBy(x => x.Member.FirstName).ThenBy(x => x.Member.LastName)
            //                : query.OrderByDescending(x => x.Member.FirstName).ThenByDescending(x => x.Member.LastName);
            //            break;
            //        case nameof(RequestsReceiptedDto.Company):
            //            query = asc
            //                ? query.OrderBy(x => x.Device.Store.Company.CompanyName)
            //                : query.OrderByDescending(x => x.Device.Store.Company.CompanyName);
            //            break;
            //        case nameof(RequestsReceiptedDto.Store):
            //            query = asc
            //                ? query.OrderBy(x => x.Device.Store.StoreName)
            //                : query.OrderByDescending(x => x.Device.Store.StoreName);
            //            break;
            //        case nameof(RequestsReceiptedDto.PICName):
            //            query = asc
            //                ? query.OrderBy(x => x.Member.PICStore.PICName)
            //                : query.OrderByDescending(x => x.Member.PICStore.PICName);
            //            break;
            //        case nameof(RequestsReceiptedDto.Remark):
            //            query = asc
            //                ? query.OrderBy(x => x.Member.Remark)
            //                : query.OrderByDescending(x => x.Member.Remark);
            //            break;
            //    }
            //}
            //else
            //{
            //    query = query.OrderByDescending(x => x.Id);
            //}
            #endregion
            switch (request.OrderBy)
            {
                case nameof(RequestsReceiptedDto.ReceiptedDatetime):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.ReceiptedDatetime) : query.OrderByDescending(n => n.ReceiptedDatetime);
                    break;
                case nameof(RequestsReceiptedDto.RequestTypeName):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.RequestType.RequestTypeName) : query.OrderByDescending(n => n.RequestType.RequestTypeName);
                    break;
                case nameof(RequestsReceiptedDto.MemberNo):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.Member.MemberNo) : query.OrderByDescending(n => n.Member.MemberNo);
                    break;
                case nameof(RequestsReceiptedDto.MemberName):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.Member.LastName).ThenBy(n => n.Member.FirstName) : query.OrderByDescending(n => n.Member.LastName).ThenBy(n => n.Member.FirstName);
                    break;
                case nameof(RequestsReceiptedDto.NormalizedCompanyName):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.Store.Company.NormalizedCompanyName) : query.OrderByDescending(n => n.Store.Company.NormalizedCompanyName);
                    break;
                case nameof(RequestsReceiptedDto.NormalizedStoreName):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.Store.NormalizedStoreName) : query.OrderByDescending(n => n.Store.NormalizedStoreName);
                    break;
                case nameof(RequestsReceiptedDto.PicName):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.Member.PICStore.PICName) : query.OrderByDescending(n => n.Member.PICStore.PICName);
                    break;
                case nameof(RequestsReceiptedDto.Remark):
                    query = request.OrderType.Equals("asc") ?
                        query.OrderBy(n => n.Member.Remark) : query.OrderByDescending(n => n.Member.Remark);
                    break;
                default:
                    query = query.OrderBy(n => n.ReceiptedDatetime);
                    break;
            }

            var obj = await query.PaginatedListAsync(request.PageNumber, request.PageSize);
            var returnData = new List<RequestsReceiptedDto>();
            var index = (request.PageNumber - 1) * request.PageSize + 1;
            foreach (var item in obj.Items)
            {
                var dto = new RequestsReceiptedDto
                {
                    Id = item.Id,
                    Index = index,
                    ReceiptedDatetime = item.ReceiptedDatetime,
                    RequestTypeName = item.RequestType.RequestTypeName,
                    MemberNo = item.Member == null ? "" : item.Member.MemberNo,
                    MemberName = item.Member == null ? "" : $"{ await item.Member.LastName.ToDecryptStringAsync()}" + "　" + $"{ await item.Member.FirstName.ToDecryptStringAsync()}",
                    Store = item.Store.StoreName,
                    NormalizedStoreName = item.Store.NormalizedStoreName,
                    Company = item.Store.Company.CompanyName,
                    NormalizedCompanyName = item.Store.Company.NormalizedCompanyName,
                    PicName = item.Member?.PICStore == null ? "" : item.Member.PICStore.PICName,
                    Remark = item.Member == null ? "" : item.Member.Remark
                };
                returnData.Add(dto);
                index++;
            }

            return new PaginatedList<RequestsReceiptedDto>(returnData, obj.TotalCount, obj.PageIndex, request.PageSize);
        }
    }
}

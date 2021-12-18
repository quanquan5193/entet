using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ReferPrepaidCard.Queries.SearchPrepaidCard
{
    public class SearchPrepaidCardQuery : IRequest<PaginatedList<PrepaidCardDto>>
    {
        public string CustomerNo { get; set; }

        public int? RequestType { get; set; }

        public int? PICStoreId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string OrderBy { get; set; } = "memberno";

        public string OrderType { get; set; } = "asc";

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int DeviceId { get; set; }
    }

    public class SearchPrepaidCardQueryHandler : IRequestHandler<SearchPrepaidCardQuery, PaginatedList<PrepaidCardDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public SearchPrepaidCardQueryHandler(IApplicationDbContext context, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }


        public async Task<PaginatedList<PrepaidCardDto>> Handle(SearchPrepaidCardQuery request, CancellationToken cancellationToken)
        {
            #region check role
            Device currentDevice = await _context.Devices.FirstOrDefaultAsync(n => !n.IsDeleted && n.Id == request.DeviceId);
            if (currentDevice == null) throw new NotFoundException("Device not found");
            List<int> storeIds = new List<int>();
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
            #endregion

            var query = from card in _context.Cards.Where(n => n.StoreId != null && storeIds.Contains((int)n.StoreId))
                        from requestReceipt in _context.RequestsReceipteds.Include(x => x.Member).ThenInclude(x => x.PICStore).Where(x => x.CardId == card.Id && x.StoreId != null && storeIds.Contains((int)x.StoreId) && !x.IsDeleted && (!request.PICStoreId.HasValue || (request.PICStoreId.HasValue && x.Member.PICStoreId == request.PICStoreId))
                                                        && (!request.RequestType.HasValue || (request.RequestType.HasValue && x.ReceiptedTypeId == request.RequestType))
                                                        && (!request.StartDate.HasValue || (request.StartDate.HasValue && x.ReceiptedDatetime >= request.StartDate))
                                                        && (!request.EndDate.HasValue || (request.EndDate.HasValue && x.ReceiptedDatetime <= request.EndDate))).OrderByDescending(x => x.Id).Take(1).DefaultIfEmpty()                   
                         select new
                        {
                            Card = card,
                            RequestsReceipted = requestReceipt
                        };

            if (!string.IsNullOrWhiteSpace(request.CustomerNo))
            {
                query = query.Where(x => x.Card.MemberNo.Contains(request.CustomerNo));
            }
            if (request.PICStoreId.HasValue)
            {
                query = query.Where(x => x.RequestsReceipted.Member.PICStoreId == request.PICStoreId);
            }
            if (request.RequestType.HasValue)
            {
                query = query.Where(x => x.RequestsReceipted.ReceiptedTypeId == request.RequestType);
            }
            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.RequestsReceipted.ReceiptedDatetime >= request.StartDate);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.RequestsReceipted.ReceiptedDatetime <= request.EndDate);
            }

            query = query.Where(x => !x.Card.IsDeleted);

            if (new[] { "asc", "desc" }.Contains(request.OrderType.ToLower().Trim()))
            {
                var asc = request.OrderType.ToLower().Trim() == "asc";
                switch (request.OrderBy.ToLower().Trim())
                {
                    case "id":
                        query = asc
                            ? query.OrderBy(x => x.Card.Id)
                            : query.OrderByDescending(x => x.Card.Id);
                        break;
                    case "memberno":
                        query = asc
                            ? query.OrderBy(x => x.Card.MemberNo)
                            : query.OrderByDescending(x => x.Card.MemberNo);
                        break;
                    case "receiptdate":
                        query = asc
                            ? query.OrderBy(x => x.RequestsReceipted.ReceiptedDatetime)
                            : query.OrderByDescending(x => x.RequestsReceipted.ReceiptedDatetime);
                        break;
                    case "status":
                        query = asc
                            ? query.OrderBy(x => x.Card.Status)
                            : query.OrderByDescending(x => x.Card.Status);
                        break;
                    case "requesttype":
                        query = asc
                            ? query.OrderBy(x => x.RequestsReceipted.ReceiptedTypeId)
                            : query.OrderByDescending(x => x.RequestsReceipted.ReceiptedTypeId);
                        break;
                    case "picstore":
                        query = asc
                            ? query.OrderBy(x => x.RequestsReceipted.Member.PICStore.PICName)
                            : query.OrderByDescending(x => x.RequestsReceipted.Member.PICStore.PICName);
                        break;
                    case "remark":
                        query = asc
                            ? query.OrderBy(x => x.RequestsReceipted.Member.Remark)
                            : query.OrderByDescending(x => x.RequestsReceipted.Member.Remark);
                        break;
                }
            }

            var listPrpaidCar = await query.PaginatedListAsync(request.PageNumber, request.PageSize);
            var returnData = new List<PrepaidCardDto>();

            var index = (request.PageNumber - 1) * request.PageSize + 1;
            foreach (var item in listPrpaidCar.Items)
            {
                var dto = new PrepaidCardDto();
                dto.Id = item.Card.Id;
                dto.No = index;
                dto.MemberNo = item.Card.MemberNo;
                dto.ReceiptedDatetime = item.RequestsReceipted?.ReceiptedDatetime;
                dto.Status = ((CardStatus)item.Card.Status).GetStringValue();
                dto.RequestType = item.RequestsReceipted != null ?
                        ((RequestTypeEnum)item.RequestsReceipted.ReceiptedTypeId).GetStringValue() : null;
                dto.PICStore = item.RequestsReceipted?.Member?.PICStore?.PICName;
                dto.Remark = item.RequestsReceipted?.Member?.Remark;

                returnData.Add(dto);
                index++;
            }

            return new PaginatedList<PrepaidCardDto>(returnData, listPrpaidCar.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}

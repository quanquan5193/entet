using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.Receptions.Queries.Dto;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Receptions.Queries.GetAppReceptionsWithPagination
{
    public class GetAppReceptionsWithPaginationQuery : IRequest<PaginatedList<ReceptionDetailDto>>
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int DeviceId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

    public class GetAppReceptionsWithPaginationQueryHandler : IRequestHandler<GetAppReceptionsWithPaginationQuery, PaginatedList<ReceptionDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetAppReceptionsWithPaginationQueryHandler(IApplicationDbContext context, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<ReceptionDetailDto>> Handle(GetAppReceptionsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            DateTime fromDateSearch = DateTime.Now;
            DateTime toDateSearch = DateTime.Now;
            if (!DateTime.TryParse(request.FromDate, out fromDateSearch)) throw new ValidationException();
            if (!DateTime.TryParse(request.ToDate, out toDateSearch)) throw new ValidationException();
            toDateSearch = toDateSearch.AddDays(1);

            double totalDate = (toDateSearch - fromDateSearch).TotalDays;
            double startDatePagination = totalDate < ((request.PageNumber - 1) * request.PageSize) ? totalDate : totalDate - ((request.PageNumber - 1) * request.PageSize);
            double dateRangeGet = startDatePagination < request.PageSize ? startDatePagination : request.PageSize;
            double endDatePagination = startDatePagination - dateRangeGet;
            DateTime startDateQuerySearch = fromDateSearch.AddDays(endDatePagination - 1);
            DateTime endDateQuerySearch = fromDateSearch.AddDays(startDatePagination);

            IQueryable<RequestsReceipted> query = _context.RequestsReceipteds.Where(n => !n.IsDeleted && n.ReceiptedDatetime >= fromDateSearch && n.ReceiptedDatetime < toDateSearch);

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
            query = query.Where(n => n.StoreId != null && storeIds.Contains((int)n.StoreId));
            #endregion

            List<ReceptionDetailDto> result = query.GroupBy(x => new DateObject
                { 
                    Year = x.ReceiptedDatetime.Year,
                    Month = x.ReceiptedDatetime.Month,
                    Day = x.ReceiptedDatetime.Day
                })
                .Select(y => new ReceptionDetailDto
                {
                    ReceptionDateObj = y.Key,
                    TotalCreateCards = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.New),
                    TotalSwitchCards = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.Switch),
                    TotalReissuedCards = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.ReIssued),
                    TotalChangeCards = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.ChangeCard),
                    TotalPointMigration = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.PMigrate),
                    TotalDiscardCards = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.LeaveGroup),
                }).ToList();

            List<ReceptionDetailDto> resultKid = _context.MemberKids
                .Where(n => !n.IsDeleted && n.CreatedAt >= startDateQuerySearch && n.CreatedAt < endDateQuerySearch && storeIds.Contains((int)n.Member.RequestsReceipteds.FirstOrDefault().StoreId))
                .GroupBy(x => new DateObject
                {
                    Year = x.CreatedAt.Year,
                    Month = x.CreatedAt.Month,
                    Day = x.CreatedAt.Day
                })
                .Select(y => new ReceptionDetailDto
                {
                    ReceptionDateObj = y.Key,
                    TotalKidClubs = y.Count()
                }).ToList();

            Parallel.ForEach(result, item =>
            {
                ReceptionDetailDto kid = resultKid.FirstOrDefault(n => (n.ReceptionDateObj.Year == item.ReceptionDateObj.Year && n.ReceptionDateObj.Month == item.ReceptionDateObj.Month && n.ReceptionDateObj.Day == item.ReceptionDateObj.Day));
                item.TotalKidClubs = kid?.TotalKidClubs ?? 0;
                item.ReceptionDate = DateTime.Parse($"{item.ReceptionDateObj.Year}/{item.ReceptionDateObj.Month}/{item.ReceptionDateObj.Day}");
            });

            List<ReceptionDetailDto> dataTable = new List<ReceptionDetailDto>();
            int index = 1;
            for (double i = startDatePagination - 1; i >= endDatePagination; i--)
            {
                DateTime currentDay = fromDateSearch.AddDays(i);
                var item = result.FirstOrDefault(n => n.ReceptionDate == currentDay);
                if (item == null)
                {
                    dataTable.Add(new ReceptionDetailDto()
                    {
                        No = index,
                        ReceptionDate = currentDay,
                        TotalChangeCards = 0,
                        TotalCreateCards = 0,
                        TotalDiscardCards = 0,
                        TotalKidClubs = 0,
                        TotalPointMigration = 0,
                        TotalReissuedCards = 0,
                        TotalSwitchCards = 0
                    });
                }
                else
                {
                    item.No = index;
                    dataTable.Add(item);
                }
                index++;
            }

            int totalCount = (int)totalDate;

            PaginatedList<ReceptionDetailDto> receptionList = new(dataTable, totalCount, request.PageNumber, request.PageSize);

            return receptionList;
        }
    }
}

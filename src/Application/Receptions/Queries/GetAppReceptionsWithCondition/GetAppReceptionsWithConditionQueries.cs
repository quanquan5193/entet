using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Receptions.Queries.Dto;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Receptions.Queries.GetAppReceptionsWithCondition
{
    public class GetAppReceptionsWithConditionQuery : IRequest<ReceptionGraphResult>
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int DeviceId { get; set; }
    }

    public class GetAppReceptionsWithConditionQueryHandler : IRequestHandler<GetAppReceptionsWithConditionQuery, ReceptionGraphResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetAppReceptionsWithConditionQueryHandler(IApplicationDbContext context, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<ReceptionGraphResult> Handle(GetAppReceptionsWithConditionQuery request, CancellationToken cancellationToken)
        {
            DateTime fromDateSearch = DateTime.Now;
            DateTime toDateSearch = DateTime.Now;
            if (!DateTime.TryParse(request.FromDate, out fromDateSearch)) throw new ValidationException();
            if (!DateTime.TryParse(request.ToDate, out toDateSearch)) throw new ValidationException();
            toDateSearch = toDateSearch.AddDays(1);
            DateTime sixMonthFromStartDate = fromDateSearch.AddMonths(6);
            bool isDisplayMonth = sixMonthFromStartDate <= toDateSearch.AddDays(-1);

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

            List<ReceptionGraphDto> result = query.GroupBy(x => new DateObject
                { 
                    Year = x.ReceiptedDatetime.Year,
                    Month = x.ReceiptedDatetime.Month,
                    Day = isDisplayMonth ? 1 : x.ReceiptedDatetime.Day
                })
                .Select(y => new ReceptionGraphDto
                {
                    ReceptionDateObj = y.Key,
                    IsDisplayMonth = isDisplayMonth,
                    TotalCreateCards = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.New),
                    TotalSwitchCards = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.Switch),
                    TotalOther = y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.ReIssued || n.ReceiptedTypeId == (int)RequestTypeEnum.ChangeCard || n.ReceiptedTypeId == (int)RequestTypeEnum.PMigrate || n.ReceiptedTypeId == (int)RequestTypeEnum.LeaveGroup),
                }).ToList();
            List<ReceptionGraphDto> resultKid = _context.MemberKids
                .Where(n => !n.IsDeleted && n.CreatedAt >= fromDateSearch && n.CreatedAt < toDateSearch && storeIds.Contains((int)n.Member.RequestsReceipteds.FirstOrDefault().StoreId))
                .GroupBy(x => new DateObject
                {
                    Year = x.CreatedAt.Year,
                    Month = x.CreatedAt.Month,
                    Day = isDisplayMonth ? 1 : x.CreatedAt.Day
                })
                .Select(y => new ReceptionGraphDto
                {
                    ReceptionDateObj = y.Key,
                    IsDisplayMonth = isDisplayMonth,
                    TotalKidClubs = y.Count()
                }).ToList();

            Parallel.ForEach(result, item =>
            {
                var kid = resultKid.FirstOrDefault(n => (n.ReceptionDateObj.Year == item.ReceptionDateObj.Year && n.ReceptionDateObj.Month == item.ReceptionDateObj.Month && n.ReceptionDateObj.Day == item.ReceptionDateObj.Day));
                item.TotalKidClubs = kid?.TotalKidClubs ?? 0;
                item.ReceptionDate = DateTime.Parse($"{item.ReceptionDateObj.Year}/{item.ReceptionDateObj.Month}/{item.ReceptionDateObj.Day}");
            });

            List<ReceptionGraphDto> dataChart = new List<ReceptionGraphDto>();
            if (!isDisplayMonth)
            {
                double totalDate = (toDateSearch - fromDateSearch).TotalDays;
                for (int i = 0; i < totalDate; i++)
                {
                    DateTime currentDay = fromDateSearch.AddDays(i);
                    var item = result.FirstOrDefault(n => n.ReceptionDate == currentDay);
                    if (item == null)
                    {
                        dataChart.Add(new ReceptionGraphDto()
                        {
                            ReceptionDate = currentDay,
                            IsDisplayMonth = isDisplayMonth,
                            TotalOther = 0,
                            TotalCreateCards = 0,
                            TotalKidClubs = 0,
                            TotalSwitchCards = 0
                        });
                    }
                    else
                    {
                        dataChart.Add(item);
                    }
                }
            }
            else
            {
                double totalMonth = Math.Abs((fromDateSearch.Month - toDateSearch.AddDays(-1).Month) + 12 * (fromDateSearch.Year - toDateSearch.AddDays(-1).Year));
                for (int i = 0; i <= totalMonth; i++)
                {
                    DateTime currentDay = fromDateSearch.AddMonths(i);
                    var item = result.FirstOrDefault(n => n.ReceptionDate.Year == currentDay.Year && n.ReceptionDate.Month == currentDay.Month);
                    if (item == null)
                    {
                        dataChart.Add(new ReceptionGraphDto()
                        {
                            ReceptionDate = currentDay,
                            IsDisplayMonth = isDisplayMonth,
                            TotalOther = 0,
                            TotalCreateCards = 0,
                            TotalKidClubs = 0,
                            TotalSwitchCards = 0
                        });
                    }
                    else
                    {
                        dataChart.Add(item);
                    }
                }
            }

            ReceptionGraphResult receptionList = new();
            receptionList.ListData = dataChart;
            receptionList.Total = new()
            {
                TotalCreateCards = result.Sum(n => n.TotalCreateCards),
                TotalSwitchCards = result.Sum(n => n.TotalSwitchCards),
                TotalKidClubs = result.Sum(n => n.TotalKidClubs),
                TotalOther = result.Sum(n => n.TotalOther),
            };


            return receptionList;
        }
    }
}

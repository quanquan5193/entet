using AutoMapper;
using MediatR;
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

namespace mrs.Application.Receptions.Queries.GetAdminReceptionsWithCondition
{
    public class GetAdminReceptionsWithConditionQuery : IRequest<ReceptionGraphResult>
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool IsSearchCreateCards { get; set; }
        public bool IsSearchSwitchCards { get; set; }
        public bool IsSearchReissuedCards { get; set; }
        public bool IsSearchChangeCards { get; set; }
        public bool IsSearchDiscardCards { get; set; }
        public bool IsSearchPointMigration { get; set; }
        public bool IsSearchKidClubs { get; set; }
    }

    public class GetAdminReceptionsWithConditionQueryHandler : IRequestHandler<GetAdminReceptionsWithConditionQuery, ReceptionGraphResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        public GetAdminReceptionsWithConditionQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<ReceptionGraphResult> Handle(GetAdminReceptionsWithConditionQuery request, CancellationToken cancellationToken)
        {
            DateTime fromDateSearch = DateTime.Now;
            DateTime toDateSearch = DateTime.Now;
            if (!DateTime.TryParse(request.FromDate, out fromDateSearch)) throw new ValidationException();
            if (!DateTime.TryParse(request.ToDate, out toDateSearch)) throw new ValidationException();
            toDateSearch = toDateSearch.AddDays(1);
            DateTime sixMonthFromStartDate = fromDateSearch.AddMonths(6);
            bool isDisplayMonth = sixMonthFromStartDate <= toDateSearch.AddDays(-1);
            IQueryable<RequestsReceipted> query = _context.RequestsReceipteds.Where(n => !n.IsDeleted && n.ReceiptedDatetime >= fromDateSearch && n.ReceiptedDatetime < toDateSearch);
            IQueryable<MemberKid> queryKids = _context.MemberKids.Where(n => !n.IsDeleted && n.CreatedAt >= fromDateSearch && n.CreatedAt < toDateSearch);

            #region check role
            IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);

            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId));
                queryKids = queryKids.Where(x => x.Member.RequestsReceipteds.Any(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId)));
            }
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
                    TotalCreateCards = request.IsSearchCreateCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.New) : 0,
                    TotalSwitchCards = request.IsSearchSwitchCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.Switch) : 0,
                    TotalOther = 
                    (request.IsSearchReissuedCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.ReIssued) : 0) +
                    (request.IsSearchChangeCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.ChangeCard) : 0) +
                    (request.IsSearchPointMigration ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.PMigrate) : 0) +
                    (request.IsSearchDiscardCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.LeaveGroup) : 0),
                }).ToList();

            List<ReceptionGraphDto> resultKid = queryKids.GroupBy(x => new DateObject
                {
                    Year = x.CreatedAt.Year,
                    Month = x.CreatedAt.Month,
                    Day = isDisplayMonth ? 1 : x.CreatedAt.Day
                })
                .Select(y => new ReceptionGraphDto
                {
                    ReceptionDateObj = y.Key,
                    IsDisplayMonth = isDisplayMonth,
                    TotalKidClubs = request.IsSearchKidClubs ? y.Count() : 0
                }).ToList();

            Parallel.ForEach(result, item =>
            {
                ReceptionGraphDto kid = resultKid.FirstOrDefault(n => (n.ReceptionDateObj.Year == item.ReceptionDateObj.Year && n.ReceptionDateObj.Month == item.ReceptionDateObj.Month && n.ReceptionDateObj.Day == item.ReceptionDateObj.Day));
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
                TotalCreateCards = dataChart.Sum(n => n.TotalCreateCards),
                TotalSwitchCards = dataChart.Sum(n => n.TotalSwitchCards),
                TotalKidClubs = dataChart.Sum(n => n.TotalKidClubs),
                TotalOther = dataChart.Sum(n => n.TotalOther),
            };

            return receptionList;
        }
    }
}

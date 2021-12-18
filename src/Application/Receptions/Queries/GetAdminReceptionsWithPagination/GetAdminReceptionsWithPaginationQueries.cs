using AutoMapper;
using MediatR;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Receptions.Queries.GetAdminReceptionsWithPagination
{
    public class GetAdminReceptionsWithPaginationQuery : IRequest<PaginatedList<ReceptionDetailDto>>
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
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

    public class GetAdminReceptionsWithPaginationQueryHandler : IRequestHandler<GetAdminReceptionsWithPaginationQuery, PaginatedList<ReceptionDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public GetAdminReceptionsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedList<ReceptionDetailDto>> Handle(GetAdminReceptionsWithPaginationQuery request, CancellationToken cancellationToken)
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

            IQueryable<RequestsReceipted> query = _context.RequestsReceipteds.Where(n => !n.IsDeleted && n.ReceiptedDatetime >= startDateQuerySearch && n.ReceiptedDatetime < endDateQuerySearch);
            IQueryable<MemberKid> queryKids = _context.MemberKids.Where(n => !n.IsDeleted && n.CreatedAt >= startDateQuerySearch && n.CreatedAt < endDateQuerySearch);

            #region check role
            IList<string> loggedInUserRoles = await _identityService.GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await _identityService.GetStoreAsync(_currentUserService.UserId);

            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId));
                queryKids = queryKids.Where(x => x.Member.RequestsReceipteds.Any(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId)));
            }
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
                    TotalCreateCards = request.IsSearchCreateCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.New) : 0,
                    TotalSwitchCards = request.IsSearchSwitchCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.Switch) : 0,
                    TotalReissuedCards = request.IsSearchReissuedCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.ReIssued) : 0,
                    TotalChangeCards = request.IsSearchChangeCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.ChangeCard) : 0,
                    TotalPointMigration = request.IsSearchPointMigration ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.PMigrate) : 0,
                    TotalDiscardCards = request.IsSearchDiscardCards ? y.Count(n => n.ReceiptedTypeId == (int)RequestTypeEnum.LeaveGroup) : 0,
                }).ToList();

            List<ReceptionDetailDto> resultKid = queryKids.GroupBy(x => new DateObject
                {
                    Year = x.CreatedAt.Year,
                    Month = x.CreatedAt.Month,
                    Day = x.CreatedAt.Day
                })
                .Select(y => new ReceptionDetailDto
                {
                    ReceptionDateObj = y.Key,
                    TotalKidClubs = request.IsSearchKidClubs ? y.Count() : 0
                }).ToList();

            Parallel.ForEach(result, item =>
            {
                ReceptionDetailDto kid = resultKid.FirstOrDefault(n => (n.ReceptionDateObj.Year == item.ReceptionDateObj.Year && n.ReceptionDateObj.Month == item.ReceptionDateObj.Month && n.ReceptionDateObj.Day == item.ReceptionDateObj.Day));
                item.TotalKidClubs = kid?.TotalKidClubs ?? 0;
                item.ReceptionDate = DateTime.Parse($"{item.ReceptionDateObj.Year}/{item.ReceptionDateObj.Month}/{item.ReceptionDateObj.Day}");
            });

            List<ReceptionDetailDto> dataTable = new List<ReceptionDetailDto>();
            for (double i = startDatePagination - 1; i >= endDatePagination; i--)
            {
                DateTime currentDay = fromDateSearch.AddDays(i);
                var item = result.FirstOrDefault(n => n.ReceptionDate == currentDay);
                if (item == null)
                {
                    dataTable.Add(new ReceptionDetailDto()
                    {
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
                    dataTable.Add(item);
                }
            }

            int totalCount = (int)totalDate;

            PaginatedList<ReceptionDetailDto> receptionList = new(dataTable, totalCount, request.PageNumber, request.PageSize);

            return receptionList;
        }
    }
}

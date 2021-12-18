using AutoMapper;
using mrs.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using mrs.Domain.Enums;
using System.Collections.Generic;
using mrs.Application.Common.Exceptions;
using mrs.Application.Receptions.Queries.Dto;
using mrs.Domain.Entities;
using mrs.Application.Common.Helpers;

namespace mrs.Application.Cards.Queries.ExportReceptionsTable
{
    public class ExportAdminReceptionsTableQuery : IRequest<ExportReceptionsTableVm>
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

    public class ExportReceptionsTableQueryHandler : IRequestHandler<ExportAdminReceptionsTableQuery, ExportReceptionsTableVm>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICsvFileBuilder _fileBuilder;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public ExportReceptionsTableQueryHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder, IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _fileBuilder = fileBuilder;
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        public async Task<ExportReceptionsTableVm> Handle(ExportAdminReceptionsTableQuery request, CancellationToken cancellationToken)
        {
            DateTime fromDateSearch = DateTime.Now;
            DateTime toDateSearch = DateTime.Now;
            if (!DateTime.TryParse(request.FromDate, out fromDateSearch)) throw new ValidationException();
            if (!DateTime.TryParse(request.ToDate, out toDateSearch)) throw new ValidationException();
            toDateSearch = toDateSearch.AddDays(1);

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

            List<ReceptionDetailDto> resultKid =queryKids.GroupBy(x => new DateObject
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

            double totalDate = (toDateSearch - fromDateSearch).TotalDays;
            List<ReceptionDetailDto> dataTable = new List<ReceptionDetailDto>();
            int index = 1;
            for (double i = totalDate - 1; i >= 0; i--)
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

            ExportReceptionsTableVm vm = new ExportReceptionsTableVm();
            List<ReceptionsRecord> resultMapping = _mapper.Map<List<ReceptionDetailDto>, List<ReceptionsRecord>>(dataTable);
            vm.Content = _fileBuilder.BuilReceptionFile(resultMapping);
            vm.ContentType = "text/csv";
            vm.FileName = $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv";

            return await Task.FromResult(vm);
        }
    }
}

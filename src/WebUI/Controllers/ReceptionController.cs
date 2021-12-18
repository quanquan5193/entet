using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Cards.Queries.ExportAdminReceptionsGraphQuery;
using mrs.Application.Cards.Queries.ExportReceptionsTable;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.Receptions.Queries.Dto;
using mrs.Application.Receptions.Queries.GetAdminReceptionsWithCondition;
using mrs.Application.Receptions.Queries.GetAdminReceptionsWithPagination;
using mrs.Application.Receptions.Queries.GetAppReceptionsWithCondition;
using mrs.Application.Receptions.Queries.GetAppReceptionsWithPagination;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class ReceptionController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string GetReceptionsWithoutPagination = "受付状況のデータをグラフとして表示するためデータを取得し集計しました。";
            public static readonly string GetReceptionsWithPagination = "受付状況のデータを表として表示するためデータを取得し集計しました。";
            public static readonly string GetAdminReceptionsWithoutPagination = "管理者に向けて受付状況のデータをグラフとして表示するためデータを取得し集計しました。";
            public static readonly string GetAdminReceptionsWithPagination = "管理者に向けて受付状況のデータを表として表示するためデータを取得し集計しました。";
            public static readonly string ExportReceptionsTable = "受付状況のデータをグラフとして出力しました。";
            public static readonly string ExportReceptionsGraph = "受付状況のデータを表として出力しました。";
        }
        public ReceptionController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<ReceptionGraphResult>> GetCardsWithCondition([FromQuery] GetAppReceptionsWithConditionQuery param)
        {
            var receptions = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetReceptionsWithoutPagination);
            return receptions;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<PaginatedList<ReceptionDetailDto>>> GetCardsWithPagination([FromQuery] GetAppReceptionsWithPaginationQuery param)
        {
            var receptions = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetReceptionsWithPagination);
            return receptions;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<ReceptionGraphResult>> GetAdminCardsWithCondition([FromQuery] GetAdminReceptionsWithConditionQuery param)
        {
            var receptions = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetAdminReceptionsWithoutPagination);
            return receptions;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<ReceptionDetailDto>>> GetAdminCardsWithPagination([FromQuery] GetAdminReceptionsWithPaginationQuery param)
        {
            var receptions = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetAdminReceptionsWithPagination);
            return receptions;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<FileResult> GetAdminReceptionsExportTable([FromQuery] ExportAdminReceptionsTableQuery param)
        {
            var vm = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.ExportReceptionsTable);
            return File(vm.Content, vm.ContentType, vm.FileName);
        }

        [HttpPost("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<FileResult> GetAdminReceptionsExportGraph([FromForm] ExportAdminReceptionsGraphQueryQuery param)
        {
            var vm = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.ExportReceptionsGraph);
            return File(vm.Content, vm.ContentType, vm.FileName);
        }
    }
}

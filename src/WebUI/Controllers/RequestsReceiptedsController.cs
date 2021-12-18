using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.RequestsReceipteds.Commands;
using mrs.Application.RequestsReceipteds.Commands.UpdateHistory;
using mrs.Application.RequestsReceipteds.Queries.ExportRequestReceipted;
using mrs.Application.RequestsReceipteds.Queries.GetRequestsReceipted;
using mrs.Application.RequestsReceipteds.Queries.GetRequestsReceiptedsWithPagination;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class RequestsReceiptedsController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string GetRequestReceipts = "受付申請一覧を正常に取得しました。";
            public static readonly string GetRequestReceipt = "受付申請を正常に取得しました。({0})";
            public static readonly string UpdateRequestReceipt = "受付申請を正常に更新しました。({0})";
            public static readonly string DeleteRequestReceipt = "受付申請を正常に削除しました。 ({0})";
            public static readonly string ExportRequestReceipt = "受付申請を正常に出力しました。";
        }
        public RequestsReceiptedsController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpGet]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<RequestsReceiptedDto>>> GetRequestsReceiptedsWithPagination([FromQuery] GetRequestsReceiptedsWithPaginationQuery query)
        {
            var requestReceipts = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetRequestReceipts);
            return requestReceipts;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<PaginatedList<RequestsReceiptedDto>>> GetRequestsReceiptedsWithPaginationMobile([FromQuery] GetRequestsReceiptedsWithPaginationQuery query)
        {
            var requestReceipts = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetRequestReceipts);
            return requestReceipts;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<RequestsReceiptedDetailsDto>> GetRequestsReceipted([FromQuery] GetRequestsReceiptedQuery query)
        {
            var requestReceipt = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetRequestReceipt, query.Id));
            return requestReceipt;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<RequestsReceiptedDetailsDto>> GetRequestsReceiptedMobile([FromQuery] GetRequestsReceiptedQuery query)
        {
            var requestReceipt = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetRequestReceipt, query.Id));
            return requestReceipt;
        }

        [HttpDelete("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteHistoryCommand { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeleteRequestReceipt, id));
            return Ok();
        }

        [HttpPut("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateHistoryCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateRequestReceipt, id));
            return NoContent();
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<FileResult> ExportRequestReceipt([FromQuery] ExportRequestReceiptedQuery query)
        {
            var vm = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.ExportRequestReceipt);
            return File(vm.Content, vm.ContentType, vm.FileName);
        }
    }
}

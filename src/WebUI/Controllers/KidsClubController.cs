using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Cards.Commands.DeleteCards;
using mrs.Application.Cards.Queries.GetKidsWithPagination;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.Kids.Commands.DeleteKid;
using mrs.Application.Kids.Commands.UpdateKid;
using mrs.Application.Kids.Queries.ExportKids;
using mrs.Application.Kids.Queries.GetKid;
using mrs.Application.Kids.Queries.GetKidsPaging;
using mrs.Application.Kids.Queries.GetKidsWithPagination;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    public class KidsClubController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string GetKids = "お子様会員一覧を正常に取得しました。";
            public static readonly string GetKidDetail = "お子様会員情報を正常に取得しました。({0})";
            public static readonly string DeleteKid = "お子様会員を正常に削除しました。({0})";
            public static readonly string UpdateKid = "お子様会員情報を正常に更新しました。({0})";
            public static readonly string ExportKids = "お子様会員情報を正常に出力しました。";
            public static readonly string GetKidCardsForDeleting = "削除用のお子様会員のカード一覧を正常に取得しました。";
        }

        public KidsClubController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpGet]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<KidDto>>> GetPaging([FromQuery] GetKidPagingQuery query)
        {
            var kids = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetKids);
            return kids;
        }

        [HttpGet]
        [Route("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<KidDetailDto> GetKid(int id)
        {
            var kidDetail = await Mediator.Send(new GetKidQuery() { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetKidDetail, id));
            return kidDetail;
        }

        [HttpDelete]
        [Route("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<bool> DeleteKid(int id)
        {
            var deleteResult = await Mediator.Send(new DeleteKidCommand() { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeleteKid, id));
            return deleteResult;
        }

        [HttpPut]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<UpdateKidResultDto> UpdateKid([FromBody] UpdateKidCommand command)
        {
            var updateResult = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateKid, command.Id));
            return updateResult;
        }

        [HttpGet]
        [Route("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_7, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<FileResult> ExportKids([FromQuery] ExportKidsQuery query)
        {
            ExportKidsVm vm = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.ExportKids);
            return File(vm.Content, vm.ContentType, vm.FileName);
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<MemberKidDeleteDto>>> GetKidsDeleteWithPagination([FromQuery] GetKidsDeleteWithPaginationQuery param)
        {
            var cards = await Mediator.Send(param);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetKidCardsForDeleting);
            return cards;
        }


        [HttpPost("[action]")]
        [DisableRequestSizeLimit]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<bool>> DeleteMemberKids(DeleteKidsCommand command)
        {
            var deletedIds = await Mediator.Send(command);
            var messageBuilder = new StringBuilder();
            if (deletedIds != null && deletedIds.Any())
            {
                foreach (var deletedId in deletedIds)
                {
                    var messageOnly = string.Format(LoggingMessage.DeleteKid, deletedId);
                    string messageFull = await _azureStorageHelpers.ConfigureMessage(messageOnly);
                    messageBuilder.AppendLine(messageFull);
                } 
            }
            await _azureStorageHelpers.SaveMultipleLogToBlob(messageBuilder.ToString());
            return true;
        }
    }
}

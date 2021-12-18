using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.ApplicationUser.Command.CreateUser;
using mrs.Application.ApplicationUser.Command.DeleteUser;
using mrs.Application.ApplicationUser.Command.UpdateUser;
using mrs.Application.ApplicationUser.Queries.GetUserPagingQuery;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [ApiController]
    [Authorize]
    public class UsersController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string CreateUser = "ユーザーを正常に登録しました。 ({0})";
            public static readonly string UpdateUser = "ユーザーを正常に更新しました。 ({0})";
            public static readonly string DeleteUser = "ユーザーを正常に削除しました。 ({0})";
            public static readonly string GetUser = "ユーザーを正常に取得しました。";
        }

        public UsersController(IWebHostEnvironment env, ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _env = env;
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpPost]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<CreateUserResultDto>> Create([FromBody] CreateUserCommand command)
        {
            var result = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CreateUser, command.UserName));
            return result;
        }

        [HttpPut]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<UpdateUserResultDto>> Update([FromBody] UpdateUserCommand command)
        {
            var result = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateUser, command.UserName));
            return result;
        }

        [HttpGet]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<UserDto>>> Get([FromQuery] GetUserPagingQuery query)
        {
            var result = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetUser));
            return result;
        }

        [HttpDelete]
        [Route("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<DeleteUserResultDto>> Delete(string id)
        {
            var result = await Mediator.Send(new DeleteUserCommand() { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeleteUser, id));
            return result;
        }
    }
}

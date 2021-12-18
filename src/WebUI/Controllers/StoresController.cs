using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.Stores.Commands.CreateStore;
using mrs.Application.Stores.Commands.DeleteStore;
using mrs.Application.Stores.Commands.UpdateStore;
using mrs.Application.Stores.Queries.GetStoreByUser;
using mrs.Application.Stores.Queries.GetStores;
using mrs.Application.Stores.Queries.GetStoresWithPagination;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class StoresController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string GetStoreByLoginUser = "店舗を正常に取得しました。";
            public static readonly string GetStores = "店舗一覧を正常に取得しました。";
            public static readonly string CreateStore = "店舗を正常に登録しました。({0})";
            public static readonly string UpdateStore = "店舗を正常に更新しました。({0})";
            public static readonly string DeleteStore = "店舗を正常に削除しました。({0})";
        }
        public StoresController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpGet]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<StoreDto>>> GetStoresWithPagination([FromQuery] GetStoresWithPaginationQuery query)
        {
            var stores = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetStores);
            return stores;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<StoresVm>> GetStores()
        {
            var stores = await Mediator.Send(new StoresQuery());
            return stores;
        }

        [HttpPost]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<int>> Create(CreateStoreCommand command)
        {
            var createdId = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CreateStore, createdId));
            return createdId;
        }

        [HttpPut("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> Update(int id, UpdateStoreCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateStore, command.Id));
            return NoContent();
        }

        [HttpPut("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> UpdateItemDetails(int id, UpdateStoreCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateStore, command.Id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, DeleteStoreCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeleteStore, id));
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<StoreByUserDto>> GetStoreByLoggedInUser()
        {
            var result = await Mediator.Send(new GetStoreByUserQuery());
            return result;
        }
    }
}

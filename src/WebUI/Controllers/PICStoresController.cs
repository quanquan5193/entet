using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.PICStores.Commands.CreatePICStore;
using mrs.Application.PICStores.Commands.DeletePICStore;
using mrs.Application.PICStores.Commands.UpdatePICStore;
using mrs.Application.PICStores.Queries.GetPICStoresWithPagination;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class PICStoresController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string GetPICStoresByLoginUser = "担当者一覧を正常に取得しました。";
            public static readonly string CreatePICStore = "担当者を正常に登録しました。({0})";
            public static readonly string DeletePICStore = "担当者を正常に削除しました。({0})";
            public static readonly string UpdatePICStore = "担当者を正常に更新しました。({0})";
        }

        public PICStoresController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpGet]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<PaginatedList<PICStoreDto>>> GetPICStores([FromQuery] GetPICStoresWithPaginationQuery query)
        {
            var result = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetPICStoresByLoginUser);
            return result;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreatePICStoreCommand command)
        {
            int createId;
            try
            {
                createId = await Mediator.Send(command);
            }
            catch (DbUpdateException ex)
            {
                var innerException = (SqlException)ex.InnerException;
                if (innerException != null && (innerException.Number == 2601 || innerException.Number == 2627))
                {
                    return Conflict();
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CreatePICStore, createId));
            return StatusCode(StatusCodes.Status201Created, createId);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] DeletePICStoreCommand command)
        {
            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeletePICStore, command.PICCode));
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdatePICStoreCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            try
            {
                await Mediator.Send(command);
            }
            catch (DbUpdateException ex)
            {
                var innerException = (SqlException)ex.InnerException;
                if (innerException != null && (innerException.Number == 2601 || innerException.Number == 2627))
                {
                    return Conflict();
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdatePICStore, id));
            return NoContent();
        }
    }
}

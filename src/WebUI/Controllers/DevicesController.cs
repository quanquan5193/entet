using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.Devices.Commands.CreateDevice;
using mrs.Application.Devices.Commands.DeleteDevice;
using mrs.Application.Devices.Commands.GetDevice;
using mrs.Application.Devices.Commands.UpdateDevice;
using mrs.Application.Devices.Queries.GetDevice;
using mrs.Application.Devices.Queries.SearchDevicesWithPagination;
using mrs.WebUI.Filters;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class DevicesController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string GetDevices = "端末一覧を正常に取得しました。";
            public static readonly string GetDevice = "端末を正常に取得しました。 ({0})";
            public static readonly string CreateDeviceSuccessfully = "端末を正常に登録しました。 ({0})";
            public static readonly string CreateDeviceFailed = "端末登録に失敗しました。 ({0})";
            public static readonly string UpdateDevice = "端末を正常に更新しました。 ({0})";
            public static readonly string DeleteDevice = "端末を正常に削除しました。 ({0})";
        }

        public DevicesController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<SearchDevicesWithPaginationDto>>> SearchDevicesWithPagination([FromQuery] SearchDevicesWithPaginationQuery query)
        {
            var listDevice = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetDevices);
            return listDevice;
        }

        [HttpGet]
        [CustomAuthorizeFilter(RoleLevel.Level_6, RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<GetDeviceDto>> GetDevice([FromQuery] GetDeviceCommand command)
        {
            var device = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetDevice, command.Id));
            return device;
        }

        [HttpPut]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<bool>> UpdateDevice([FromQuery] UpdateDeviceCommand command)
        {
            var result = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateDevice, command.Id));
            return result;
        }

        [HttpPost]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<int>> CreateDevice(CreateDeviceCommand command)
        {
            var createdId = await Mediator.Send(command);
            if (createdId == -1)
            {
                await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CreateDeviceFailed, command.DeviceCode));
            }

            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CreateDeviceSuccessfully, createdId));
            return createdId;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "9,10")]
        public async Task<ActionResult<bool>> DeleteDevice(int id)
        {
            var result = await Mediator.Send(new DeleteDeviceCommand() { Id = id });
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeleteDevice, id));
            return result;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<DeviceDto>> GetDeviceById([FromQuery] GetDeviceByIdQuery query)
        {
            var device = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetDevice, device.Id));
            return device;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult<DeviceDto>> GetDeviceByCode([FromQuery] GetDeviceByCodeQuery query)
        {
            var device = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetDevice, device.Id));
            return device;
        }

        [HttpPut("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_2, RoleLevel.Level_3)]
        public async Task<ActionResult> Update(int id, UpdateDeviceMobileCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateDevice, command.Id));
            return NoContent();
        }
    }
}

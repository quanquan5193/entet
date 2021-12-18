using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Helpers.AzureStorage;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Application.Companies.Commands.CreateCompany;
using mrs.Application.Companies.Commands.DeleteCompany;
using mrs.Application.Companies.Commands.GetDetailCompany;
using mrs.Application.Companies.Commands.GetDetailCompanyWithCode;
using mrs.Application.Companies.Commands.UpdateCompany;
using mrs.Application.Companies.Queries.GetCompanies;
using mrs.Application.Companies.Queries.GetCompaniesWithPagination;
using mrs.WebUI.Filters;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    [Authorize]
    public class CompaniesController : ApiControllerBase
    {
        private readonly IAzureStorageHelper _azureStorageHelpers;
        private static class LoggingMessage
        {
            public static readonly string GetCompaniesWithPagination = "ページングを利用する会社一覧を正常に取得しました。";
            public static readonly string CreateCompany = "会社を正常に新規登録しました。({0})";
            public static readonly string DeleteCompany = "会社を正常に削除しました。({0})";
            public static readonly string UpdateCompany = "会社を正常に更新しました。({0})";
            public static readonly string GetCompany = "会社を正常に取得しました。({0})";
        }

        public CompaniesController(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(currentUserService, identityService, configuration);
        }

        [HttpGet]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<PaginatedList<CompanyDto>>> GetCompaniesWithPagination([FromQuery] GetCompaniesWithPaginationQuery query)
        {
            var companies = await Mediator.Send(query);
            await _azureStorageHelpers.SaveLogToBlob(LoggingMessage.GetCompaniesWithPagination);
            return companies;
        }

        [HttpGet("[action]")]
        [CustomAuthorizeFilter(RoleLevel.Level_8, RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<CompaniesVm>> GetCompanies()
        {
            var companies = await Mediator.Send(new CompaniesQuery());
            return companies;
        }

        [HttpPost]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult<int>> Create(CreateCompanyCommand command)
        {
            var createdId = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.CreateCompany, createdId));
            return createdId;
        }

        [HttpPut("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> Update(int id, UpdateCompanyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateCompany, id));
            return NoContent();
        }

        [HttpPut("[action]")]
        public async Task<ActionResult> UpdateItemDetails(int id, UpdateCompanyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.UpdateCompany, id));
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CustomAuthorizeFilter(RoleLevel.Level_9, RoleLevel.Level_10)]
        public async Task<ActionResult> Delete(int id, DeleteCompanyCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.DeleteCompany, id));
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<GetDetailCompanyDto>> GetDetailCompanyCommand([FromQuery] GetDetailCompanyCommand command)
        {
            var company = await Mediator.Send(command);
            await _azureStorageHelpers.SaveLogToBlob(string.Format(LoggingMessage.GetCompany, command.Id));
            return company;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<GetDetailCompanyWithCodeDto>> GetDetailCompanyWithCodeCommand([FromQuery] GetDetailCompanyWithCodeCommand command)
        {
            var company = await Mediator.Send(command);
            return company;
        }
    }
}

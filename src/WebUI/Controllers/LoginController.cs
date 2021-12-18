using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.Common.Helpers.AzureStorage;
using System.Threading.Tasks;

namespace mrs.WebUI.Controllers
{
    public class LoginController : ApiControllerBase
    {
        private const string LoginSuccessfulMessage = "ログインに成功しました。";
        private readonly IAzureStorageHelper _azureStorageHelpers;
        public LoginController(IConfiguration configuration)
        {
            _azureStorageHelpers = new AzureStorageHelper(null, null, configuration);
        }
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Create(GetTokenQuery query)
        {
            var loginUser = await Mediator.Send(query);
            if(loginUser != null)
            {
                string userName = loginUser.User.UserName;
                await _azureStorageHelpers.SaveLogAfterLoginToBlob(userName, LoginSuccessfulMessage);
            }    
            
            return Ok(loginUser);
        }
    }
}
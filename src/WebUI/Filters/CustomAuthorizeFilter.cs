
using Microsoft.AspNetCore.Mvc.Filters;
using mrs.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace mrs.WebUI.Filters
{
    public class CustomAuthorizeFilter : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;
        public CustomAuthorizeFilter(params string[] roles)
        {
            this._allowedRoles = roles;
        }
        public override void OnActionExecuting(ActionExecutingContext actionExecutingContext)
        {
            var identityService = actionExecutingContext.HttpContext.RequestServices.GetService<IIdentityService>();
            var currentUserId = actionExecutingContext.HttpContext.RequestServices.GetService<ICurrentUserService>();

            foreach (var role in this._allowedRoles)
            {
                var roles =  identityService.GetRolesUserAsync(currentUserId?.UserId).GetAwaiter().GetResult();
                if(roles.Contains(role))
                {
                    base.OnActionExecuting(actionExecutingContext);
                    return;
                }    
            }
            actionExecutingContext.Result = new ForbidResult();            
        }
    }
}


using System.Collections.Generic;

namespace mrs.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string CreateJwtSecurityToken(ApplicationUser.Queries.GetToken.ApplicationUserDto user, IList<string> roles);
    }
}

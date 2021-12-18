using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.ApplicationUser.Queries.GetUserPagingQuery;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mrs.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);

        Task<bool> IsInRoleAsync(string userId, string role);

        Task<bool> AuthorizeAsync(string userId, string policyName);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string fullName = "", string rolePermission = "", int? storeId = null);

        Task<(Result Result, string UserId)> UpdateUserAsync(string id, string userName, string password, string fullName, string rolePermission, int storeId);

        Task<(bool result, string messageCode)> DeleteUserLogicAsync(string id);

        Task<Result> DeleteUserAsync(string userId);

        Task<ApplicationUserDto> CheckUserPassword(string userName, string password);

        ApplicationUserDto GetUserDto(string userId);

        Task<IList<string>> GetRolesUserAsync(string userId);

        Task<Store> GetStoreAsync(string userId);
        Task<IEnumerable<int>> GetStoreIdsAsync(string userId, string roleLevel);

        Task<IEnumerable<ApplicationUserDto>> GetUsersId(string userName);

        Task<IEnumerable<ApplicationUserDto>> GetAllUsers();

        Task<PaginatedList<UserDto>> GetUserPaging(string companyCode, string companyName, string loginID, string fullName, int page, int size, string sortBy, string sortDimension);

        Task<bool> IsUserNameExist(string userName);
        Task<bool> IsCompanyStoreNotChange(string id, string companyCode, string storeCode);
        Task<bool> IsStoreExistUser(int storeId);
        List<string> GetUsersByStoreId(int? storeId);
        IQueryable<CardDto> GetCardsWithUserQueryable();
    }
}

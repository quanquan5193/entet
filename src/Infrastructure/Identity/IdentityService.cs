using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.ApplicationUser.Queries.GetUserPagingQuery;
using mrs.Application.Cards.Queries.GetCards;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.ExpressionExtension;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mrs.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IAuthorizationService _authorizationService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly IApplicationDbContext _context;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService,
            IConfiguration configuration,
            IMapper mapper,
            IDateTime dateTime,
            ICurrentUserService currentUserService,
            IApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
            _configuration = configuration;
            _mapper = mapper;
            _dateTime = dateTime;
            _currentUserService = currentUserService;
            _context = context;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user.UserName;
        }

        public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string fullName = "", string rolePermission = "", int? storeId = null)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                StoreId = storeId,
                FullName = fullName,
                CreatedAt = _dateTime.Now
            };

            var logedInUserRole = _currentUserService.RoleLevel;

            if (logedInUserRole != null && int.Parse(logedInUserRole) < int.Parse(rolePermission))
            {
                return (Result.Failure(new List<string>() { "createPermissionForbidden" }), user.Id);
            }

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return (Result.Failure(new List<string>() { "createFailLoginID" }), user.Id);
            }

            if (!string.IsNullOrWhiteSpace(rolePermission))
            {
                await _userManager.AddToRolesAsync(user, new[] { rolePermission });
            }

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<(Result Result, string UserId)> UpdateUserAsync(string id, string userName, string password, string fullName, string rolePermission, int storeId)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return (Result.Failure(new List<string>() { "updateFail02" }), user.Id);
            }

            var logedInUserRole = _currentUserService.RoleLevel;

            if (logedInUserRole != null && int.Parse(logedInUserRole) < int.Parse(rolePermission))
            {                
                return (Result.Failure(new List<string>() { "updatePermissionForbidden" }), user.Id);
            }

            user.UserName = userName;
            user.Email = userName;
            user.StoreId = storeId;
            user.FullName = fullName;

            var result = await _userManager.UpdateAsync(user);

            if (!string.IsNullOrWhiteSpace(password))
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, password);
            }

            if (!string.IsNullOrWhiteSpace(rolePermission))
            {
                var roleResult = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

                if (roleResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, new[] { rolePermission });
                }
            }

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<IList<string>> GetRolesUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }

        public async Task<ApplicationUserDto> CheckUserPassword(string email, string password)
        {
            List<ApplicationUser> users = await _userManager.Users.Include(x => x.Store).Where(u => u.UserName == email && !u.IsDeleted).ToListAsync();
            ApplicationUser user = users.FirstOrDefault(n => n.UserName.Equals(email));

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                int webSesstionTimeout;

                if (!int.TryParse(_configuration["ApplicationSetting:WebSessionTimeout"], out webSesstionTimeout))
                {
                    webSesstionTimeout = 0;
                }

                int appSesstionTimeout;

                if (!int.TryParse(_configuration["ApplicationSetting:AppSessionTimeout"], out appSesstionTimeout))
                {
                    appSesstionTimeout = 0;
                }

                int distanceOfAppLocking;

                if (!int.TryParse(_configuration["ApplicationSetting:DistanceOfAppLocking"], out distanceOfAppLocking))
                {
                    distanceOfAppLocking = 0;
                }

                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                ApplicationRole role = await _roleManager.Roles.FirstOrDefaultAsync(x => userRoles.Contains(x.Name));

                if (role == null)
                    throw new NotFoundException(string.Format("Role Not Found for ({0})", email));

                return new ApplicationUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Id = user.Id,
                    FullName = user.FullName,
                    StoreId = user.StoreId,
                    CompanyId = user.Store?.CompanyId,
                    WebSessionTimeout = webSesstionTimeout == 0 ? null : webSesstionTimeout,
                    AppSessionTimeout = appSesstionTimeout == 0 ? null : appSesstionTimeout,
                    RolePermission = JsonConvert.DeserializeObject(role.RolePermission),
                    DistanceOfAppLocking = distanceOfAppLocking == 0 ? null : distanceOfAppLocking
                };
            }

            return null;
        }

        public Task<IEnumerable<ApplicationUserDto>> GetUsersId(string userName)
        {
            IQueryable<ApplicationUserDto> user = _userManager.Users.Where(u => u.UserName.Contains(userName)).Select(u => new ApplicationUserDto
            {
                UserName = u.UserName,
                Email = u.Email,
                Id = u.Id
            });
            return Task.FromResult(user.AsEnumerable());
        }

        public Task<IEnumerable<ApplicationUserDto>> GetAllUsers()
        {
            IQueryable<ApplicationUserDto> user = _userManager.Users.Select(u => new ApplicationUserDto
            {
                UserName = u.UserName,
                Email = u.Email,
                Id = u.Id
            });
            return Task.FromResult(user.AsEnumerable());
        }

        public async Task<Store> GetStoreAsync(string userId)
        {
            return (await _userManager.Users.Include(x => x.Store).Include(x => x.Store.Company).SingleOrDefaultAsync(x => x.Id.Equals(userId))).Store;
        }

        public async Task<IEnumerable<int>> GetStoreIdsAsync(string userId, string roleLevel)
        {
            var storeIds = new List<int>();
            var user = await _userManager.Users.Include(x => x.Store).Include(x => x.Store.Company)
                .Include(x => x.Store.Company.Stores).FirstOrDefaultAsync(c => c.Id == userId);
            if (user?.Store != null)
            {
                var store = user.Store;
                switch (roleLevel)
                {
                    case RoleLevel.Level_2:
                        storeIds.Add(store.Id);
                        break;
                    case RoleLevel.Level_3:
                        var userStores = store.Company.Stores;
                        if (userStores != null && userStores.Any())
                        {
                            storeIds.AddRange(store.Company.Stores.Select(s => s.Id));
                        }

                        break;
                }
            }

            return storeIds;
        }

        public ApplicationUserDto GetUserDto(string userId)
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                return new ApplicationUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Id = user.Id,
                    StoreId = user.StoreId,
                    FullName = user.FullName
                };
            }

            return null;
        }

        public async Task<PaginatedList<UserDto>> GetUserPaging(string companyCode, string companyName, string loginID, string fullName, int page, int size, string sortBy, string sortDimension)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.Include(c => c.UserRoles).ThenInclude(x => x.Role).Include(x => x.Store).Include(x => x.Store.Company).Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(companyCode))
            {
                query = query.Where(x => x.Store.Company.CompanyCode.Contains(companyCode));
            }

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                query = query.Where(x => x.Store.Company.CompanyName.Contains(companyName));
            }

            if (!string.IsNullOrWhiteSpace(loginID))
            {
                query = query.Where(x => x.UserName.Contains(loginID));
            }

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(x => x.FullName.Contains(fullName));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("Permission"))
                {
                    query = sortDimension.ToLowerInvariant().Equals("asc")
                          ? query.OrderBy(n => n.UserRoles.FirstOrDefault().Role.Name)
                          : query.OrderByDescending(n => n.UserRoles.FirstOrDefault().Role.Name);
                } else if (sortBy.Equals("NormalizedCompanyName"))
                {
                    query = sortDimension.ToLowerInvariant().Equals("asc")
                          ? query.OrderBy(n => n.Store.Company.NormalizedCompanyName)
                          : query.OrderByDescending(n => n.Store.Company.NormalizedCompanyName);
                } else if (sortBy.Equals("NormalizedStoreName"))
                {
                    query = sortDimension.ToLowerInvariant().Equals("asc")
                          ? query.OrderBy(n => n.Store.NormalizedStoreName)
                          : query.OrderByDescending(n => n.Store.NormalizedStoreName);
                } else
                {
                    query = query.OrderByCustom(sortBy + " " + sortDimension.ToUpper());
                }
            }

            query = query.Where(x => x.Store.IsActive && x.Store.Company.IsActive);

            #region check role
            IList<string> loggedInUserRoles = await GetRolesUserAsync(_currentUserService.UserId);
            Store loggedInUserStore = await GetStoreAsync(_currentUserService.UserId);

            if (loggedInUserRoles.Contains(RoleLevel.Level_6))
            {
                query = query.Where(x => x.Store.CompanyId == (loggedInUserStore == null ? 0 : loggedInUserStore.CompanyId));
            }
            #endregion

            int total = query.Count();

            var users = await query.Skip((page - 1) * size).Take(size).ToArrayAsync();

            PaginatedList<UserDto> userDtos = new PaginatedList<UserDto>(users.Select(x => new UserDto()
            {
                Id = x.Id,
                Permission = x.UserRoles.FirstOrDefault().Role.Name,
                CompanyName = x.Store.Company.CompanyName,
                NormalizedCompanyName = x.Store.Company.NormalizedCompanyName,
                CompanyCode = x.Store.Company.CompanyCode,
                CompanyId = x.Store.CompanyId,
                CreatedAt = x.CreatedAt,
                FullName = x.FullName,
                UserName = x.UserName,
                StoreName = x.Store.StoreName,
                NormalizedStoreName = x.Store.NormalizedStoreName,
                StoreCode = x.Store.StoreCode
            }).ToList(), total, page, size);

            return userDtos;
        }

        public async Task<bool> IsUserNameExist(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> IsCompanyStoreNotChange(string id, string companyCode, string storeCode)
        {
            return await _userManager.Users.AnyAsync(x => x.Id.Equals(id) && x.Store.StoreCode == storeCode && x.Store.Company.CompanyCode == companyCode);
        }

        public async Task<(bool result, string messageCode)> DeleteUserLogicAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            string messageCode = "";

            var logedInUserRole = _currentUserService.RoleLevel;

            if (logedInUserRole != null && int.Parse(logedInUserRole) < int.Parse((await _userManager.GetRolesAsync(user)).FirstOrDefault()))
            {
                messageCode = "deletePermissionForbidden";
                return (false, messageCode);
            }

            if (user.IsDeleted)
            {
                messageCode = "deleteFail01";
                return (false, messageCode);
            }

            var userHavePicstore = _context.PICStores.Where(x => x.CreatedBy.Equals(user.Id) && !x.IsDeleted);

            if (userHavePicstore.Any())
            {
                foreach (var picStore in userHavePicstore)
                {
                    picStore.IsDeleted = true;
                }
                _context.PICStores.UpdateRange(userHavePicstore);
            }

            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);
            messageCode = "deleteSuccess";

            if (!result.Succeeded)
            {
                messageCode = "deleteFail01";
            }

            return (result.Succeeded, messageCode);
        }

        public async Task<bool> IsStoreExistUser(int storeId)
        {
            return await _userManager.Users.AnyAsync(x => x.StoreId == storeId && !x.IsDeleted);
        }
        public List<string> GetUsersByStoreId(int? storeId)
        {
            var userIds = _userManager.Users.Where(u => u.StoreId == storeId).Select(x => x.Id).ToList();
            return userIds;
        }

        public IQueryable<CardDto> GetCardsWithUserQueryable()
        {
            var query = from card in _context.Cards
                        join user in _userManager.Users
                        on card.CreatedBy
                        equals user.Id
                        join userUpdated in _userManager.Users
                        on card.UpdatedBy
                        equals userUpdated.Id into cardUserUpdate
                        from x in cardUserUpdate.DefaultIfEmpty()
                        join store in _context.Stores
                        on card.StoreId
                        equals store.Id
                        join company in _context.Companies
                        on card.CompanyId
                        equals company.Id
                        where !card.IsDeleted
                        select new CardDto {
                            Id = card.Id,
                            MemberNo = card.MemberNo,
                            ExpiredAt = card.ExpiredAt,
                            Status = card.Status,
                            Point = card.Point,
                            Store = store,
                            StoreId = store.Id,
                            Company = company,
                            CompanyId = company.Id,
                            CreatedAt = card.CreatedAt,
                            CreatedByName = user.UserName,
                            UpdatedAt = card.UpdatedAt,
                            UpdatedByName = x.UserName,
                            CreatedBy = card.CreatedBy
                        };
            return query;
        }
    }
}

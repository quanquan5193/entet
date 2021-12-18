using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Helpers;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ApplicationUser.Queries.GetToken
{
    public class GetTokenQuery : IRequest<LoginResponse>
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsWebApp { get; set; } = false;

        public int? DeviceId { get; set; }
    }

    public class GetTokenQueryHandler : IRequestHandler<GetTokenQuery, LoginResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IApplicationDbContext _context;

        public GetTokenQueryHandler(IIdentityService identityService, ITokenService tokenService, IApplicationDbContext context)
        {
            _context = context;
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(GetTokenQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.CheckUserPassword(request.Email, request.Password);
            if (user == null)
                throw new NotFoundException("User", request.Email);

            var roles = await _identityService.GetRolesUserAsync(user.Id);
            if (roles == null)
                throw new NotFoundException(string.Format("Role Not Found for ({0})", request.Email));
            if (request.IsWebApp)
            {
                if (roles.Contains(RoleLevel.Level_2)
                    || roles.Contains(RoleLevel.Level_3)
                    || roles.Contains(RoleLevel.Level_1))
                {
                    throw new NotFoundException(string.Format("No Permission for ({0})", request.Email));
                }
            }
            else
            {
                if (roles.Contains(RoleLevel.Level_6)
                    || roles.Contains(RoleLevel.Level_7)
                    || roles.Contains(RoleLevel.Level_8)
                    || roles.Contains(RoleLevel.Level_9)
                    || roles.Contains(RoleLevel.Level_10)
                    || roles.Contains(RoleLevel.Level_1))
                {
                    throw new NotFoundException(string.Format("No Permission for ({0})", request.Email));
                }

                if (request.DeviceId != null)
                {
                    if (roles.Contains(RoleLevel.Level_2))
                    {
                        if (!(await _context.Devices.AnyAsync(x => x.Id == request.DeviceId && x.StoreId == user.StoreId)))
                            throw new NotFoundException(string.Format("No Permission for ({0})", request.Email));
                    }
                    else
                    {
                        if (!(await _context.Devices.AnyAsync(x => x.Id == request.DeviceId && x.Store.CompanyId == user.CompanyId)))
                            throw new NotFoundException(string.Format("No Permission for ({0})", request.Email));
                    }
                }
            }

            return new LoginResponse
            {
                User = user,
                Token = _tokenService.CreateJwtSecurityToken(user, roles)
            };
        }

    }
}

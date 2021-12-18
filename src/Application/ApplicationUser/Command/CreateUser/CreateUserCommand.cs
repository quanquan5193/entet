using MediatR;
using Microsoft.EntityFrameworkCore;
using mrs.Application.Common.Exceptions;
using mrs.Application.Common.Interfaces;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ApplicationUser.Command.CreateUser
{
    public class CreateUserCommand : IRequest<CreateUserResultDto>
    {
        public string CompanyCode { get; set; }
        public string StoreCode { get; set; }
        public string Permission { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }

    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResultDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IApplicationDbContext _context;

        public CreateUserCommandHandler(IIdentityService identityService, IApplicationDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        public async Task<CreateUserResultDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            Store store = _context.Stores.Include(n => n.Company).FirstOrDefault(x => x.StoreCode.Equals(request.StoreCode) && x.Company.CompanyCode.Equals(request.CompanyCode));

            if (store == null)
                throw new NotFoundException("Store", request.StoreCode);

            (Common.Models.Result Result, string UserId) result = await _identityService.CreateUserAsync(request.UserName, request.Password, request.FullName, request.Permission, store.Id);

            if (!result.Result.Succeeded)
            {
                return new CreateUserResultDto() { IsSuccess = result.Result.Succeeded, MessageCode = result.Result.Errors.FirstOrDefault() };
            }

            return new CreateUserResultDto() { IsSuccess = result.Result.Succeeded, MessageCode = "createSuccess" };
        }
    }
}

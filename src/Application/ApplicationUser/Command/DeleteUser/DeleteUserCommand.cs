using MediatR;
using mrs.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ApplicationUser.Command.DeleteUser
{
    public class DeleteUserCommand : IRequest<DeleteUserResultDto>
    {
        public string Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResultDto>
    {
        private readonly IIdentityService _identityService;
        public DeleteUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<DeleteUserResultDto> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.DeleteUserLogicAsync(request.Id);
            DeleteUserResultDto rtnModel = new DeleteUserResultDto() { MessageCode = result.messageCode, IsSuccess = result.result };
            return rtnModel;
        }
    }
}

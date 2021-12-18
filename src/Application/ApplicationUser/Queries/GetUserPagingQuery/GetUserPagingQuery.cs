using MediatR;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.ApplicationUser.Queries.GetUserPagingQuery
{
    public class GetUserPagingQuery : IRequest<PaginatedList<UserDto>>
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDimension { get; set; } = "DESC";
    }

    public class UserDto
    {
        public string UserName { get; set; }
        public string Permission { get; set; }
        public string CompanyName { get; set; }
        public string NormalizedCompanyName { get; set; }
        public string CompanyCode { get; set; }
        public int CompanyId { get; set; }
        public string StoreName { get; set; }
        public string NormalizedStoreName { get; set; }
        public string StoreCode { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Id { get; set; }
    }
    

    public class GetUserPaginQueryHandler : IRequestHandler<GetUserPagingQuery, PaginatedList<UserDto>>
    {
        private readonly IIdentityService _identityService;
        public GetUserPaginQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }


        public async Task<PaginatedList<UserDto>> Handle(GetUserPagingQuery request, CancellationToken cancellationToken)
        {
            return await _identityService.GetUserPaging(request.CompanyCode, request.CompanyName, request.UserName, request.FullName,request.Page,request.Size, request.SortBy, request.SortDimension);
        }
    }
}

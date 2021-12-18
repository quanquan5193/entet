using mrs.Application.Common.Mappings;

namespace mrs.Application.ApplicationUser.Queries.GetToken
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public int? StoreId { get; set; }

        public int? WebSessionTimeout { get; set; }

        public int? AppSessionTimeout { get; set; }

        public int? DistanceOfAppLocking { get; set; }

        public object RolePermission { get; set; }

        public int? CompanyId { get; set; }

        public string FullName { get; set; }
    }
}

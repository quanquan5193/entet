using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name)
        {
        }
        public ApplicationRole(string name, string description,string rolePermission) : base(name)
        {
            Description = description;
            RolePermission = rolePermission;
        }

        public string Description { get; set; }
        public string RolePermission { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}

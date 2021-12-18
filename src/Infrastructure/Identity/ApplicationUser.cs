using Microsoft.AspNetCore.Identity;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;

namespace mrs.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public int? StoreId { get; set; }

        public Store Store { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string DeletedBy { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}

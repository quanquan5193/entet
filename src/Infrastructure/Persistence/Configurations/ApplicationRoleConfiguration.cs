using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mrs.Infrastructure.Identity;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.RolePermission)
                .HasMaxLength(4000);
        }
    }
}

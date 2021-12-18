using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mrs.Infrastructure.Identity;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(t => t.FullName)
                .HasMaxLength(500);

            builder.Property(t => t.PasswordHash)
                .HasMaxLength(4000);

            builder.Property(t => t.DeletedBy)
                .HasMaxLength(450);
        }
    }
}

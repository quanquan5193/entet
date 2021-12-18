using mrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.Property(t => t.CompanyCode)
                .HasMaxLength(500);

            builder.Property(t => t.CompanyName)
                .HasMaxLength(500);

            builder.Property(t => t.NormalizedCompanyName)
                .HasMaxLength(250);

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.DeletedBy)
                .HasMaxLength(450);
        }
    }
}
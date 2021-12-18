using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mrs.Domain.Entities;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class PICStoreConfiguration : IEntityTypeConfiguration<PICStore>
    {
        public void Configure(EntityTypeBuilder<PICStore> builder)
        {
            builder.Property(x => x.PICCode)
                .HasMaxLength(450);

            builder.Property(x => x.PICName)
                .HasMaxLength(500);

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.DeletedBy)
                .HasMaxLength(450);
        }
    }
}

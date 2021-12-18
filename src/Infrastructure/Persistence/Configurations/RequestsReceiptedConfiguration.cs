using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mrs.Domain.Entities;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class RequestsReceiptedConfiguration : IEntityTypeConfiguration<RequestsReceipted>
    {
        public void Configure(EntityTypeBuilder<RequestsReceipted> builder)
        {
            builder.HasOne(e => e.RequestType)
                .WithMany(e => e.RequestsReceipteds)
                .HasForeignKey(e => e.ReceiptedTypeId);

            builder.Property(x => x.RequestCode)
                .HasMaxLength(450);

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.DeletedBy)
                .HasMaxLength(450);
        }
    }
}

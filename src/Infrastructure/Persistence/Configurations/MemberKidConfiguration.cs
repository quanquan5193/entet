using mrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class MemberKidConfiguration : IEntityTypeConfiguration<MemberKid>
    {
        public void Configure(EntityTypeBuilder<MemberKid> builder)
        {
            builder.Property(t => t.ParentFirstName)
                .HasMaxLength(500);

            builder.Property(t => t.ParentLastName)
                .HasMaxLength(500);

            builder.Property(t => t.ParentFuriganaFirstName)
                .HasMaxLength(500);

            builder.Property(t => t.ParentFuriganaLastName)
                .HasMaxLength(500);

            builder.Property(t => t.FirstName)
                .HasMaxLength(500);

            builder.Property(t => t.LastName)
                .HasMaxLength(500);

            builder.Property(t => t.FuriganaFirstName)
                .HasMaxLength(500);

            builder.Property(t => t.FuriganaLastName)
                .HasMaxLength(500);

            builder.Property(t => t.Remark)
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
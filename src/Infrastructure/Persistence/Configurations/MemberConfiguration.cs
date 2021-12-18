using mrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.HasOne(e => e.PICStore)
                .WithMany(e => e.Members)
                .HasForeignKey(e => e.PICStoreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(t => t.ZipcodeId)
                .HasMaxLength(7).IsFixedLength();

            builder.Property(t => t.MemberNo)
                .HasMaxLength(10);

            builder.Property(t => t.OldMemberNo)
                 .HasMaxLength(10);

            builder.Property(t => t.FirstName)
                .HasMaxLength(500);

            builder.Property(t => t.LastName)
                .HasMaxLength(4000);


            builder.Property(t => t.FuriganaFirstName)
                .HasMaxLength(4000);

            builder.Property(t => t.FuriganaLastName)
                 .HasMaxLength(4000);

            builder.Property(t => t.FixedPhone)
                .HasMaxLength(4000);

            builder.Property(t => t.MobilePhone)
                .HasMaxLength(4000);

            builder.Property(t => t.Email)
                .HasMaxLength(4000);

            builder.Property(t => t.Remark)
                .HasMaxLength(500);

            builder.Property(t => t.Province)
                .HasMaxLength(4000);

            builder.Property(t => t.District)
                .HasMaxLength(4000);

            builder.Property(t => t.Street)
                .HasMaxLength(4000);

            builder.Property(t => t.BuildingName)
                .HasMaxLength(4000);

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.UpdatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.DeletedBy)
                .HasMaxLength(450);
        }
    }
}
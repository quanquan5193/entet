using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Infrastructure.Persistence.Configurations
{
    public class ZipcodeConfiguration : IEntityTypeConfiguration<ZipCode>
    {
        public void Configure(EntityTypeBuilder<ZipCode> builder)
        {
            builder.HasKey(t => t.Zipcode);

            builder.Property(t => t.Zipcode)
                .HasMaxLength(7).IsFixedLength()
                .IsRequired();

            builder.Property(t => t.Province)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(t => t.District)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(t => t.Street)
                .HasMaxLength(500);

            builder.Property(t => t.BuildingName)
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

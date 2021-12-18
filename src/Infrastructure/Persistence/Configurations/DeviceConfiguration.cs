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
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.Property(x => x.DeviceCode)
                .HasMaxLength(450);

            builder.Property(x => x.Lat)
                .HasMaxLength(500);

            builder.Property(x => x.Long)
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

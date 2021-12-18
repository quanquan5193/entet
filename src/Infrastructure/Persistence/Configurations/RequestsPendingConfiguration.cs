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
    public class RequestsPendingConfiguration : IEntityTypeConfiguration<RequestsPending>
    {
        public void Configure(EntityTypeBuilder<RequestsPending> builder)
        {
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

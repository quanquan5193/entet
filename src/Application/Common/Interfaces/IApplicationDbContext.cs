using mrs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace mrs.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Store> Stores { get; set; }

        DbSet<Company> Companies { get; set; }

        DbSet<Member> Members { get; set; }

        DbSet<Card> Cards { get; set; }

        DbSet<MemberKid> MemberKids { get; set; }

        DbSet<ZipCode> ZipCodes { get; set; }

        DbSet<RequestsPending> RequestsPendings { get; set; }

        DbSet<RequestsReceipted> RequestsReceipteds { get; set; }

        DbSet<Device> Devices { get; set; }

        DbSet<PICStore> PICStores { get; set; }

        DbSet<RequestType> RequestTypes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

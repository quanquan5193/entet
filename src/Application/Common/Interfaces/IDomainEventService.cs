using mrs.Domain.Common;
using System.Threading.Tasks;

namespace mrs.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}

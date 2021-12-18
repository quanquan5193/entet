using mrs.Domain.Common;
using System.Collections.Generic;

namespace mrs.Domain.Entities
{
    public class Store : AuditableEntity
    {
        public int Id { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public int CompanyId { get; set; }
        public string NormalizedStoreName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public IList<Card> Cards { get; private set; } = new List<Card>();
        public IList<Device> Devices { get; private set; } = new List<Device>();
        public IList<RequestsPending> RequestsPendings { get; set; }
        public IList<RequestsReceipted> RequestsReceipteds { get; set; }
        public Company Company { get; set; }      
    }
}

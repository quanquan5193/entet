using mrs.Domain.Common;

namespace mrs.Domain.Entities
{
    public class Device : AuditableEntity
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; }
        public int StoreId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAutoLock { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public Store Store { get; set; }
    }
}

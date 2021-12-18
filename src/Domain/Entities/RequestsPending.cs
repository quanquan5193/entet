using mrs.Domain.Common;
using System;

namespace mrs.Domain.Entities
{
    public class RequestsPending : AuditableEntity
    {
        public int Id { get; set; }
        public DateTime ReceiptedDatetime { get; set; }
        public string RequestCode { get; set; }
        public int? DeviceId { get; set; }
        public int? MemberId { get; set; }
        public int? StoreId { get; set; }
        public Store Store { get; set; }
        public Device Device { get; set; }
        public Member Member { get; set; }
    }
}

using mrs.Domain.Common;
using mrs.Domain.Enums;
using System;

namespace mrs.Domain.Entities
{
    public class RequestsReceipted : AuditableEntity
    {
        public int Id { get; set; }
        public int? CardId { get; set; }
        public DateTime ReceiptedDatetime { get; set; }
        public int ReceiptedTypeId { get; set; }
        public int? DeviceId { get; set; }
        public int? MemberId { get; set; }
        public string RequestCode { get; set; }
        public int? StoreId { get; set; }
        public RequestTypeDetail? ReceiptedTypeDetail { get; set; }
        public Store Store { get; set; }
        public Device Device { get; set; }
        public Member Member { get; set; }
        public RequestType RequestType { get; set; }
    }
}

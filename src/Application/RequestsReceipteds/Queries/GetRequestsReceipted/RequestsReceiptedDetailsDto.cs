using mrs.Domain.Entities;
using System;
using mrs.Application.Common.Mappings;

namespace mrs.Application.RequestsReceipteds.Queries.GetRequestsReceipted
{
    public class RequestsReceiptedDetailsDto : IMapFrom<RequestsReceipted>
    {
        public int Id { get; set; }
        public string RequestCode { get; set; }
        public int? StoreId { get; set; }
        public DateTime ReceiptedDatetime { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public RequestType RequestType { get; set; }
        public Device Device { get; set; }
        public Member Member { get; set; }
        public Store Store { get; set; }
    }
}

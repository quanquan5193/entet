using System;
using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;

namespace mrs.Application.RequestsReceipteds.Queries.GetRequestsReceiptedsWithPagination
{
    public class RequestsReceiptedDto : IMapFrom<RequestsReceipted>
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public DateTime ReceiptedDatetime { get; set; }
        public string RequestTypeName { get; set; }
        public string MemberNo { get; set; }
        public string MemberName { get; set; }
        public string Store { get; set; }
        public string NormalizedStoreName { get; set; }
        public string Company { get; set; }
        public string NormalizedCompanyName { get; set; }
        public string PicName { get; set; }
        public string Remark { get; set; }
    }
}

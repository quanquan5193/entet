using mrs.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Domain.Entities
{
    public class RequestType : AuditableEntity
    {
        public int Id { get; set; }

        public string RequestTypeCode { get; set; }

        public string RequestTypeName { get; set; }

        public IList<RequestsReceipted> RequestsReceipteds { get; private set; } = new List<RequestsReceipted>();
    }
}

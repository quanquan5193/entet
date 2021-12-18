using mrs.Application.PICStores.Queries.GetPICStoresWithPagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.ReferPrepaidCard.Commands.GetPrepaidCardDetail
{
    public class PrepaidCardDetailDto
    {
        public int Id { get; set; }

        public string CustomerNo { get; set; }

        public DateTime? RegisteredDate { get; set; }

        public DateTime? ExpiratedAt { get; set; }

        public string Status { get; set; }

        public string RequestCode { get; set; }

        public DateTime? ReceiptedDatetime { get; set; }

        public string RequestType { get; set; }

        public PICStoreDto? PicStoreDto { get; set; }
        public int? PicStore { get; set; }

        public string Remark { get; set; }
    }
}

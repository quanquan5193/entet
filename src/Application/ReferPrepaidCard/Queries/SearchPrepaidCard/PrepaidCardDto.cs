using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.ReferPrepaidCard.Queries.SearchPrepaidCard
{
    public class PrepaidCardDto
    {
        public int Id { get; set; }

        public int No { get; set; }

        public string MemberNo { get; set; }

        public DateTime? ReceiptedDatetime { get; set; }

        public string Status { get; set; }

        public string RequestType { get; set; }

        public string PICStore { get; set; }

        public string Remark { get; set; }
    }
}

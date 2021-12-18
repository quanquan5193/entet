using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Queries.SearchPendingRequestListWithPagination
{
    public class PendingRequestListDto
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string RequestType { get; set; }

        public string MemberNo { get; set; }

        public string OldMemberNo { get; set; }

        public string CustomerName { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }

        public string PICName { get; set; }

        public string Remark { get; set; }
    }
}

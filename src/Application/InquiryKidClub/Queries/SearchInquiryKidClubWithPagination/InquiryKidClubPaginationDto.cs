using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.InquiryKidClub.Queries.SearchInquiryKidClubWithPagination
{
    public class InquiryKidClubDto
    {
        public int Id { get; set; }

        public int No { get; set; }

        public string MemberNo { get; set; }

        public DateTime? ReceiptedDatetime { get; set; }

        public string GuardianName { get; set; }

        public string KidName { get; set; }

        public string PICName { get; set; }

        public string Remark { get; set; }
    }
}

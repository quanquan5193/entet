using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.InquiryKidClub.Commands.GetInquiryKidClubDetail
{
    public class InquiryKidClubDetailDto
    {
        public int Id { get; set; }

        public DateTime? ReceiptedDatetime { get; set; }

        public int RelationshipMember { get; set; }

        public string GuardianFirstName { get; set; }

        public string GuardianLastName { get; set; }

        public string GuardianFuriganaFirstName { get; set; }

        public string GuardianFuriganaLastName { get; set; }

        public bool IsLivingWithParent { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FuriganaFirstName { get; set; }

        public string FuriganaLastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Remarks { get; set; }

        public string KidGender { get; set; }
    }
}

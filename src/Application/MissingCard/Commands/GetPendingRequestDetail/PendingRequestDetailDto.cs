using mrs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.MissingCard.Commands.GetPendingRequestDetail
{
    public class PendingRequestDetailDto
    {
        public int Id { get; set; }

        public string MemberNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public SexType Sex { get; set; }
        public string ZipcodeId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public bool IsRegisterAdvertisement { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FixedPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string BuildingName { get; set; }
        public DateTime ReceiptedDatetime { get; set; }
        public int? StoreId { get; set; }
        public string DeviceCode { get; set; }
        public int? PICStoreId { get; set; }
    }
}

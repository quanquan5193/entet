using mrs.Domain.Common;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;

namespace mrs.Domain.Entities
{
    public class Member : AuditableEntity
    {
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public string OldMemberNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public bool IsNetMember { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FixedPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public int? PICStoreId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string BuildingName { get; set; }
        public bool IsRegisterAdvertisement { get; set; }
        public string ZipcodeId { get; set; }
        public bool IsUpdateInformation { get; set; }
        public bool IsRegisterKidClub { get; set; }
        public bool IsAgreeGetOutMember { get; set; }
        public SexType Sex { get; set; }
        public IList<MemberKid> MemberKids { get; private set; } = new List<MemberKid>();
        public PICStore PICStore { get; set; }
        public IList<RequestsReceipted> RequestsReceipteds { get; private set; } = new List<RequestsReceipted>();
        public IList<RequestsPending> RequestsPendings { get; private set; } = new List<RequestsPending>();
    }
}

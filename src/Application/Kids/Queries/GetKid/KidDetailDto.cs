using mrs.Domain.Enums;
using System;

namespace mrs.Application.Kids.Queries.GetKid
{
    public class KidDetailDto
    {
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string ParentFuriganaFirstName { get; set; }
        public string ParentFuriganaLastName { get; set; }
        public int RelationshipMember { get; set; }
        public string RelationshipMemberText { get; set; }
        public string Email { get; set; }
        public DateTime RegisterKidClubDate { get; set; }
        public RequestTypeEnum RequestType { get; set; }
        public string RequestTypeText { get; set; }
        public string CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public int? PICStoreId { get; set; }
        public string PICStoreName { get; set; }
        public string DeviceCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public int Sex { get; set; }
        public string SexTypeText { get; set; }
        public bool IsLivingWithParent { get; set; }
        public string IsLivingWithParentText { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Remark { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsEnableEdit { get; set; }
    }
}

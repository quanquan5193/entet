using System;

namespace mrs.Application.Kids.Queries.GetKidsPaging
{
    public class KidDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MemberNo { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string ParentName { get; set; }
        public string ParentFuriganaFirstName { get; set; }
        public string ParentFuriganaLastName { get; set; }
        public string ParentFuriganaName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public string FuriganaName { get; set; }
        public int Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RelationshipMember { get; set; }
        public int? PICStoreId { get; set; }
        public string PICStoreName { get; set; }
    }
}

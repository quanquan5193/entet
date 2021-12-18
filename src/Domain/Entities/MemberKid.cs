using mrs.Domain.Common;
using System;

namespace mrs.Domain.Entities
{
    public class MemberKid : AuditableEntity
    {
        public int Id { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public string ParentFuriganaFirstName { get; set; }
        public string ParentFuriganaLastName { get; set; }
        public int RelationshipMember { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public int Sex { get; set; }
        public bool IsLivingWithParent { get; set; }
        public string Remark { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Member Member { get; set; }
    }
}

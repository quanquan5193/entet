using AutoMapper;
using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Members.Queries.GetMemberInfo
{
    public class MemberInfoDto : IMapFrom<Member>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FuriganaFirstName { get; set; }
        public string FuriganaLastName { get; set; }
        public int Sex { get; set; }
        public string Zipcode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string BuildingName { get; set; }
        public bool IsRegisterAdvertisement { get; set; }
        public bool IsUpdateInformation { get; set; }
        public bool IsNetMember { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FixedPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Remark { get; set; }
        public string MemberNo { get; set; }
        public string OldMemberNo { get; set; } = "";
    }
}

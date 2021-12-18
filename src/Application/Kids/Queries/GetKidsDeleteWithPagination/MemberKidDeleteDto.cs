using AutoMapper;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;

namespace mrs.Application.Kids.Queries.GetKidsWithPagination
{
    public class MemberKidDeleteDto : IMapFrom<MemberKid>
    {
        public int Id { get; set; }
        public int No { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MemberNo { get; set; }
        public string ParentName { get; set; }
        public string KidName { get; set; }
        public int Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int RelationshipMember { get; set; }
        public int? StoreId { get; set; }
        public int? CompanyId { get; set; }
        public Store Store { get; set; }
        public Company Company { get; set; }
    }

    public class MemberKidDeleteCommandDto
    {
        public int Id { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

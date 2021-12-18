using AutoMapper;
using mrs.Application.ApplicationUser.Queries.GetToken;
using mrs.Application.Common.Interfaces;
using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;

namespace mrs.Application.Cards.Queries.GetCards
{
    public class CardDto : IMapFrom<Card>
    {
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public CardStatus Status { get; set; }
        public string Point { get; set; }
        public int? StoreId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public Store Store { get; set; }
        public Company Company { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public ApplicationUserDto Updateder { get; set; }
        public ApplicationUserDto Createder { get; set; }
    }

}

using mrs.Domain.Common;
using mrs.Domain.Enums;
using System;
using System.Collections.Generic;

namespace mrs.Domain.Entities
{
    public class Card : AuditableEntity
    {
        public int Id { get; set; }
        public int No { get; set; }
        public string MemberNo { get; set; }
        public CardStatus Status { get; set; }
        public string Point { get; set; }
        public int? StoreId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public Store Store { get; set; }
        public Company Company { get; set; }

    }
}

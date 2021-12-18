using mrs.Domain.Common;
using System.Collections.Generic;

namespace mrs.Domain.Entities
{
    public class Company : AuditableEntity
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NormalizedCompanyName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public IList<Card> Cards { get; private set; } = new List<Card>();
        public IList<Store> Stores { get; private set; } = new List<Store>();
    }
}

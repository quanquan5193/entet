using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;

namespace mrs.Application.Stores.Queries.GetStores
{
    public class StoreDto
    {
        public int Id { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string NormalizedStoreName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }
}

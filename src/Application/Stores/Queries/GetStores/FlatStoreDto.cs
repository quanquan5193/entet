using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;

namespace mrs.Application.Stores.Queries.GetStores
{
    public class FlatStoreDto : IMapFrom<Store>
    {
        public int Id { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public int CompanyId { get; set; }
        public string NormalizedStoreName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}

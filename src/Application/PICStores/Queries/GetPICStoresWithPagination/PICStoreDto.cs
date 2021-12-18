using System;

namespace mrs.Application.PICStores.Queries.GetPICStoresWithPagination
{
    public class PICStoreDto
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string PICCode { get; set; }
        public string PICName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Store { get; set; }
        public string NormalizedStoreName { get; set; }
        public string Company { get; set; }
        public string NormalizedCompanyName { get; set; }
    }
}

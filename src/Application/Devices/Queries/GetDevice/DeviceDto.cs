using mrs.Application.Companies.Queries.GetCompanies;
using mrs.Application.Stores.Queries.GetStores;

namespace mrs.Application.Devices.Queries.GetDevice
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string DeviceCode { get; set; }
        public int StoreId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAutoLock { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public StoreDto Store { get; set; }
        public CompanyDto Company { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Queries.SearchDevicesWithPagination
{
    public class SearchDevicesWithPaginationDto
    {
        public int Id { get; set; }

        public int No { get; set; }

        public string DeviceCode { get; set; }

        public string CompanyCode { get; set; }

        public string NormalizedCompanyName { get; set; }

        public string CompanyName { get; set; }

        public string StoreCode { get; set; }

        public string NormalizedStoreName { get; set; }

        public string StoreName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }
        public string IsAutoLock { get; set; }
    }
}

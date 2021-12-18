using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Devices.Commands.GetDevice
{
    public class GetDeviceDto
    {
        public int Id { get; set; }

        public string DeviceCode { get; set; }

        public int StoreId { get; set; }

        public int CompanyId { get; set; }

        public bool IsActive { get; set; }
        public bool IsAutoLock { get; set; }

        public string Lat { get; set; }

        public string Long { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedByUserName { get; set; }

        public string CreatedByFullName { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public string UpdatedByUserName { get; set; }

        public string UpdatedByFullName { get; set; }
    }
}

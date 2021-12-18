using mrs.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Domain.Entities
{
    public class ZipCode : AuditableEntity
    {        
        public string Zipcode { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string BuildingName { get; set; }
    }
}

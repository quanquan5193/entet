using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.ZipCodes.Queries.GetInfoZipcode
{
    public class ZipcodeDto : IMapFrom<ZipCode>
    {
        public string Zipcode { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
    }
}

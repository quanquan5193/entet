using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.ZipCodes.Queries.ImportZipcode
{
    public class ZipcodeCsvImportDto
    {
        [Index(2)]
        public int Zipcode { get; set; }
        [Index(6)]
        public string Province { get; set; }
        [Index(7)]
        public string District { get; set; }
        [Index(8)]
        public string Street { get; set; }
    }
}

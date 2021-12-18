using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Companies.Commands.GetDetailCompanyWithCode
{
    public class GetDetailCompanyWithCodeDto
    {
        public int Id { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }
    }
}

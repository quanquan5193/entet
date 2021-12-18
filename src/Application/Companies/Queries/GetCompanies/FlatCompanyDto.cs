using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using System;
using System.Collections.Generic;

namespace mrs.Application.Companies.Queries.GetCompanies
{
    public class FlatCompanyDto : IMapFrom<Company>
    {
        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string NormalizedCompanyName { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}

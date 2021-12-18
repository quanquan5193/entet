using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Receptions.Queries.Dto
{  
    public class DateObject
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }

    public class GraphObject
    {
        public int TotalCreateCards { get; set; }
        public int TotalSwitchCards { get; set; }
        public int TotalKidClubs { get; set; }
        public int TotalOther { get; set; }
    }

    public class DetailObject
    {
        public int TotalCreateCards { get; set; }
        public int TotalSwitchCards { get; set; }
        public int TotalReissuedCards { get; set; }
        public int TotalChangeCards { get; set; }
        public int TotalDiscardCards { get; set; }
        public int TotalPointMigration { get; set; }
        public int TotalKidClubs { get; set; }
    }

    public class ReceptionGraphResult
    {
        public GraphObject Total { get; set; }
        public List<ReceptionGraphDto> ListData { get; set; }
    }

    public class ReceptionDetailResult
    {
        public DetailObject Total { get; set; }
        public List<ReceptionDetailDto> ListData { get; set; }
    }

    public class ReceptionGraphDto
    {
        public DateObject ReceptionDateObj { get; set; }
        public DateTime ReceptionDate { get; set; }
        public bool IsDisplayMonth { get; set; }
        public int TotalCreateCards { get; set; }
        public int TotalSwitchCards { get; set; }
        public int TotalOther { get; set; }
        public int TotalKidClubs { get; set; }
    }

    public class ReceptionDetailDto
    {
        public int No { get; set; }
        public DateObject ReceptionDateObj { get; set; }
        public DateTime ReceptionDate { get; set; }
        public int TotalCreateCards { get; set; }
        public int TotalSwitchCards { get; set; }
        public int TotalReissuedCards { get; set; }
        public int TotalChangeCards { get; set; }
        public int TotalDiscardCards { get; set; }
        public int TotalPointMigration { get; set; }
        public int TotalKidClubs { get; set; }
    }
}

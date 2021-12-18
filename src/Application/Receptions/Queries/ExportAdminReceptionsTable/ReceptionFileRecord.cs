using mrs.Application.Common.Mappings;
using mrs.Application.Receptions.Queries.Dto;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;

namespace mrs.Application.Cards.Queries.ExportReceptionsTable
{
    public class ReceptionsRecord : IMapFrom<ReceptionDetailDto>
    {
        [CsvHelper.Configuration.Attributes.Name("No.")]
        public int No { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付日")]
        public DateTime ReceptionDate { get; set; }
        [CsvHelper.Configuration.Attributes.Name("新規")]
        public int TotalCreateCards { get; set; }
        [CsvHelper.Configuration.Attributes.Name("切替")]
        public int TotalSwitchCards { get; set; }
        [CsvHelper.Configuration.Attributes.Name("再発行")]
        public int TotalReissuedCards { get; set; }
        [CsvHelper.Configuration.Attributes.Name("変更")]
        public int TotalChangeCards { get; set; }
        [CsvHelper.Configuration.Attributes.Name("退会")]
        public int TotalDiscardCards { get; set; }
        [CsvHelper.Configuration.Attributes.Name("P移行")]
        public int TotalPointMigration { get; set; }
        [CsvHelper.Configuration.Attributes.Name("キッズ")]
        public int TotalKidClubs { get; set; }
    }
}

using CsvHelper.Configuration;
using mrs.Application.Cards.Queries.ExportReceptionsTable;
using System.Globalization;

namespace mrs.Infrastructure.Files.Maps
{
    public class ReceptionRecordMap : ClassMap<ReceptionsRecord>
    {
        public ReceptionRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.ReceptionDate).ConvertUsing(c => c.ReceptionDate.ToString("yyyyMMdd"));
        }
    }
}

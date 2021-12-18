using CsvHelper.Configuration;
using mrs.Application.Cards.Queries.ExportCards;
using mrs.Application.RequestsReceipteds.Queries.ExportRequestReceipted;
using mrs.Domain.Enums;
using System.Globalization;

namespace mrs.Infrastructure.Files.Maps
{
    public class RequestReceiptedRecordMap : ClassMap<RequestReceiptedRecord>
    {
        public RequestReceiptedRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            //Map(m => m.Status).ConvertUsing(c => StaticEnum.GetStringValue(c.Status));
            //Map(m => m.ExpiredAt).ConvertUsing(c => c.ExpiredAt.ToString("yyyyMM"));
            //Map(m => m.CreatedAt).ConvertUsing(c => c.CreatedAt.ToString("yyyyMMdd"));
            //Map(m => m.UpdatedAt).ConvertUsing(c => c.UpdatedBy != null ? c.UpdatedAt.ToString("yyyyMMddhhmmss") : "");
        }
    }
}

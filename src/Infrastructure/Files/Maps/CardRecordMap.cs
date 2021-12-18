using CsvHelper.Configuration;
using mrs.Application.Cards.Queries.ExportCards;
using mrs.Application.Kids.Queries.ExportKids;
using mrs.Domain.Enums;
using System;
using System.Globalization;

namespace mrs.Infrastructure.Files.Maps
{
    public class CardRecordMap : ClassMap<CardRecord>
    {
        public CardRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Status).ConvertUsing(c => StaticEnum.GetStringValue(c.Status));
            Map(m => m.ExpiredAt).ConvertUsing(c => c.ExpiredAt.ToString("yyyyMM"));
            Map(m => m.CreatedAt).ConvertUsing(c => c.CreatedAt.ToString("yyyyMMdd"));
            Map(m => m.UpdatedAt).ConvertUsing(c => c.UpdatedAt > DateTime.MinValue ? c.UpdatedAt.ToString("yyyyMMddhhmmss") : "");
        }
    }
    public class KidCsvRecordMap : ClassMap<KidCsvRecord>
    {
        public KidCsvRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.RegisterDate).ConvertUsing(c => c.RegisterDate.ToString("yyyyMMdd"));
            Map(m => m.DateOfBirth).ConvertUsing(c => c.DateOfBirth.ToString("yyyyMMdd"));
        }
    }
}

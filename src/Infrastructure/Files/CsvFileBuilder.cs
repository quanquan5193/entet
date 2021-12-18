using mrs.Application.Common.Interfaces;
using mrs.Infrastructure.Files.Maps;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using mrs.Application.Cards.Queries.ExportCards;
using mrs.Application.Kids.Queries.ExportKids;
using mrs.Application.RequestsReceipteds.Queries.ExportRequestReceipted;
using mrs.Application.Cards.Queries.ExportReceptionsTable;

namespace mrs.Infrastructure.Files
{
    public class CsvFileBuilder : ICsvFileBuilder
    {
        public byte[] BuilCardsFile(IEnumerable<CardRecord> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

                csvWriter.Configuration.RegisterClassMap<CardRecordMap>();
                csvWriter.WriteRecords(records);
            }

            return memoryStream.ToArray();
        }

        public byte[] BuildKidsFile(IEnumerable<KidCsvRecord> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

                csvWriter.Configuration.RegisterClassMap<KidCsvRecordMap>();
                csvWriter.WriteRecords(records);
            }

            return memoryStream.ToArray();
        }

        public byte[] BuilRequestReceiptedFile(IEnumerable<RequestReceiptedRecord> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

                csvWriter.Configuration.RegisterClassMap<RequestReceiptedRecordMap>();
                csvWriter.WriteRecords(records);
            }

            return memoryStream.ToArray();
        }

        public byte[] BuilReceptionFile(IEnumerable<ReceptionsRecord> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

                csvWriter.Configuration.RegisterClassMap<ReceptionRecordMap>();
                csvWriter.WriteRecords(records);
            }

            return memoryStream.ToArray();
        }
    }
}

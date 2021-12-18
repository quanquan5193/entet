using mrs.Application.Cards.Queries.ExportCards;
using mrs.Application.Cards.Queries.ExportReceptionsTable;
using mrs.Application.Kids.Queries.ExportKids;
using mrs.Application.RequestsReceipteds.Queries.ExportRequestReceipted;
using System.Collections.Generic;

namespace mrs.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuilCardsFile(IEnumerable<CardRecord> records);
        byte[] BuildKidsFile(IEnumerable<KidCsvRecord> records);
        byte[] BuilRequestReceiptedFile(IEnumerable<RequestReceiptedRecord> records);
        byte[] BuilReceptionFile(IEnumerable<ReceptionsRecord> records);

    }
}

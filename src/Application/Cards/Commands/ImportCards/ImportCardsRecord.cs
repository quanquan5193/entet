using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;

namespace mrs.Application.Cards.Commands.ImportCards
{
    public class ImportCardsRecord : IMapFrom<Card>
    {
        [CsvHelper.Configuration.Attributes.Name("No.")]
        public string No { get; set; }
        [CsvHelper.Configuration.Attributes.Name("お客様番号")]
        public string MemberNo { get; set; }
        [CsvHelper.Configuration.Attributes.Name("有効期限")]
        public string ExpirationDate { get; set; }
        [CsvHelper.Configuration.Attributes.Name("会社コード")]
        public string CompanyCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("店舗コード")]
        public string StoreCode { get; set; }
    }
}

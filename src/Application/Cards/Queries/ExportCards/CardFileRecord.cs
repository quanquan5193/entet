using mrs.Application.Common.Mappings;
using mrs.Domain.Entities;
using mrs.Domain.Enums;
using System;

namespace mrs.Application.Cards.Queries.ExportCards
{
    public class CardRecord : IMapFrom<Card>
    {
        [CsvHelper.Configuration.Attributes.Name("No.")]
        public string No { get; set; }
        [CsvHelper.Configuration.Attributes.Name("お客様番号")]
        public string MemberNo { get; set; }
        [CsvHelper.Configuration.Attributes.Name("登録日")]
        public DateTime CreatedAt { get; set; }
        [CsvHelper.Configuration.Attributes.Name("有効期限")]
        public DateTime ExpiredAt { get; set; }
        [CsvHelper.Configuration.Attributes.Name("カード状態")]
        public CardStatus Status { get; set; }
        [CsvHelper.Configuration.Attributes.Name("会社コード")]
        public string CompanyCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("店舗コード")]
        public string StoreCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("登録者")]
        public string CreatedByName { get; set; }
        [CsvHelper.Configuration.Attributes.Name("更新日時")]
        public DateTime UpdatedAt { get; set; }
        [CsvHelper.Configuration.Attributes.Name("更新者")]
        public string UpdatedByName { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string CreatedBy { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string UpdatedBy { get; set; }
    }
}

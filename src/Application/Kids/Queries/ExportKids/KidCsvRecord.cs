using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Kids.Queries.ExportKids
{
    public class KidCsvRecord
    {
        [CsvHelper.Configuration.Attributes.Name("No.")]
        public string No { get; set; }
        [CsvHelper.Configuration.Attributes.Name("お客様番号")]
        public string CustomerNo { get; set; }

        [CsvHelper.Configuration.Attributes.Name("受付日")]
        public DateTime RegisterDate { get; set; }

        [CsvHelper.Configuration.Attributes.Name("受付区分")]
        public string RequestType { get; set; }

        [CsvHelper.Configuration.Attributes.Name("受付会社コード")]
        public string CompanyCode { get; set; }

        [CsvHelper.Configuration.Attributes.Name("受付店舗コード")]
        public string StoreCode { get; set; }

        [CsvHelper.Configuration.Attributes.Name("受付担当者")]
        public string PICStoreName { get; set; }

        [CsvHelper.Configuration.Attributes.Name("受付端末番号")]
        public string DeviceCode { get; set; }

        [CsvHelper.Configuration.Attributes.Name("保護者氏名漢字")]
        public string ParentName { get; set; }

        [CsvHelper.Configuration.Attributes.Name("保護者氏名カナ")]
        public string KanaParentName { get; set; }

        [CsvHelper.Configuration.Attributes.Name("メールアドレス")]
        public string Email { get; set; }

        [CsvHelper.Configuration.Attributes.Name("続柄")]
        public string Relationship { get; set; }

        [CsvHelper.Configuration.Attributes.Name("同居別居")]
        public string IsLivingWithParent { get; set; }

        [CsvHelper.Configuration.Attributes.Name("子氏名漢字")]
        public string Name { get; set; }

        [CsvHelper.Configuration.Attributes.Name("子氏名カナ")]
        public string KanaName { get; set; }

        [CsvHelper.Configuration.Attributes.Name("子性別")]
        public string SexType { get; set; }

        [CsvHelper.Configuration.Attributes.Name("子生年月日")]
        public DateTime DateOfBirth { get; set; }

        [CsvHelper.Configuration.Attributes.Name("備考")]
        public string Remark { get; set; }
    }
}

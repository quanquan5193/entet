using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.RequestsReceipteds.Queries.ExportRequestReceipted
{
    public class RequestReceiptedRecord
    {
        [CsvHelper.Configuration.Attributes.Name("No.")]
        public int No { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付番号")]
        public string RequestCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付日")]
        public string ReceiptedDatetime { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付区分")]
        public string RequestType { get; set; }
        [CsvHelper.Configuration.Attributes.Name("情報変更")]
        public string ChangeInfomation { get; set; }
        [CsvHelper.Configuration.Attributes.Name("お客様番号")]
        public string MemberNo { get; set; }
        [CsvHelper.Configuration.Attributes.Name("旧お客様番号")]
        public string OldMemberNo { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付会社コード")]
        public string CompanyCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付店舗コード")]
        public string StoreCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付担当者")]
        public string PICStore { get; set; }
        [CsvHelper.Configuration.Attributes.Name("受付端末番号")]
        public string DeviceoCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("氏名漢字")]
        public string KanjiName { get; set; }
        [CsvHelper.Configuration.Attributes.Name("氏名カナ")]
        public string KanaName { get; set; }
        [CsvHelper.Configuration.Attributes.Name("性別")]
        public string Sex { get; set; }
        [CsvHelper.Configuration.Attributes.Name("生年月日")]
        public string DateOfBirth { get; set; }
        [CsvHelper.Configuration.Attributes.Name("電話番号")]
        public string FixedNumber { get; set; }
        [CsvHelper.Configuration.Attributes.Name("携帯電話")]
        public string MobileNumber { get; set; }
        [CsvHelper.Configuration.Attributes.Name("郵便番号")]
        public string ZipCode { get; set; }
        [CsvHelper.Configuration.Attributes.Name("住所")]
        public string Address { get; set; }
        [CsvHelper.Configuration.Attributes.Name("DM受取フラグ")]
        public string IsRegisterAdvertisement { get; set; }
        [CsvHelper.Configuration.Attributes.Name("メールアドレス")]
        public string Email { get; set; }
        [CsvHelper.Configuration.Attributes.Name("ネット会員フラグ")]
        public string IsNetMember { get; set; }
        [CsvHelper.Configuration.Attributes.Name("キッズクラブ登録フラグ")]
        public string IsRegisterKidClub { get; set; }
        [CsvHelper.Configuration.Attributes.Name("退会同意フラグ")]
        public string IsAgreeGetOutMember { get; set; }
        [CsvHelper.Configuration.Attributes.Name("備考")]
        public string Remark { get; set; }

        //properties not allow export
        [CsvHelper.Configuration.Attributes.Ignore]
        public string FirstName { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string LastName { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string FuriganaFirstName { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string FuriganaLastName { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string Province { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string District { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string Street { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public string BuildingName { get; set; }
    }
}

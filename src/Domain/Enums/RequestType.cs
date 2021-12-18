namespace mrs.Domain.Enums
{
    public enum RequestTypeEnum
    {
        [StringValue("新規")]
        New = 1,
        [StringValue("切替")]
        Switch = 2,
        [StringValue("再発行")]
        ReIssued = 3,
        [StringValue("変更")]
        ChangeCard = 4,
        [StringValue("退会")]
        LeaveGroup = 5,
        [StringValue("Ｐ移行")]
        PMigrate = 6,
        [StringValue("キッズ")]
        Kid = 7
    }
}

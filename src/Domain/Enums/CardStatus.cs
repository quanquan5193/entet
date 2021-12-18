namespace mrs.Domain.Enums
{
    public enum CardStatus : byte
    {
        [StringValue("未発行")]
        Unissued = 1,
        [StringValue("発行済")]
        Issued = 2,
        [StringValue("退会")]
        Withdrawal = 3,
        [StringValue("紛失")]
        Missing = 4,
        [StringValue("廃棄")]
        Disposal = 5
    }
}

namespace mrs.Domain.Enums
{
    public enum KidRelationshipEnum : byte
    {
        [StringValue("")]
        Unset = 0,
        [StringValue("父")]
        Father = 1,
        [StringValue("母")]
        Mother = 2,
        [StringValue("祖父")]
        GrandFarther = 3,
        [StringValue("祖母")]
        GrandMother = 4,
        [StringValue("その他")]
        Other = 5,
    }
}

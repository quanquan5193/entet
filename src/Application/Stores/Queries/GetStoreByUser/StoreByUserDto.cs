namespace mrs.Application.Stores.Queries.GetStoreByUser
{
    public class StoreByUserDto
    {
        public int Id { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public int CompanyId { get; set; }
        public string NormalizedStoreName { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }
    }
}

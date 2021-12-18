namespace mrs.Application.Cards.Queries.ExportCards
{
    public class ExportCardsVm
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
namespace mrs.Application.Cards.Queries.ExportReceptionsTable
{
    public class ExportReceptionsTableVm
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
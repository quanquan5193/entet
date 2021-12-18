using System;
using mrs.Domain.Common;

namespace mrs.Domain.ValueObjects
{
    public class FileUpload : IFileUpload
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public string PreviewImagePath { get; set; }
        public string UniqueFileId { get; set; }
    }
}

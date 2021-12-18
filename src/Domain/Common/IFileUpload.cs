using System;
using System.Collections.Generic;
using System.Text;

namespace mrs.Domain.Common
{
    public interface IFileUpload
    {
        string FileName { get; set; }
        string FileType { get; set; }
        long FileSize { get; set; }
        string FilePath { get; set; }
    }
}

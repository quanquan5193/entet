using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Common.Helpers.AzureStorage
{
    public interface IAzureStorageHelper
    {
        Task SaveLogToBlob(string message);
        Task SaveLogAfterLoginToBlob(string userName, string message);
        Task SaveMultipleLogToBlob(string messages);
        Task<string> ConfigureMessage(string message);
        Task SaveLogPerformanceToBlob(string message);
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace mrs.Application.Common.Helpers.InitiateServices
{
    public static class StorageAccountConnection
    {
        public static CloudStorageAccount CreateConnection(IConfiguration configuration)
        {
            var storageConnectionString = configuration["StorageAccount:StorageAccountConnection"];
            if (CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                return storageAccount;
            }
            return null;
        }
    }
}

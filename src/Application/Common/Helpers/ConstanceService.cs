using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.WindowsAzure.Storage;
using System;

namespace mrs.Application.Common.Helpers
{
    public static class ConstanceService
    {
        public static CryptographyClient cryptographyClient { get; set; }
        public static CloudStorageAccount cloudStorageAccount { get; set; }
        public static string Folder => DateTime.UtcNow.ToString("yyyyMMdd");
    }
}

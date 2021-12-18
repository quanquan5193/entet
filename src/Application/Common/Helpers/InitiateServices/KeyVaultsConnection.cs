using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace mrs.Application.Common.Helpers.InitiateServices
{
    public static class KeyVaultsConnection
    {
        public static async Task<CryptographyClient> CreateCryptClient(IConfiguration configuration)
        {
            string tenantId = configuration["KeyVaults:TenantID"];
            string clientId = configuration["KeyVaults:ClientId"];
            string clientSecret = configuration["KeyVaults:ClientSecret"];
            string keyVaultName = configuration["KeyVaults:KeyVaultName"];
            string keyName = configuration["KeyVaults:KeyName"];
            string keyVaultUri = $"https://{keyVaultName}.vault.azure.net/";

            ClientSecretCredential clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            KeyClient keyClient = new KeyClient(new Uri(keyVaultUri), clientSecretCredential);
            var key = await keyClient.GetKeyAsync(keyName);

            CryptographyClient cryptoClient = new CryptographyClient(key.Value.Id, clientSecretCredential);

            return cryptoClient;
        }
    }
}

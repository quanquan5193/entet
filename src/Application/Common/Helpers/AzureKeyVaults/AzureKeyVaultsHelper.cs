using Azure.Security.KeyVault.Keys.Cryptography;
using mrs.Domain.Entities;
using System;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Common.Helpers.AzureKeyVaults
{
    public static class AzureKeyVaultsHelper
    {
        public static async Task<string> ToEncryptStringAsync(this string inputFieldData)
        {
            if(string.IsNullOrEmpty(inputFieldData))
            {
                return null;
            }    
            byte[] inputAsByteArray = Encoding.UTF8.GetBytes(inputFieldData);

            EncryptResult encryptResult = await ConstanceService.cryptographyClient.EncryptAsync(EncryptionAlgorithm.RsaOaep, inputAsByteArray);

            return Convert.ToBase64String(encryptResult.Ciphertext);
        }
        public static async Task<string> ToDecryptStringAsync(this string inputFieldData)
        {
            if (string.IsNullOrEmpty(inputFieldData))
            {
                return null;
            }
            byte[] inputAsByteArray = Convert.FromBase64String(inputFieldData);

            DecryptResult decryptResult = await ConstanceService.cryptographyClient.DecryptAsync(EncryptionAlgorithm.RsaOaep, inputAsByteArray);

            return Encoding.Default.GetString(decryptResult.Plaintext);
        }
        public static async Task EncryptMember(Member member)
        {
            member.FuriganaFirstName = await member.FuriganaFirstName.ToEncryptStringAsync();
            member.FuriganaLastName = await member.FuriganaLastName.ToEncryptStringAsync();
            member.FirstName = await member.FirstName.ToEncryptStringAsync();
            member.LastName = await member.LastName.ToEncryptStringAsync();
            member.FixedPhone = await member.FixedPhone.ToEncryptStringAsync();
            member.MobilePhone = await member.MobilePhone.ToEncryptStringAsync();
            member.Province = await member.Province.ToEncryptStringAsync();
            member.District = await member.District.ToEncryptStringAsync();
            member.Street = await member.Street.ToEncryptStringAsync();
            member.BuildingName = await member.BuildingName.ToEncryptStringAsync();
            member.Email = await member.Email.ToEncryptStringAsync();
        }
        public static async Task DecryptMember(Member member)
        {
            member.FuriganaFirstName = await member.FuriganaFirstName.ToDecryptStringAsync();
            member.FuriganaLastName = await member.FuriganaLastName.ToDecryptStringAsync();
            member.FirstName = await member.FirstName.ToDecryptStringAsync();
            member.LastName = await member.LastName.ToDecryptStringAsync();
            member.FixedPhone = await member.FixedPhone.ToDecryptStringAsync();
            member.MobilePhone = await member.MobilePhone.ToDecryptStringAsync();
            member.Province = await member.Province.ToDecryptStringAsync();
            member.District = await member.District.ToDecryptStringAsync();
            member.Street = await member.Street.ToDecryptStringAsync();
            member.BuildingName = await member.BuildingName.ToDecryptStringAsync();
            member.Email = await member.Email.ToDecryptStringAsync();
        }
    }
}

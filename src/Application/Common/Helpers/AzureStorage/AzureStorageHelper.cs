using Microsoft.Extensions.Configuration;
using mrs.Application.Common.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace mrs.Application.Common.Helpers.AzureStorage
{
    public class AzureStorageHelper : IAzureStorageHelper
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;
        private static string _userName;
        public AzureStorageHelper(ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration)
        {
            _currentUserService = currentUserService;
            _identityService = identityService;
            _configuration = configuration;
            _userName = null;
        }
        /// <summary>
        /// write log to storage accounts
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SaveLogToBlob(string message)
        {
            var containername = _configuration["StorageAccount:ContainerName"];
            var nowUTC = DateTime.UtcNow;

            var userId = _currentUserService.UserId ?? string.Empty;
            string userName = string.Empty;
            string contentFile = string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
                contentFile = $"{nowUTC:s}: {userName} - {message}\n";
            }
            else
            {
                contentFile = $"{nowUTC:s}: {message}\n";
            }

            var cloudBlobClient = ConstanceService.cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containername);
            //var folder = cloudBlobContainer.GetDirectoryReference(Constance.Folder);

            var cloudBlockBlob = cloudBlobContainer.GetAppendBlobReference($"{nowUTC:yyyyMMdd}.txt");
            if (await cloudBlockBlob.ExistsAsync())
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contentFile)))
                {
                    await cloudBlockBlob.AppendBlockAsync(stream);
                }
            }
            else
            {
                await cloudBlockBlob.UploadTextAsync(contentFile);
            }
        }
        /// <summary>
        /// write logs to storage account after login successful
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SaveLogAfterLoginToBlob(string userName, string message)
        {
            var containername = _configuration["StorageAccount:ContainerName"];
            var nowUTC = DateTime.UtcNow;

            var contentFile = $"{nowUTC:s}: {userName} - {message}\n";

            var cloudBlobClient = ConstanceService.cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containername);
            //var folder = cloudBlobContainer.GetDirectoryReference(Constance.Folder);

            var cloudBlockBlob = cloudBlobContainer.GetAppendBlobReference($"{nowUTC:yyyyMMdd}.txt");
            if (await cloudBlockBlob.ExistsAsync())
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contentFile)))
                {
                    await cloudBlockBlob.AppendBlockAsync(stream);
                }
            }
            else
            {
                await cloudBlockBlob.UploadTextAsync(contentFile);
            }
        }

        /// <summary>
        /// write multiple log to storage accounts
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public async Task SaveMultipleLogToBlob(string messages)
        {
            var containername = _configuration["StorageAccount:ContainerName"];
            var nowUTC = DateTime.UtcNow;
            string contentFile = messages;

            var cloudBlobClient = ConstanceService.cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containername);
            //var folder = cloudBlobContainer.GetDirectoryReference(Constance.Folder);

            var cloudBlockBlob = cloudBlobContainer.GetAppendBlobReference($"{nowUTC:yyyyMMdd}.txt");

            const int uploadLimit = 4194304;
            int bytesRead;
            long index = 0;
            byte[] buffer = new byte[uploadLimit];
            if (await cloudBlockBlob.ExistsAsync())
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contentFile)))
                {
                    if (stream.Length <= uploadLimit)
                    {
                        await cloudBlockBlob.AppendBlockAsync(stream);
                    }
                    else
                    {
                        // Stream is larger than the limit so we need to upload in chunks
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            // Create a memory stream for the buffer to upload
                            using MemoryStream ms = new MemoryStream(buffer, 0, bytesRead);
                            await cloudBlockBlob.AppendFromStreamAsync(ms);
                            index += ms.Length; // increment the index to the account for bytes already written
                        }
                    }
                }
            }
            else
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contentFile)))
                {
                    if (stream.Length <= uploadLimit)
                    {
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        // Stream is larger than the limit so we need to upload in chunks
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            // Create a memory stream for the buffer to upload
                            using MemoryStream ms = new MemoryStream(buffer, 0, bytesRead);
                            if (index == 0)
                            {
                                await cloudBlockBlob.UploadFromStreamAsync(ms);
                            }
                            else
                            {
                                await cloudBlockBlob.AppendFromStreamAsync(ms);
                            }
                            index += ms.Length; // increment the index to the account for bytes already written
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> ConfigureMessage(string message)
        {
            var userId = _currentUserService.UserId ?? string.Empty;
            if (string.IsNullOrEmpty(_userName))
            {
                _userName = await _identityService.GetUserNameAsync(userId);
            }
            var nowUTC = DateTime.UtcNow;

            return $"{nowUTC:s}: {_userName} - {message}";
        }
        /// <summary>
        /// logging performance
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SaveLogPerformanceToBlob(string message)
        {
            var containername = _configuration["StorageAccount:ContainerNameLoggingPerformance"];
            var nowUTC = DateTime.UtcNow;

            var userId = _currentUserService.UserId ?? string.Empty;
            string userName = string.Empty;
            string contentFile = string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
                contentFile = $"{nowUTC:s}: {userName} - {message}\n";
            }
            else
            {
                contentFile = $"{nowUTC:s}: {message}\n";
            }

            var cloudBlobClient = ConstanceService.cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containername);
            //var folder = cloudBlobContainer.GetDirectoryReference(Constance.Folder);

            var cloudBlockBlob = cloudBlobContainer.GetAppendBlobReference($"{nowUTC:yyyyMMdd}.txt");
            if (await cloudBlockBlob.ExistsAsync())
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contentFile)))
                {
                    await cloudBlockBlob.AppendBlockAsync(stream);
                }
            }
            else
            {
                await cloudBlockBlob.UploadTextAsync(contentFile);
            }
        }
    }
}

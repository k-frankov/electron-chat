using System;
using System.IO;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

using ElectronChatAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ElectronChatAPI.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string storageConnectionString;
        private readonly ILogger<BlobStorageService> logger;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            this.storageConnectionString = configuration.GetValue<string>("StorageConnectionString");
            this.logger = logger;
        }

        public async Task<FileUploadResultDto> UploadFileToStorage(IFormFile file, string userName)
        {
            FileUploadResultDto fileUploadResultDto = new();

            try
            {
                BlobServiceClient cloudStorageAccount = new(this.storageConnectionString);
                BlobContainerClient blobContainer = cloudStorageAccount.GetBlobContainerClient("shared-files");
                await blobContainer.CreateIfNotExistsAsync();

                BlobClient blockBlob = blobContainer.GetBlobClient($"{userName}/{file.Name}");
                using Stream readStream = file.OpenReadStream();
                await blockBlob.UploadAsync(readStream, true);

                fileUploadResultDto.UploadedFileUrl = blockBlob.Uri.ToString();
                return fileUploadResultDto;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Saving to blob storage error: {ex.Message}");
                fileUploadResultDto.Errors.Add(ex.Message);
            }

            return fileUploadResultDto;
        }
    }
}

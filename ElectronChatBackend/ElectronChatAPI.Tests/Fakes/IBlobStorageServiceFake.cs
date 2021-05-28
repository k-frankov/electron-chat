using System.Threading.Tasks;

using ElectronChatAPI.Models;
using ElectronChatAPI.Services;

using Microsoft.AspNetCore.Http;

namespace ElectronChatAPI.Tests.Fakes
{
    public class BlobStorageServiceFake : IBlobStorageService
    {
        public Task<FileUploadResultDto> UploadFileToStorage(IFormFile file, string userName)
        {
            return Task.FromResult<FileUploadResultDto>(new() { UploadedFileUrl = "https://shared.file" });
        }
    }
}

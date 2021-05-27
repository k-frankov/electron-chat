using System.Threading.Tasks;
using ElectronChatAPI.Models;
using Microsoft.AspNetCore.Http;

namespace ElectronChatAPI.Services
{
    public interface IBlobStorageService
    {
        Task<FileUploadResultDto> UploadFileToStorage(IFormFile file, string userName);
    }
}

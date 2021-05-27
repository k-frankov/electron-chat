using System.Collections.Generic;

namespace ElectronChatAPI.Models
{
    public class FileUploadResultDto
    {
        public string UploadedFileUrl = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}

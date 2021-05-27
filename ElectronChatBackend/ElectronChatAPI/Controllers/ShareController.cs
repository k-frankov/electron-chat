using System.Text.Json;
using System.Threading.Tasks;
using ElectronChatAPI.Extensions;
using ElectronChatAPI.Models;
using ElectronChatAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElectronChatAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShareController : ControllerBase
    {
        private readonly ILogger<ShareController> logger;
        private readonly IBlobStorageService blobStorageService;

        public ShareController(ILogger<ShareController> logger, IBlobStorageService blobStorageService)
        {
            this.logger = logger;
            this.blobStorageService = blobStorageService;
        }

        [HttpPost()]
        public async Task<IActionResult> ShareFile()
        {
            try
            {
                IFormFileCollection files = this.Request.Form.Files;
                if(files.Count == 0)
                {
                    return BadRequest("No file found");
                }

                if (files.Count > 1)
                {
                    return BadRequest("Only one file is allowed");
                }

                IFormFile file = files[0];

                var mb = (file.Length / 1024f) / 1024f;
                if (mb <= 0 || mb > 5)
                {
                    return BadRequest("File is too big");
                }

                FileUploadResultDto fileUploadResult = await this.blobStorageService.UploadFileToStorage(file, this.User.GetUserName());

                if (fileUploadResult.Errors.Count == 0)
                {
                    return Ok();
                }
                else
                {
                    this.logger.LogError($"Share file errors: {JsonSerializer.Serialize(fileUploadResult.Errors)}");
                    return StatusCode(StatusCodes.Status500InternalServerError, fileUploadResult.Errors);
                }

            }
            catch (System.Exception ex)
            {
                this.logger.LogError($"Share file error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

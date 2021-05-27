using System;
using System.Threading.Tasks;
using ElectronChatAPI.Extensions;
using ElectronChatAPI.Hubs;
using ElectronChatAPI.Models;
using ElectronChatAPI.Services;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<ElectronChatHub> hubContext;
        private readonly IMessageRepository messageRepository;

        public ShareController(
            ILogger<ShareController> logger,
            IBlobStorageService blobStorageService,
            IHubContext<ElectronChatHub> hubContext,
            IMessageRepository messageRepository)
        {
            this.logger = logger;
            this.blobStorageService = blobStorageService;
            this.hubContext = hubContext;
            this.messageRepository = messageRepository;
        }

        [HttpPost("{group}")]
        public async Task<IActionResult> ShareFile(string group)
        {
            try
            {
                IFormFileCollection files = this.Request.Form.Files;
                if (files.Count == 0)
                {
                    return BadRequest("No file found");
                }

                if (files.Count > 1)
                {
                    return BadRequest("Only one file is allowed");
                }

                IFormFile file = files[0];

                var mb = file.Length / 1024f / 1024f;
                if (mb <= 0 || mb > 5)
                {
                    return BadRequest("File is too big");
                }

                string userName = this.User.GetUserName();
                FileUploadResultDto fileUploadResult = await this.blobStorageService.UploadFileToStorage(file, userName);

                if (fileUploadResult.Errors.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(group))
                    {
                        DateTime now = DateTime.UtcNow;
                        MessageDto messageDto = new()
                        {
                            UserName = userName,
                            Message = "Shared file",
                            MessageTime = now.ToShortTimeString(),
                            SharedLink = fileUploadResult.UploadedFileUrl,
                        };

                        await this.hubContext.Clients.Groups(group).SendAsync("GetMessageInChannel", messageDto);

                        MessageEntity messageEntity = new()
                        {
                            UserName = messageDto.UserName,
                            Message = messageDto.Message,
                            MessageTime = now,
                            ChannelName = group,
                            SharedLink = messageDto.SharedLink,
                        };

                        _ = await this.messageRepository.AddMessageAsync(messageEntity);
                    }
                    else
                    {
                        this.logger.LogWarning("Group name is not provided during file sharing...");
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Share file error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

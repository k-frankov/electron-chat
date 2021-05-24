using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronChatAPI.Hubs;
using ElectronChatAPI.Models;
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
    public class ChannelController : ControllerBase
    {
        private readonly ILogger<ChannelController> logger;
        private readonly IChannelRepository channelRepository;
        private readonly IHubContext<ElectronChatHub> hubContext;

        public ChannelController(
            ILogger<ChannelController> logger,
            IChannelRepository channelRepository,
            IHubContext<ElectronChatHub> hubContext)
        {
            this.logger = logger;
            this.channelRepository = channelRepository;
            this.hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel(ChannelDto channelDto)
        {
            try
            {
                string userName = this.User.GetUserName();
                if (string.IsNullOrWhiteSpace(userName))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "OOPS!");
                }

                ChannelEntity channelEntityByName = await this.channelRepository.GetChannelByNameAsync(channelDto.ChannelName);
                if (channelEntityByName != null)
                {
                    return BadRequest("Channel with this name is already registered.");
                }

                ChannelEntity channelEntity = new ChannelEntity
                {
                    ChannelName = channelDto.ChannelName,
                    UserName = userName,
                };

                await this.channelRepository.CreateChannelAsync(channelEntity);
                List<ChannelEntity> channels = await this.channelRepository.GetAllChannelsAsync();
                List<string> channelNames = channels.Select(x => x.ChannelName).ToList();
                await this.hubContext.Clients.All.SendAsync("GetChannels", channelNames);

                return Ok();
            }
            catch (Exception e)
            {
                this.logger.LogError(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
            }
        }
    }
}

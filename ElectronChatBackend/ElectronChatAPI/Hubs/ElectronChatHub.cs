using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ElectronChatAPI.Hubs
{
    [Authorize]
    public class ElectronChatHub : Hub
    {
        private readonly IChannelRepository channelRepository;

        public ElectronChatHub(IChannelRepository channelRepository)
        {
            this.channelRepository = channelRepository;
        }

        public override async Task OnConnectedAsync()
        {
            List<ChannelEntity> channels = await this.channelRepository.GetAllChannelsAsync();
            List<string> channelNames = channels.Select(x => x.ChannelName).ToList();
            await this.Clients.Caller.SendAsync("GetChannels", channelNames);

            await base.OnConnectedAsync();
        }
    }
}
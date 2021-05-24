using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ElectronChatAPI.Hubs
{
    [Authorize]
    public class ElectronChatHub : Hub
    {
        private readonly ILogger<ElectronChatHub> logger;
        private readonly IChannelRepository channelRepository;
        private Dictionary<string, string> usersInGroups = new Dictionary<string, string>();

        public ElectronChatHub(ILogger<ElectronChatHub> logger, IChannelRepository channelRepository)
        {
            this.logger = logger;
            this.channelRepository = channelRepository;
        }

        public override async Task OnConnectedAsync()
        {
            List<ChannelEntity> channels = await this.channelRepository.GetAllChannelsAsync();
            List<string> channelNames = channels.Select(x => x.ChannelName).ToList();
            await this.Clients.Caller.SendAsync("GetChannels", channelNames);

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await this.LeaveGroupIfInGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChannel(string groupName)
        {
            try
            {
                ChannelEntity channel = await this.channelRepository.GetChannelByNameAsync(groupName);
                if (channel == null)
                {
                    await this.Clients.Caller.SendAsync("ChannelJoined", new { groupJoined = false, groupName = groupName });
                }

                string userName = this.Context.User.GetUserName();

                await this.LeaveGroupIfInGroup();

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                usersInGroups.Add(userName, groupName);
                await this.Clients.Caller.SendAsync("ChannelJoined", new { groupJoined = true, groupName = groupName });

                await Clients.Group(groupName).SendAsync("SendToClientChannel", $"{userName} has joined the channel.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
                await this.Clients.Caller.SendAsync("ChannelJoined", new { groupJoined = false, groupName = groupName });
            }
        }

        public async Task RemoveFromGroup(string groupName)
        {
            string userName = this.Context.User.GetUserName();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{userName} has left the channel.");
        }

        private async Task LeaveGroupIfInGroup()
        {
            string userName = this.Context.User.GetUserName();

            var userAlreadyInGroup = string.Empty;
            if (usersInGroups.TryGetValue(userName, out userAlreadyInGroup))
            {
                await this.RemoveFromGroup(userAlreadyInGroup);
                usersInGroups.Remove(userName);
            }
        }
    }
}
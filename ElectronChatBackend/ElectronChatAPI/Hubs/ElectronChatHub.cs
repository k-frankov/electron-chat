using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectronChatAPI.Extensions;
using ElectronChatAPI.Models;
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
        private readonly IMessageRepository messageRepository;
        private static readonly ReaderWriterLock rwl = new();
        private readonly Dictionary<string, string> usersInGroups = new();

        public ElectronChatHub(
            ILogger<ElectronChatHub> logger,
            IChannelRepository channelRepository,
            IMessageRepository messageRepository)
        {
            this.logger = logger;
            this.channelRepository = channelRepository;
            this.messageRepository = messageRepository;
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

        public async Task QuitChannel(string groupName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await this.LeaveGroupIfInGroup();
                await Clients.Group(groupName).SendAsync("SendToClientChannel", $"{this.Context.User.GetUserName()} has left the channel.");
                await this.Clients.Caller.SendAsync("ChannelJoined", new { groupJoined = true, string.Empty });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
            }
        }

        public async Task JoinChannel(string groupName)
        {
            try
            {
                ChannelEntity channel = await this.channelRepository.GetChannelByNameAsync(groupName);
                if (channel == null)
                {
                    await this.Clients.Caller.SendAsync("ChannelJoined", new { groupJoined = false, groupName });
                }

                string userName = this.Context.User.GetUserName();

                await this.LeaveGroupIfInGroup();

                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                // TODO: Add pagination / retrieve portion by portion on demand;
                List<MessageEntity> messagesFromDb = await this.messageRepository.GetAllMessagesInChannelAsync(groupName);
                List<MessageDto> messages = new();

                // TODO: Automapper would be nice...
                messagesFromDb.ForEach(m =>
                {
                    messages.Add(new MessageDto
                    {
                        Message = m.Message,
                        MessageTime = m.MessageTime.ToShortTimeString(),
                        UserName = m.UserName,
                        SharedLink = m.SharedLink,
                    });
                });

                rwl.AcquireWriterLock(200);
                try
                {
                    usersInGroups.Add(userName, groupName);
                }
                finally
                {
                    rwl.ReleaseWriterLock();
                }

                await this.Clients.Caller.SendAsync("ChannelJoined", new { groupJoined = true, groupName });
                await this.Clients.Caller.SendAsync("GetUsersInChannel", this.GetGroupUsers(groupName));
                await this.Clients.Caller.SendAsync("GetMessagesInChannel", messages);
                await Clients.Group(groupName).SendAsync("SendToClientChannel", $"{userName} has joined the channel.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
                await this.Clients.Caller.SendAsync("ChannelJoined", new { groupJoined = false, groupName });
            }
        }

        public async Task SendMessageToChannel(string groupName, string message)
        {
            try
            {
                string userName = this.Context.User.GetUserName();
                DateTime now = DateTime.UtcNow;
                MessageDto messageDto = new()
                {
                    UserName = userName,
                    Message = message,
                    MessageTime = now.ToShortTimeString(),
                };

                await this.Clients.Group(groupName).SendAsync("GetMessageInChannel", messageDto);

                MessageEntity messageEntity = new()
                {
                    UserName = messageDto.UserName,
                    Message = messageDto.Message,
                    MessageTime = now,
                    ChannelName = groupName,
                };

                await this.messageRepository.AddMessageAsync(messageEntity);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.ToString());
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

            rwl.AcquireReaderLock(200);
            try
            {
                if (usersInGroups.TryGetValue(userName, out string userAlreadyInGroup))
                {
                    await this.RemoveFromGroup(userAlreadyInGroup);
                    rwl.AcquireWriterLock(200);
                    try
                    {
                        _ = usersInGroups.Remove(userName);
                    }
                    finally
                    {
                        rwl.ReleaseWriterLock();
                    }
                }
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }

        private List<string> GetGroupUsers(string groupName)
        {
            rwl.AcquireReaderLock(200);
            try
            {
                return usersInGroups
                    .Where(e => e.Value.ToLowerInvariant() == groupName.ToLowerInvariant())
                    .Select(e => e.Key)
                    .ToList();
            }
            finally
            {
                rwl.ReleaseReaderLock();
            }
        }
    }
}
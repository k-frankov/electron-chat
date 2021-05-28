using System.Collections.Generic;
using System.Threading.Tasks;

using ElectronChatAPI.Hubs;
using ElectronChatAPI.Models;

namespace ElectronChatAPI.Tests.Fakes
{
    public class ElectronChatHubFake : IElectronChatHub
    {
        public Task JoinChannel(string groupName)
        {
            return Task.FromResult(true);
        }

        public Task QuitChannel(string groupName)
        {
            return Task.FromResult(true);
        }

        public Task RemoveFromGroup(string groupName)
        {
            return Task.FromResult(true);
        }

        public Task SendMessageToChannel(string groupName, string message)
        {
            return Task.FromResult(true);
        }

        public Task SendMessageToChannel(string groupName, MessageDto messageDto)
        {
            return Task.FromResult(true);
        }

        public Task SetChannels(List<string> channelNames)
        {
            return Task.FromResult(true);
        }
    }
}

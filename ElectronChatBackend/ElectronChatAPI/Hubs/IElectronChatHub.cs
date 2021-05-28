using System.Collections.Generic;
using System.Threading.Tasks;

using ElectronChatAPI.Models;

namespace ElectronChatAPI.Hubs
{
    public interface IElectronChatHub
    {
        Task QuitChannel(string groupName);
        Task JoinChannel(string groupName);
        Task SendMessageToChannel(string groupName, string message);
        Task RemoveFromGroup(string groupName);
        Task SetChannels(List<string> channelNames);
        public Task SendMessageToChannel(string groupName, MessageDto messageDto);
    }
}

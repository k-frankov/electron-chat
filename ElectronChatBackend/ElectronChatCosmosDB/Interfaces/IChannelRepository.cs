using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;

namespace ElectronChatCosmosDB.Interfaces
{
    public interface IChannelRepository
    {
        Task<ChannelEntity> CreateChannelAsync(ChannelEntity  channelEntity);
        Task<ChannelEntity> GetChannelByNameAsync(string  channelName);
        Task<List<ChannelEntity>> GetAllChannelsAsync();
    }
}

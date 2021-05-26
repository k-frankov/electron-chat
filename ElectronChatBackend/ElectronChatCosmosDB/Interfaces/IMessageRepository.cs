using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;

namespace ElectronChatCosmosDB.Interfaces
{
    public interface IMessageRepository
    {
        Task<MessageEntity> AddMessageAsync(MessageEntity message);
        Task<List<MessageEntity>> GetAllMessagesInChannelAsync(string channelName);
    }
}

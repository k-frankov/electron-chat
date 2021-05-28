using System.Collections.Generic;
using System.Threading.Tasks;

using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;

namespace ElectronChatAPI.Tests.Fakes
{
    public class MessageRepositoryFake : IMessageRepository
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<MessageEntity> AddMessageAsync(MessageEntity message)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return message;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<List<MessageEntity>> GetAllMessagesInChannelAsync(string channelName)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return new List<MessageEntity>();
        }
    }
}

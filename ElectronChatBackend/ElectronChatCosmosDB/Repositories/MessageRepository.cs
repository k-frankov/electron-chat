using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.Azure.Cosmos;

namespace ElectronChatCosmosDB.Repositories
{
    public class MessageRepository : CosmosDbBase, IMessageRepository
    {
        public const string ContainerName = "Messages";

        public async Task<MessageEntity> AddMessageAsync(MessageEntity message)
        {
            message.id = Guid.NewGuid();
            using CosmosClient client = base.GetCosmosClient();
            Database db = client.GetDatabase(base.dBConfiguration.DBName);
            Container container = db.GetContainer(ContainerName);
            ItemResponse<MessageEntity> result = await container.CreateItemAsync(message);

            return result.Resource;
        }

        public async Task<List<MessageEntity>> GetAllMessagesInChannelAsync(string channelName)
        {
            using CosmosClient client = base.GetCosmosClient();
            
            Database db = client.GetDatabase(base.dBConfiguration.DBName);
            Container container = db.GetContainer(ContainerName);

            QueryDefinition query = new QueryDefinition("SELECT * FROM Messages u WHERE u.ChannelName = @channelName")
                .WithParameter("@channelName", channelName);

            using FeedIterator<MessageEntity> resultSetIterator = container.GetItemQueryIterator<MessageEntity>(query);
            var messages = new List<MessageEntity>();
            while (resultSetIterator.HasMoreResults)
            {
                FeedResponse<MessageEntity> response = await resultSetIterator.ReadNextAsync();
                messages.AddRange(response.Resource);
            }

            return messages;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.Azure.Cosmos;

namespace ElectronChatCosmosDB.Repositories
{
    public class ChannelRepository : CosmosDbBase, IChannelRepository
    {
        public const string ContainerName = "Channels";

        public async Task<ChannelEntity> CreateChannelAsync(ChannelEntity channelEntity)
        {
            channelEntity.id = Guid.NewGuid();
            using CosmosClient client = base.GetCosmosClient();
            Database db = client.GetDatabase(base.dBConfiguration.DBName);
            Container container = db.GetContainer(ContainerName);
            ItemResponse<ChannelEntity> result = await container.CreateItemAsync(channelEntity);

            return result.Resource;
        }

        public async Task<ChannelEntity> GetChannelByNameAsync(string channelName)
        {
            using (CosmosClient client = base.GetCosmosClient())
            {
                Database db = client.GetDatabase(base.dBConfiguration.DBName);
                Container container = db.GetContainer(ContainerName);

                QueryDefinition query = new QueryDefinition("SELECT * FROM Channels u WHERE u.ChannelName = @channelName")
                    .WithParameter("@channelName", channelName);

                using FeedIterator<ChannelEntity> resultSetIterator = container.GetItemQueryIterator<ChannelEntity>(query);
                while (resultSetIterator.HasMoreResults)
                {
                    FeedResponse<ChannelEntity> response = await resultSetIterator.ReadNextAsync();
                    return response.Resource.FirstOrDefault();
                }
            }

            return null;
        }

        public async Task<List<ChannelEntity>> GetAllChannelsAsync()
        {
            var result = new List<ChannelEntity>();
            using (CosmosClient client = base.GetCosmosClient())
            {
                Database db = client.GetDatabase(base.dBConfiguration.DBName);
                Container container = db.GetContainer(ContainerName);

                QueryDefinition query = new("SELECT * FROM Channels");

                using FeedIterator<ChannelEntity> resultSetIterator = container.GetItemQueryIterator<ChannelEntity>(query);
                while (resultSetIterator.HasMoreResults)
                {
                    FeedResponse<ChannelEntity> response = await resultSetIterator.ReadNextAsync();
                    result.AddRange(response);
                }
            }

            return result;
        }
    }
}

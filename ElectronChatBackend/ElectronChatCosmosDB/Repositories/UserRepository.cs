using System;
using System.Linq;
using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;
using ElectronChatCosmosDB.Interfaces;
using Microsoft.Azure.Cosmos;

namespace ElectronChatCosmosDB.Repositories
{
    public class UserRepository : CosmosDbBase, IUserRepository
    {
        public const string ContainerName = "Users";

        public async Task<UserEntity> CreateUserAsync(UserEntity userEntity)
        {
            userEntity.id = Guid.NewGuid();
            using CosmosClient client = base.GetCosmosClient();
            Database db = client.GetDatabase(base.dBConfiguration.DBName);
            Container container = db.GetContainer(ContainerName);
            ItemResponse<UserEntity> result = await container.CreateItemAsync(userEntity);

            return result.Resource;
        }

        public async Task<UserEntity> GetUserByUserNameAsync(string userName)
        {
            using (CosmosClient client = base.GetCosmosClient())
            {
                Database db = client.GetDatabase(base.dBConfiguration.DBName);
                Container container = db.GetContainer(ContainerName);

                QueryDefinition query = new QueryDefinition("SELECT * FROM Users u WHERE u.UserName = @userName")
                    .WithParameter("@userName", userName);

                using FeedIterator<UserEntity> resultSetIterator = container.GetItemQueryIterator<UserEntity>(query);
                while (resultSetIterator.HasMoreResults)
                {
                    FeedResponse<UserEntity> response = await resultSetIterator.ReadNextAsync();
                    return response.Resource.FirstOrDefault();
                }
            }

            return null;
        }
    }
}

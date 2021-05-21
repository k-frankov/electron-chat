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

        public UserRepository()
        {
            using (CosmosClient client = base.GetCosmosClient())
            {
                Database db = client.GetDatabase(base.dBConfiguration.DBName);
            }
        }

        public async Task<UserEntity> CreateUser(UserEntity userEntity)
        {
            userEntity.id = Guid.NewGuid();
            using (CosmosClient client = base.GetCosmosClient())
            {
                Database db = client.GetDatabase(base.dBConfiguration.DBName);
                Container container = db.GetContainer(ContainerName);
                ItemResponse<UserEntity> result = await container.CreateItemAsync(userEntity);

                return result.Resource;
            }
        }

        public async Task<UserEntity> GetUserByUserName(string userName)
        {
            using (CosmosClient client = base.GetCosmosClient())
            {
                Database db = client.GetDatabase(base.dBConfiguration.DBName);
                Container container = db.GetContainer(ContainerName);

                QueryDefinition query = new QueryDefinition("SELECT * FROM Users u WHERE u.UserName = @userName")
                    .WithParameter("@userName", userName);

                using (FeedIterator<UserEntity> resultSetIterator = container.GetItemQueryIterator<UserEntity>(query))
                {
                    while (resultSetIterator.HasMoreResults)
                    {
                        FeedResponse<UserEntity> response = await resultSetIterator.ReadNextAsync();
                        return response.Resource.FirstOrDefault();
                    }
                }
            }

            return null;
        }
    }
}

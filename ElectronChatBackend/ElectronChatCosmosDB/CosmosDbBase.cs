using Microsoft.Azure.Cosmos;

namespace ElectronChatCosmosDB
{
    public class CosmosDbBase
    {
        protected DBConfiguration dBConfiguration = DBConfiguration.Instance;
        protected CosmosClient GetCosmosClient() => new CosmosClient(dBConfiguration.EnpointUrl, dBConfiguration.PrimaryKey);
    }
}

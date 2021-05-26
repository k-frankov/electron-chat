using Microsoft.Azure.Cosmos;

namespace ElectronChatCosmosDB
{
    public class CosmosDbBase
    {
        protected DBConfiguration dBConfiguration = DBConfiguration.Instance;
        protected CosmosClient GetCosmosClient() => new(dBConfiguration.EnpointUrl, dBConfiguration.PrimaryKey);
    }
}

using System;
using ElectronChatCosmosDB.Repositories;
using Microsoft.Azure.Cosmos;

namespace ElectronChatCosmosDB
{
    public sealed class DBConfiguration
    {
        private static DBConfiguration _instance;
        private DBConfiguration() { }

        public string EnpointUrl { get; private set; }
        public string PrimaryKey { get; private set; }
        public string DBName { get; private set; }

        public static async void Initialize(string endpointUrl, string primaryKey, string dbName)
        {
            if (String.IsNullOrWhiteSpace(endpointUrl))
            {
                throw new ArgumentNullException(endpointUrl);
            }

            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(primaryKey);
            }

            if (String.IsNullOrWhiteSpace(dbName))
            {
                throw new ArgumentNullException(dbName);
            }

            using (CosmosClient client = new(endpointUrl, primaryKey))
            {
                Database db = await client.CreateDatabaseIfNotExistsAsync(dbName);
                await client.GetDatabase(dbName).DefineContainer(name: UserRepository.ContainerName, partitionKeyPath: "/UserName")
                    .WithUniqueKey()
                        .Path("/UserName")
                    .Attach()
                    .CreateIfNotExistsAsync();

                await client.GetDatabase(dbName).DefineContainer(name: ChannelRepository.ContainerName, partitionKeyPath: "/ChannelName")
                    .WithUniqueKey()
                        .Path("/UserName")
                        .Path("/ChannelName")
                    .Attach()
                    .CreateIfNotExistsAsync();

                await client.GetDatabase(dbName).DefineContainer(name: MessageRepository.ContainerName, partitionKeyPath: "/ChannelName")
                    .WithUniqueKey()
                        .Path("/UserName")
                        .Path("/MessageTime")
                    .Attach()
                    .CreateIfNotExistsAsync();
            }

            _instance = new DBConfiguration
            {
                EnpointUrl = endpointUrl,
                PrimaryKey = primaryKey,
                DBName = dbName
            };
        }

        public static DBConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new Exception("DBConfiguraion must be initialized.");
                }

                return _instance;
            }
        }
    }
}

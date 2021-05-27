namespace ElectronChatCosmosDB.Entities
{
    public class MessageEntity
    {
        public System.Guid id { get; set; }
        public string ChannelName { get; set; }
        public string UserName { get; set; }
        public System.DateTime MessageTime { get; set; }
        public string Message { get; set; }
        public string SharedLink { get; set; }
    }
}

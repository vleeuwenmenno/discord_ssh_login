namespace DiscordWebhookNET.Models
{
    public class Author
    {
        public bool bot { get; set; }
        public string id { get; set; }
        public string username { get; set; }
        public object avatar { get; set; }
        public string discriminator { get; set; }
    }
}
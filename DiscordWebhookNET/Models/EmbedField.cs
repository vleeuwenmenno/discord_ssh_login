namespace DiscordWebhookNET.Models
{
    public class EmbedField
    {
        public string name { get; set; }
        public string value { get; set; } = "";
        public bool inline { get; set; } = false;
    }
}
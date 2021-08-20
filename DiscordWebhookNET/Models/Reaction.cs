namespace DiscordWebhookNET.Models
{
    public class Reaction
    {
        public Emoji emoji { get; set; }
        public int count { get; set; }
        public bool me { get; set; }
    }
}
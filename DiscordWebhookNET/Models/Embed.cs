using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DiscordWebhookNET.Models
{
    public class Embed
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("title")]
        public string title { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("type")]
        public string type { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("description")]
        public string description { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("url")]
        public string url { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("timestamp")]
        public DateTime timestamp { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("color")]
        public int color { get; set; } = 0;
        
        // footer?	embed footer object	footer information
        // image?	embed image object	image information
        // thumbnail?	embed thumbnail object	thumbnail information
        // video?	embed video object	video information
        // provider?	embed provider object	provider information
        // author?	embed author object	author information
            
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("fields")]
        public List<EmbedField> fields { get; set; }
    }
}
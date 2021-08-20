using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DiscordWebhookNET.Models;

namespace DiscordWebhookNET
{
    public class DiscordWebHook
    {
        public string webHook { get; private set; }
        
        public DiscordWebHook(string webHook)
        {
            this.webHook = webHook;
        }

        public async Task<Message> SendMessageAsync(string message, string username, Embed[] embeds = null)
        {
            Dictionary<object, object> body = new Dictionary<object, object>() { { "content", message } };
            
            if (!string.IsNullOrEmpty(username))
                body.Add("username", username);
            
            if (embeds != null)
                body.Add("embeds", embeds);
            
            Tuple<string, bool> response = await DoRequest($"{webHook}?wait=true", body);
            if (response.Item2)
                return JsonSerializer.Deserialize<Message>(response.Item1);
            
            return null;
        }
        
        public async Task<Message> GetMessageAsync(string id)
        {
            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{webHook}/messages/{id}?wait=true"),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<Message>(await response.Content.ReadAsStringAsync());
            }
        }
        
        public bool SendMessage(string message, string username, Embed[] embeds = null)
        {
            Dictionary<object, object> body = new Dictionary<object, object>() { { "content", message } };
            
            if (!string.IsNullOrEmpty(username))
                body.Add("username", username);
            
            if (embeds != null)
                body.Add("embeds", embeds);

            return (DoRequest($"{webHook}?wait=false", body).Result).Item2;
        }

        private async Task<Tuple<string, bool>> DoRequest(string url, Dictionary<object, object> content)
        {
            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(JsonSerializer.Serialize(content))
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };
            
            using (var response = await client.SendAsync(request))
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    return new Tuple<string, bool>(await response.Content.ReadAsStringAsync(), true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new Tuple<string, bool>("", false);
                }
            }
        }
        
        /// <summary>
        /// Wait for a reaction to be placed on the given message id
        /// </summary>
        /// <param name="msgId">The message id to poll reactions on.</param>
        /// <param name="timeoutMs">The timeout in ms (minimal 5000ms)</param>
        /// <param name="allowedEmoji">A list of allowed emojis, if this is null we return the first placed emoji and we ignore any emoji that's not allowed.</param>
        /// <param name="timeoutDelay">Timeout delay is the delay we use in between the tries.</param>
        /// <returns></returns>
        public async Task<Emoji> WaitForReaction(string msgId, int timeoutMs, Emoji[] allowedEmoji = null, int timeoutDelay = 500)
        {
            if (timeoutDelay < 200)
                timeoutDelay = 200;

            if (timeoutMs < 5000)
                timeoutDelay = 5000;
            
            for (int i = 0; i < timeoutMs / timeoutDelay; i++)
            {
                Message msg = await this.GetMessageAsync(msgId);

                if (msg.reactions != null)
                {
                    if (allowedEmoji == null)
                        return msg.reactions.First().emoji;
                    
                    foreach (Emoji emoji in allowedEmoji)
                        if (msg.reactions.Any(x => x.emoji.name == emoji.name || (x.emoji.id == emoji.id && !string.IsNullOrEmpty(emoji.id))))
                            return emoji;
                }

                Thread.Sleep(timeoutDelay);
            }

            return null;
        }
    }
}
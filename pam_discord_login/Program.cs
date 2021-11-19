using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using DiscordWebhookNET;
using DiscordWebhookNET.Models;

namespace pam_discord_login
{
    class Program
    {
        private Dictionary<string, DateTime> whiteList;
        
        static void Main(string[] args) =>
            new Program().MainAsync(args).GetAwaiter().GetResult();

        private async Task MainAsync(string[] args)
        {
            if (args.Length == 0 && args.Length != 4)
            {
                Console.WriteLine("Missing parameters, expected command [webhook] [remote_ip] [hostname] [open|close]");
                return;
            }

            if (File.Exists("/tmp/dpsdwl.json"))
                whiteList = JsonSerializer.Deserialize<Dictionary<string, DateTime>>(
                    await File.ReadAllTextAsync("/tmp/dpsdwl.json"));
            
            else
                whiteList = new Dictionary<string, DateTime>();
            
            Console.Write("Requesting permission from Discord ... ");
            
            IPInfo ipInfo = await IPApi.GetInfo(IPAddress.Parse(args[1]));
            DiscordWebHook webHook = new DiscordWebHook($"https://discord.com/api/webhooks/{args[0]}");

            if (ipInfo == null)
            {
                ipInfo = new IPInfo()
                {
                    Ip = "Unknown ip",
                    City = "Unknown city",
                    Region = "unknown region",
                    CountryName = "unknown country"
                };
            }

            if (args[3] == "close")
            {
                await webHook.SendMessageAsync("", $"{args[2]} (SSH Login)", new []
                {
                    new Embed()
                    {
                        color = 2123412,
                        description = "A SSH connection is being closed.",
                        title = "SSH Connection is closing",
                        fields = new List<EmbedField>()
                        {
                            new()
                            {
                                name = "Remote IP",
                                value = $"`{ipInfo.Ip}`"
                            },
                            new()
                            {
                                name = "Server hostname",
                                value = $"`{args[2]}`"
                            },
                            new()
                            {
                                name = "Location",
                                value = $"{ipInfo.City}, {ipInfo.Region}, {ipInfo.CountryName}."
                            },
                            new()
                            {
                                name = "Maps",
                                value = $"[Google Maps](https://maps.google.com/?q={ipInfo.Latitude},{ipInfo.Longitude})"
                            }
                        },
                        timestamp = DateTime.Now
                    }
                });
                Console.WriteLine("[ CLOSED ]");
                Environment.Exit(0);
            }
            else if (args[3] == "open")
            {
                if (whiteList != null && whiteList.Any(x => x.Key == ipInfo.Ip))
                {
                    if (DateTime.Now.Subtract(whiteList.SingleOrDefault(x => x.Key == ipInfo.Ip).Value).Hours < 24)
                    {
                        await webHook.SendMessageAsync("", $"{args[2]} (SSH Login)", new []
                        {
                            new Embed()
                            {
                                color = 2067276,
                                description = "Whitelisted IP has been auto-approved for login.",
                                title = "SSH Login auto-approved",
                                fields = new List<EmbedField>()
                                {
                                    new()
                                    {
                                        name = "Remote IP",
                                        value = $"`{ipInfo.Ip}`"
                                    },
                                    new()
                                    {
                                        name = "Server hostname",
                                        value = $"`{args[2]}`"
                                    },
                                    new()
                                    {
                                        name = "Location",
                                        value = $"{ipInfo.City}, {ipInfo.Region}, {ipInfo.CountryName}."
                                    },
                                    new()
                                    {
                                        name = "Maps",
                                        value = $"[Google Maps](https://maps.google.com/?q={ipInfo.Latitude},{ipInfo.Longitude})"
                                    }
                                },
                                timestamp = DateTime.Now
                            }
                        });
                        Console.WriteLine("[ ACCEPTED ]");
                        Environment.Exit(0);
                    }
                    else
                    {
                        whiteList.Remove(ipInfo.Ip);
                        await File.WriteAllTextAsync("/tmp/dpsdwl.json", JsonSerializer.Serialize(whiteList));
                    }
                }
                
                Message msg = await webHook.SendMessageAsync("", $"{args[2]} (SSH Login)", new []
                {
                    new Embed()
                    {
                        color = 10038562,
                        description = "Would you like to approve this login?",
                        title = "SSH Login attempt",
                        fields = new List<EmbedField>()
                        {
                            new()
                            {
                                name = "Remote IP",
                                value = $"`{ipInfo.Ip}`"
                            },
                            new()
                            {
                                name = "Server hostname",
                                value = $"`{args[2]}`"
                            },
                            new()
                            {
                                name = "Location",
                                value = $"{ipInfo.City}, {ipInfo.Region}, {ipInfo.CountryName}."
                            },
                            new()
                            {
                                name = "Maps",
                                value = $"[Google Maps](https://maps.google.com/?q={ipInfo.Latitude},{ipInfo.Longitude})"
                            },
                            new()
                            {
                                name = "Options: ",
                                value = $"\U00002705 = Approve | \U000026D4 = Decline | \U0001F4BE = Approve & Remember IP for today",
                            }
                        },
                        timestamp = DateTime.Now
                    }
                });

                if (msg != null)
                {
                    Emoji reaction = await webHook.WaitForReaction(msg.id, 60 * 1000, new Emoji[]
                    {
                        new()
                        {
                            name = "\U00002705"
                        },
                        new()
                        {
                            name = "\U00002611"
                        },
                        new()
                        {
                            name = "\U00002714"
                        },
                        new()
                        {
                            name = "\U000026D4"
                        },
                        new()
                        {
                            name = "\U0001F4BE"
                        }
                    });

                    if (reaction != null)
                    {
                        if (reaction.name is "\U00002705" or "\U00002611" or "\U00002714")
                        {
                            webHook.SendMessage("", $"{args[2]} (SSH Login)", new[]
                            {
                                new Embed()
                                {
                                    color = 2067276,
                                    description = $"Access has been approved for `{ipInfo.Ip}`",
                                    title = "SSH Login approved"
                                }
                            });
                            Console.WriteLine("[ ACCEPTED ]");
                            Environment.Exit(0);
                        }
                        else if (reaction.name == "\U0001F4BE")
                        {
                            webHook.SendMessage("", $"{args[2]} (SSH Login)", new[]
                            {
                                new Embed()
                                {
                                    color = 2067276,
                                    description = $"Access has been approved for `{ipInfo.Ip}` and IP has been whitelisted for the next 24 hours.",
                                    title = "SSH Login approved"
                                }
                            });
                            Console.WriteLine("[ ACCEPTED ]");
                            
                            whiteList.Add(ipInfo.Ip, DateTime.Now);
                            await File.WriteAllTextAsync("/tmp/dpsdwl.json", JsonSerializer.Serialize(whiteList));
                            
                            Environment.Exit(0);
                        }
                        else
                        {
                            webHook.SendMessage("", $"{args[2]} (SSH Login)", new[]
                            {
                                new Embed()
                                {
                                    color = 10038562,
                                    description = $"Access has been declined for `{ipInfo.Ip}`",
                                    title = "SSH Login declined"
                                }
                            });
                            Console.WriteLine("[ DECLINED ]");
                            Environment.Exit(400);
                        }
                    }
                    else
                    {
                        webHook.SendMessage("", $"{args[2]} (SSH Login)", new[]
                        {
                            new Embed()
                            {
                                color = 10038562,
                                description = "Login on SSH has timed-out after 60 seconds.",
                                title = "SSH Login timed-out"
                            }
                        });
                        Console.WriteLine("[ TIME-OUT ]");
                        Environment.Exit(408);
                    }
                }
                else
                {
                    Console.WriteLine("[ ERROR ]\nFATAL ISSUE!! Failed to send Discord message!!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
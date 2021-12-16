using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core
{
    public class DiscordChannelNotification
    {
        public DiscordChannelNotification(string message)
        {
            Content = message;
        }
        [JsonProperty("username")]
        public string UserName { get; set; } = "BrokenSwordsBot 1.1";

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}

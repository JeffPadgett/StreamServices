using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core
{
    public class StreamerInfo
    {
        [JsonProperty("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonProperty("broadcaster_login")]
        public string BroadcasterLogin { get; set; }
        [JsonProperty("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonProperty("broadcaster_language")]
        public string BroadcasterLanguage { get; set; }
        [JsonProperty("game_id")]
        public string GameId { get; set; }
        [JsonProperty("game_name")]
        public string GameName { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
    }
}

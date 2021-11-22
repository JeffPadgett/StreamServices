using Newtonsoft.Json;
using System;
using System.Linq;

namespace StreamServices.Core
{

    public class TwitchSubscriptionInitalPost
    {
        public TwitchSubscriptionInitalPost(string userId, string eventType = "stream.online")
        {
            Condition = new Condition() { BroadcasterUserId = userId};
            Transport = new Transport() 
            {
                Method = "webhook",
                Callback = Environment.GetEnvironmentVariable("StreamStartFunctionUri"),
                Secret = Environment.GetEnvironmentVariable("EventSubSecret")
            };
            Type = eventType;
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; } = "1";

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("transport")]
        public Transport Transport { get; set; }
    }

}

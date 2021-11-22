using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core
{
    public class Subscription
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("cost")]
        public int Cost { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        public Condition Condition { get; set; }
        public Transport Transport { get; set; }
    }

    public class Event
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("user_name")]
        public string UserName { get; set; }
        [JsonProperty("user_login")]
        public string UserLogin { get; set; }
        [JsonProperty("broadcaster_user_id")]
        public string BroadcasterUserId { get; set; }
        [JsonProperty("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; set; }
        [JsonProperty("broadcaster_user_name")]
        public string BroadcasterUserName { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("started_at")]
        public DateTime StartedAt { get; set; }
    }


    public class Condition
    {
        [JsonProperty("broadcaster_user_id")]
        public string BroadcasterUserId { get; set; }
    }

    public class Transport
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("callback")]
        public string Callback { get; set; }
        [JsonProperty("secret")]
        public string Secret { get; set; }
    }
    public class Pagination
    {
    }

}

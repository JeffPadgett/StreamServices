using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace StreamServices.Dto
{
    public class TwitchUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("broadcaster_type")]
        public string BroadcasterType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("profile_image_url")]
        public Uri ProfileImageUrl { get; set; } //TODO

        [JsonProperty("offline_image_url")]
        public Uri OfflineImageUrl { get; set; }

        [JsonProperty("view_count")]
        public long ViewCount { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }

    }

    public class TwitchUsers
    {
        [JsonProperty("data")]
        [JsonPropertyName("data")]
        public List<TwitchUser> Users { get; set; }
    }
}

using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;

namespace StreamServices.Core
{
    public class AppAccessToken : TableEntity
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresInSeconds { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        public DateTime ExpiresAtUTC { get; set; }
    }

}

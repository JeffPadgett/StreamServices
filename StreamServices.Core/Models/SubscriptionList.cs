using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core.Models
{
    public class SubscriptionList
    {
        [JsonProperty("data")]
        public List<Subscription> Subscriptions { get; set; }
        [JsonProperty("total")]
        public string Total { get; set; }
        [JsonProperty("total_cost")]
        public string TotalCost { get; set; }
        [JsonProperty("max_total_cost")]
        public string MaxTotalCost { get; set; }
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
    }
}

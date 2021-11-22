using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core.Models
{
    public class TotalSubscriptions
    {
        [JsonProperty("data")]
        public Subscription[] Data { get; set; }
        [JsonProperty("Total")]
        public int Total { get; set; }
        [JsonProperty("total_cost")]
        public int TotalCost { get; set; }
        [JsonProperty("max_total_cost")]
        public int MaxTotalCost { get; set; }
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }

    }
}

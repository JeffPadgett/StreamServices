using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core
{

    public class ChallengeJson
    {
        [JsonProperty("challenge")]
        public string Challenge { get; set; }
        public Subscription Subscription { get; set; }
    }

}

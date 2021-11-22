using System;
using System.Collections.Generic;
using System.Text;

namespace StreamServices.Core.Models
{
    public class StreamStatusJson
    {
        public Subscription Subscription { get; set; }
        public Event Event { get; set; }
    }
}

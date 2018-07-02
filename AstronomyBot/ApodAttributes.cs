using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Cosmo
{
    public class ApodAttributes
    {
        [JsonProperty("explanation")]
        public string Explanation { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }
    }
}
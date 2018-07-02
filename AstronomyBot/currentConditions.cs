using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace AstronomyBot
{
    public class currentConditions
    {
        [JsonProperty("weather")]
        public IList<Weather> Weather { get; set; }

        [JsonProperty("main")]
        public Main Main { get; set; }

        [JsonProperty("results")]
        public Results results { get; set; }

        [JsonProperty("explanation")]

        public string Explanation { get; set; }

        [JsonProperty("name")]

        public string CityName { get; set; }
      

    }



    public class Weather

    {

        [JsonProperty("main")]

        public string Main { get; set; }

    }



    public class Main

    {

        [JsonProperty("temp")]

        public double Temperature { get; set; }
        
    }

    public class Results
    {
        [JsonProperty("sunrise")]
        public string Sunrise { get; set; }

        [JsonProperty("sunset")]
        public string Sunset { get; set; }

    }
}
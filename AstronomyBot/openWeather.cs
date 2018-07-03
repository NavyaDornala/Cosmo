using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AstronomyBot
{
    public class OpenWeather

    {

        private const string APIKEY = "6152def9feb0674a20f2c6257a248603";

        public static async Task<currentConditions> GetWeatherAsync(string placeName)

        {
            //Uri url = new Uri(string.Format("https://api.nasa.gov/planetary/apod?api_key=NNKOjkoul8n1CH18TWA9gwngW1s1SmjESPjNoUFo"));
            Uri ServiceURL = new Uri(string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&APPID={1}&units=imperial", placeName, APIKEY));
           // Uri url = new Uri(string.Format("https://api.nasa.gov/planetary/apod?api_key=NNKOjkoul8n1CH18TWA9gwngW1s1SmjESPjNoUFo"));
            var client = new HttpClient();
            
            HttpResponseMessage response = await client.GetAsync(ServiceURL);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            currentConditions conditions = JsonConvert.DeserializeObject<currentConditions>(result);



            return conditions;

        }
        public static async Task<currentConditions> GetSunAsync()

        {
            Uri url = new Uri(string.Format(" https://api.sunrise-sunset.org/json?lat=36.7201600&lng=-4.4203400"));
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            currentConditions conditions = JsonConvert.DeserializeObject<currentConditions>(result);
            return conditions;

        }
       

    }
}
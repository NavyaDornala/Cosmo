using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;


namespace AstronomyBot

{
    public class Apod
    {
        public static async Task<ApodAttributes> GetDetails()

        {

            Uri url = new Uri(string.Format("https://api.nasa.gov/planetary/apod?api_key=ASkiH6sOHY8alzAxulfwz16ZvOrrr4t2isH6qEHN"));
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result =  await response.Content.ReadAsStringAsync();
            ApodAttributes apod = JsonConvert.DeserializeObject<ApodAttributes>(result);
            return apod;

        }
        
    }
}
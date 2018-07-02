using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;


namespace Cosmo

{
    public class Apod
    {
        public static async Task<ApodAttributes> GetDetails()

        {

            Uri url = new Uri(string.Format("https://api.nasa.gov/planetary/apod?api_key=rXDVLSbsZjhcNkrs0dl52tPbrpiI2g9xGSW7vHpo"));
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result =  await response.Content.ReadAsStringAsync();
            ApodAttributes apod = JsonConvert.DeserializeObject<ApodAttributes>(result);
            return apod;

        }
        
    }
}
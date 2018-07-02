using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AstronomyBot
{
    public class LuisMain
    {
        private static async Task<LuisConnect> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            LuisConnect Data = new LuisConnect();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/6e0e7f8a-3483-41fe-a5ab-a576c6b616d3?subscription-key=ec622bf0f30f46a6bc27ddd75ecb57f3&spellCheck=true&bing-spell-check-subscription-key={YOUR_BING_KEY_HERE}&verbose=true&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LuisConnect>(JsonDataResponse);
                }
            }
            return Data;
        }
    }
}
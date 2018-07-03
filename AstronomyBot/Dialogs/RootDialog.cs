using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace AstronomyBot.Dialogs
{


    [LuisModel("364a2880-2fa0-4c7c-a2b7-d4a25b740856", "c23e0248ff6f437fa1b3dd9585a0c7ba")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private const string ShowInternetAttachment = "(3) Show Internal attachment";
        
        [LuisIntent("")]
        [LuisIntent("None")]

        public async Task NoneIntent(IDialogContext context, LuisResult result)

        {
            string message = $"Sorry, I did not understand '{result.Query}'.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);

           
        }
        [LuisIntent("Wikipedia")]
       public async Task WikiIntent(IDialogContext context, LuisResult result)
       {
      
           var webClient = new WebClient();
           var pageSourceCode = webClient.DownloadString("http://en.wikipedia.org/w/api.php?format=xml&action=query&prop=extracts&exsentences=2&titles=" + result.Query + "&redirects=true");
           XmlDocument doc = new XmlDocument();
           doc.LoadXml(pageSourceCode);
           var fnode = doc.GetElementsByTagName("extract")[0];
           try

           {
               var replyMessage = context.MakeMessage();
               string ss = fnode.InnerText;
               Regex regex = new Regex("\\<[^\\>]*\\>");
               string.Format("Before:{0}", ss);
               ss = regex.Replace(ss, string.Empty);
               string res = String.Format(ss);
                Attachment attachment = await GetWikiAttachemnt(res,result.Query);
                replyMessage.Attachments = new List<Attachment> { attachment };
                await context.PostAsync(replyMessage);
            }

           catch (Exception)
           {
               await context.PostAsync("Try about astronomy");
           }

       }


        int countjoke = 0;
        int countfact = 0;

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            var activity = await result as Activity;
            if (activity.Text.Equals("help"))
            {
                await context.PostAsync("Cosmo would tell you:\nAstronomy picture of the day\nFun Facts on astronomy\nJokes on astronomy\nSunrise and sunset timings");
            }

        }
        

        [LuisIntent("Jokes")]
        public async Task JokeIntent(IDialogContext context, LuisResult result)
        {
            string path1 = @"C:\Users\NAVYA\source\repos\AstronomyBot\AstronomyBot\Joke.xlsx";
            _Application excel = new _Excel.Application();
            Workbook wb;
            Worksheet ws;
            wb = excel.Workbooks.Open(path1);
            ws = wb.Worksheets[1];
            
            Random number1 = new Random();
            int i = number1.Next(1, 32);
            if (ws.Cells[i, 1].Value2 != null)
            {
                if (countjoke == 0)
                {
                    await context.PostAsync("I've got one for you: ");
                }
                countjoke++;
                await context.PostAsync($"Q : {ws.Cells[i, 1].Value2}\n A : {ws.Cells[i, 2].Value2}");
            }

        }
        
        [LuisIntent("Facts")]
        public async Task FactIntent(IDialogContext context, LuisResult result)
        {
            string[] lines = File.ReadAllLines(@"C:\Users\NAVYA\source\repos\AstronomyBot\AstronomyBot\Facts.txt");
            Random number = new Random();
            int num = number.Next(1, 57);
            if (countfact == 0)
            {
                await context.PostAsync("Here's something interesting:");
            }
            countfact++;
            await context.PostAsync($"{lines[num]}");
        }
        [LuisIntent("Sunset")]
        public async Task SunsetIntent(IDialogContext context, LuisResult result)
        {
            currentConditions conditions = await OpenWeather.GetSunAsync();
            await context.PostAsync(string.Format("Today's sunset Time : {0}", conditions.results.Sunset));
            //context.Wait(MessageReceivedAsync);

        }

        [LuisIntent("Sunrise")]
        public async Task SunriseIntent(IDialogContext context,LuisResult result)
        {
            currentConditions conditions = await OpenWeather.GetSunAsync();
            await context.PostAsync(string.Format("Today sunrise Time : {0}", conditions.results.Sunrise));
           // context.Wait(MessageReceivedAsync);
        }
        [LuisIntent("Apod")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            var replyMessage = context.MakeMessage();
            ApodAttributes apod = await Apod.GetDetails();
            Attachment attachment = await GetProfileHeroCardAsync(apod.url);
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
            // context.Wait(MessageReceivedAsync);
        }
        
        public async Task<Attachment> GetWikiAttachemnt(string res,string query)
        {
            // ApodAttributes apod = await Apod.GetDetails();
            var heroCard = new HeroCard
            {
                Subtitle = res,
               // Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://antwrp.gsfc.nasa.gov/apod/astropix.html"),
                 Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://en.wikipedia.org/wiki/"+ query) }
            };
            return heroCard.ToAttachment();
        }
        public async Task<Attachment> GetProfileHeroCardAsync(string url)
        {
            ApodAttributes apod = await Apod.GetDetails();
            var heroCard = new HeroCard
            {
                Title = string.Format(apod.title),
                Subtitle = string.Format(apod.Explanation),
                Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://antwrp.gsfc.nasa.gov/apod/astropix.html"),
                Images = new List<CardImage> { new CardImage(apod.url) },
                // Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://www.devenvexe.com"), new CardAction(ActionTypes.OpenUrl, "C# Corner", value: "http://www.c-sharpcorner.com/members/suthahar-j"), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "https://social.msdn.microsoft.com/profile/j%20suthahar/") }
            };
            return heroCard.ToAttachment();

        }
    }
}
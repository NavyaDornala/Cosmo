using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using System.Web;
using System.Xml;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
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
        int countfact = 0, countjoke = 0;
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'.";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Greeting")]
        public async Task MessageIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hey! Try this : \nAstronomy picture of the day(apod)\nFun Facts on astronomy\nJokes on astronomy\nSunrise and sunset timings");
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
                Attachment attachment = await GetWikiAttachemnt(res, result.Query);
                replyMessage.Attachments = new List<Attachment> { attachment };
                await context.PostAsync(replyMessage);
            }
            catch (Exception)
            {
                await context.PostAsync("Try about astronomy");
            }
        }



        /*string path1 = @"C:\Users\NAVYA\source\repos\AstronomyBot\AstronomyBot\Joke.xlsx";
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
          }*/
        
        [LuisIntent("Jokes")]
        public async Task JokeIntent(IDialogContext context, LuisResult result)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Microsoft.Azure.CloudConfigurationManager.GetSetting("cosmostorageaccount_AzureStorageConnectionString"));
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference("files");
            if (share.Exists())
            {
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("AstroFiles");
                if (sampleDir.Exists())
                {
                    CloudFile sourceFile = sampleDir.GetFileReference("Joke.txt");

                    if (sourceFile.Exists())
                    {
                        if (countjoke == 0)
                        {
                            await context.PostAsync("I've got one for you: ");
                        }
                        countjoke++;
                        var lines = string.Format(sourceFile.DownloadText());
                        Random number = new Random();
                        int num = number.Next(1, 64);
                        var linesArray = lines.Split('.');
                        await context.PostAsync($"{linesArray[num]}");
                    
                }
                }
            }

        }

        [LuisIntent("Facts")]
        public async Task FactIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
           
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                Microsoft.Azure.CloudConfigurationManager.GetSetting("cosmostorageaccount_AzureStorageConnectionString"));
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
            CloudFileShare share = fileClient.GetShareReference("files");
            if (share.Exists())
            {
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("AstroFiles");
                if (sampleDir.Exists())
                {
                    CloudFile sourceFile = sampleDir.GetFileReference("Facts.txt");

                    if (sourceFile.Exists())
                    {
                        if (countfact == 0)
                        {
                            await context.PostAsync("Here's something interesting:");
                        }
                        countfact++;
                        var lines = string.Format(sourceFile.DownloadText());
                        var linesArray = lines.Split('.');                    
                        Random number = new Random();
                        int num = number.Next(1, 57);
                        await context.PostAsync($"{linesArray[num]}");
                    }
                }
            }
        }


        [LuisIntent("Sunset")]
        public async Task SunsetIntent(IDialogContext context, LuisResult result)
        {
            currentConditions conditions = await OpenWeather.GetSunAsync();
            await context.PostAsync(string.Format("Today's sunset Time : {0}", conditions.results.Sunset));
        }

        [LuisIntent("Sunrise")]
        public async Task SunriseIntent(IDialogContext context, LuisResult result)
        {
            currentConditions conditions = await OpenWeather.GetSunAsync();
            await context.PostAsync(string.Format("Today sunrise Time : {0}", conditions.results.Sunrise));
        }

        [LuisIntent("Apod")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            var replyMessage = context.MakeMessage();
            ApodAttributes apod = await Apod.GetDetails();
            Attachment attachment = await GetProfileHeroCardAsync(apod.url);
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }

        public async Task<Attachment> GetWikiAttachemnt(string res, string query)
        {
            var heroCard = new HeroCard       
            {
                Subtitle = res,
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: "http://en.wikipedia.org/wiki/" + query) }
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
            };
            return heroCard.ToAttachment();
        }
    }
}
using System;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace AstronomyBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
           public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;

       }
       
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result){
            var activity = await result as Activity;
            if (activity.Text.Equals("Apod"))
            {
                var replyMessage = context.MakeMessage();
                ApodAttributes apod = await Apod.GetDetails();
                Attachment attachment = await  GetProfileHeroCardAsync(apod.url);
                replyMessage.Attachments = new List<Attachment> { attachment };
                await context.PostAsync(replyMessage);
                context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text.Equals("joke"))
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
                    await context.PostAsync("I've got one for you: ");
                    await context.PostAsync($"Q : {ws.Cells[i, 1].Value2}\n A : {ws.Cells[i, 2].Value2}");
                }
            }
            else if (activity.Text.Equals("fact"))
            {
                string[] lines = System.IO.File.ReadAllLines(@"C:\Users\NAVYA\source\repos\AstronomyBot\AstronomyBot\Facts.txt");
                Random number = new Random();
                int num = number.Next(1, 57);
                await context.PostAsync("Here's something interesting:");
                await context.PostAsync($"{lines[num]}");
            }
            else if (activity.Text.Equals("sunrise"))
            {
                currentConditions conditions = await OpenWeather.GetSunAsync();
                await context.PostAsync(string.Format("Today sunrise Time : {0}", conditions.results.Sunrise));
                context.Wait(MessageReceivedAsync);

            }
            else if (activity.Text.Equals("sunset"))
            {
                currentConditions conditions = await OpenWeather.GetSunAsync();
                await context.PostAsync(string.Format("Today's sunset Time : {0}", conditions.results.Sunset));
                context.Wait(MessageReceivedAsync);

            }
            else if (activity.Text.Equals("title"))
            {
                ApodAttributes apod = await Apod.GetDetails();
                await context.PostAsync(string.Format("Title : {0}", apod.title));
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                currentConditions conditions = await OpenWeather.GetWeatherAsync(activity.Text);
               // await context.PostAsync(string.Format(conditions.results.Sunrise));
               await context.PostAsync(string.Format("Current conditions in {1}: {0}. The temperature is {2}\u00B0 C.",conditions.Weather[0].Main,conditions.CityName, (float)(conditions.Main.Temperature - 32) * 5 / 9));
               context.Wait(MessageReceivedAsync);
            }
        }
        private static async Task<Attachment> GetProfileHeroCardAsync(string url)
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
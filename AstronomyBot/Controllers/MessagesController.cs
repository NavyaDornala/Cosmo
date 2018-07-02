using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Configuration;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace AstronomyBot
{
   

        [BotAuthentication]

    public class MessagesController : ApiController
    {
        
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            } 
            else
            {
                HandleSystemMessage(activity);

            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;

        }



        private async Task<Activity> HandleSystemMessage(Activity message)

        {
            if (message.Type == ActivityTypes.ConversationUpdate)
            {
                IConversationUpdateActivity iConversationUpdated = message as IConversationUpdateActivity;
                if (iConversationUpdated != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));
                    foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        // if the bot is added, then   
                        if (member.Id == iConversationUpdated.Recipient.Id)
                        {
                            var reply = ((Activity)iConversationUpdated).CreateReply($"Hi! I'm Cosmo , your astronomy geek.\n How can I help you?");
                            await connector.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            return null;
        }

    }

}
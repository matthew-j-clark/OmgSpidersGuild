using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace SpiderDiscordBot.TextCommands
{
    public class SpiderHelp:IBotCommand
    {
        const int DiscordMaxMessageLength = 2000;
        public string StartsWithKey => "!spiderhelp";
        public string Description => "Prints this help message";
        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var helpList = OmgSpidersBotDriver.CommandList.Select(x => $"{x.Key} : {x.Value.Description}");

            var outputBuilder = new StringBuilder();
            foreach(var helpEntry in helpList)
            {
                if(outputBuilder.Length+helpEntry.Length+1>=DiscordMaxMessageLength)
                {
                    await SendHelpStringMessage(message, outputBuilder);
                    outputBuilder.Clear();
                }
                else
                {
                    outputBuilder.AppendLine(helpEntry);
                }
            }

            await SendHelpStringMessage(message, outputBuilder);

        }

        private static async Task SendHelpStringMessage(SocketMessage message, StringBuilder outputBuilder)
        {
            var helpString = $"```{outputBuilder.ToString()}```";
            await message.Channel.SendMessageAsync(helpString);
        }
    }
}
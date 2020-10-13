using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace SpiderDiscordBot.TextCommands
{
    public class SpiderHelp:IBotCommand
    {
        public string StartsWithKey => "!spiderhelp";
        public string Description => "Prints this help message";
        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var helpList = OmgSpidersBotDriver.CommandList.Select(x => $"{x.Key} : {x.Value.Description}");
            var helpString=string.Join('\n', helpList);
            helpString = $"```{helpString}```";
            await message.Channel.SendMessageAsync(helpString);
        }

    }
}
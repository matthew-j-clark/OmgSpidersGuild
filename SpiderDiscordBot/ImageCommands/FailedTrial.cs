using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace SpiderDiscordBot.ImageCommands
{
    public class FailedTrial:IBotCommand
    {
        public string StartsWithKey => "!failed";
        public string Description => "Post when a trial is failed";
        public async Task ProcessMessageAsync(SocketMessage message)
        {
            await message.Channel.SendMessageAsync("https://tenor.com/view/terminator-hasta-la-vista-baby-shoot-gun-gif-15365750");
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

namespace SpiderDiscordBot.ImageCommands
{    
    public class FailedTrial:AuthorizedCommand
    {        
        public const string Description = "Post when a trial is failed";

        [Command(ignoreExtraArgs: true, text: "failed")]
        [Summary(Description)]
        public async Task ProcessMessageAsync(SocketMessage message)
        {
            await message.Channel.SendMessageAsync("https://tenor.com/view/terminator-hasta-la-vista-baby-shoot-gun-gif-15365750");
        }
    }
}

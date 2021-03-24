using System.Collections.Generic;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

namespace SpiderDiscordBot.ImageCommands
{
    public class ThisIsFine: AuthorizedCommand
    {        
        public const string Description = "THIS IS FINE";

        [Command(ignoreExtraArgs: true, text: "fine")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            await message.Channel.SendMessageAsync("https://tenor.com/view/this-is-fine-gif-5263684");
        }
    }
}

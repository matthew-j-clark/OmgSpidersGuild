using System.Collections.Generic;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

namespace SpiderDiscordBot.ImageCommands
{
    public class NutButtonImage : AuthorizedCommand
    {        
        public const string Description = "NUT NUT NUT UP IN HERE";

        [Command(ignoreExtraArgs: true, text: "nut")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            await message.Channel.SendMessageAsync("https://tenor.com/view/lol-nut-button-gif-14436356");
        }
    }
}
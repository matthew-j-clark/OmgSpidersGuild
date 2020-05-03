using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace OmgSpiders.DiscordBot.ImageCommands
{
    public class ThisIsFine:IBotCommand
    {
        public string StartsWithKey => "!fine";
        public string Description => "THIS IS FINE";
        public async Task ProcessMessageAsync(SocketMessage message)
        {
            await message.Channel.SendMessageAsync("https://tenor.com/view/this-is-fine-gif-5263684");
        }
    }
}

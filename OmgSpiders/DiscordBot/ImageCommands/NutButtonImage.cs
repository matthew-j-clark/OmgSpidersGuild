using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace OmgSpiders.DiscordBot.ImageCommands
{
    public class NutButtonImage:IBotCommand
    {
        public string StartsWithKey => "!nut";
        public async Task ProcessMessageAsync(SocketMessage message)
        {
            await message.Channel.SendMessageAsync("https://tenor.com/view/lol-nut-button-gif-14436356");
        }
    }
}
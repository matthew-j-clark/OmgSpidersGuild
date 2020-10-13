using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SpiderDiscordBot.ImageCommands
{

    [IgnoreCommand]
    public class GenericImageCommand : IBotCommand

    {
        public string StartsWithKey { get; }
        public string Description { get; }

        private List<string> ImageList { get; }
        public GenericImageCommand(string startsWith, string desc, string imageCommaSep)
        {
            // default so it is unlikely to get picked up
            this.StartsWithKey = startsWith;
            this.Description = desc;
            this.ImageList = imageCommaSep.Split(",").ToList();
        }

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            await message.Channel.SendMessageAsync(this.ImageList[new Random().Next(0, this.ImageList.Count)]);
        }
    }
}

using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SpiderDiscordBot.ImageCommands
{
    public class GenericImageCommand : AuthorizedCommand
    {
        [Command(ignoreExtraArgs: true, text: "hr")]
        [Summary("I NEED HR")]
        public async Task ProcessHrImage()
        {
            var message = this.Context.Message;
            await message.Channel.SendMessageAsync("https://tenor.com/view/karen-karening-intensifies-done-iam-done-gif-16742218");
        }

        [Command(ignoreExtraArgs: true, text: "karen")]
        [Summary("KAAAAAARRENNNN")]
        public async Task ProcessKarenImage()
        {
            var message = this.Context.Message;            
            await message.Channel.SendMessageAsync("https://tenor.com/view/snl-hell-naw-no-black-panther-karen-gif-11636970");
        }

        [Command(ignoreExtraArgs: true, text: "ravioli")]
        [Summary("raviolis?")]
        public async Task ProcessRavioliImage()
        {
            var message = this.Context.Message;
            await message.Channel.SendMessageAsync("https://tenor.com/view/trailer-gif-7304634");
        }
    }
}

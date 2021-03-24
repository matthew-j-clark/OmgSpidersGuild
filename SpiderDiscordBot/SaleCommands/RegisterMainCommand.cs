using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    public class RegisterMainCommand : AuthorizedCommand
    {        
        public const string Description = "Register your main toon for payouts \"!registermain <charactername>\"";

        [Command(ignoreExtraArgs: true, text: "registermain")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            var stringSpaced = message.Content.Split(' ');
            if (stringSpaced.Length != 2)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!registermain charactername\"");
                return;
            }

            var discordName = message.Author.Mention;
            var mainToRegister = stringSpaced[1];
            var result = await new PlayerManager().RegisterMain(discordName, mainToRegister);
            await message.Channel.SendMessageAsync(result);
        }
    }
}

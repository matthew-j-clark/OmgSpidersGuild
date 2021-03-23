using Discord.WebSocket;

using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    public class RegisterMainCommand : IBotCommand

    {
        public string StartsWithKey => "!registermain";
        public string Description => "Register your main toon for payouts \"!registermain <charactername>\"";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
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

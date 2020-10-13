using Discord.WebSocket;

using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    public class ClaimPlayerCommand : IBotCommand
    {
        public string StartsWithKey => "!claim";
        public string Description => "Used to claim an alt or character";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var stringSpaced = message.Content.Split(' ');
            if (stringSpaced.Length != 2)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!claim playerName\"");
                return;
            }

            var discordName = message.Author.Mention;
            var playerToClaim = stringSpaced[1];
            var result = await new PlayerRegistrationHandler().ClaimPlayer(playerToClaim, discordName, message.Author.Username);
            await message.Channel.SendMessageAsync(result);

        }
    }
}

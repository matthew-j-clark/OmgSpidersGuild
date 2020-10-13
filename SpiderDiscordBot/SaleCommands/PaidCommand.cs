using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    [AuthorizedGroup("Banana Spider")]
    public class PaidCommand : IBotCommand
    {
        public string StartsWithKey => "!paid";
        public string Description => "Set the player as completely paid out. No partial payments.";

        public async Task ProcessMessageAsync(SocketMessage message)
        {          
            var stringSpaced=message.Content.Split(' ');
            if(stringSpaced.Length!=2)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!paid playerName\"");
                return;
            }
            var userTarget = stringSpaced[1];
            if(message.MentionedUsers.Any())
            {
                userTarget=message.MentionedUsers.First().Mention;
            }
            
            await new PayoutManager().PayoutPlayer(userTarget);
            await message.Channel.SendMessageAsync($"Payout complete for {stringSpaced[1]}.");
        }
    }
}

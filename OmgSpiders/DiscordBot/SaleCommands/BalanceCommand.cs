using Discord.WebSocket;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.SaleCommands
{
    public class BalanceCommand : IBotCommand

    {
        public string StartsWithKey => "!balance";
        public string Description => "Get the balance for yourself (no mention) or another mentioned player.";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var stringSpaced = message.Content.Split(' ');
            var userTarget = message.Author.Mention;
            if (stringSpaced.Length == 2 && message.MentionedUsers.Any())
            {
                userTarget = message.MentionedUsers.First().Mention;
            }
            else if(stringSpaced.Length == 2)
            {
                userTarget = stringSpaced[1];                
            }
            
            var amountOwed = await new PayoutManager().GetBalance(userTarget);
            await message.Channel.SendMessageAsync($"{userTarget} is owed {amountOwed} gold.");
        }
    }
}

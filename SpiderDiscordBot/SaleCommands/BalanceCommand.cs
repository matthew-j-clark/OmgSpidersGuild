using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    public class BalanceCommand : AuthorizedCommand
    {        
        public const string Description = "Get the balance for yourself (no mention) or another mentioned player.";

        [Command(ignoreExtraArgs: true, text: "balance")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
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

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
            var discordMentionToCheck = message.Author.Mention;
            if (stringSpaced.Length == 2 && message.MentionedUsers.Any())
            {
                discordMentionToCheck = message.MentionedUsers.First().Mention;
            }

            var amountOwed = await new PayoutManager().GetBalance(discordMentionToCheck);
            await message.Channel.SendMessageAsync($"{discordMentionToCheck} is owed {amountOwed} gold.");


        }
    }
}

using Discord.WebSocket;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.SaleCommands
{
    public class HistoryCommand : IBotCommand

    {
        public string StartsWithKey => "!history";
        public string Description => "Get the total historical earnings (paid out and not) for a user.";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var stringSpaced = message.Content.Split(' ');
            var discordMentionToCheck = message.Author.Mention;
            if (stringSpaced.Length == 2 && message.MentionedUsers.Any())
            {
                discordMentionToCheck = message.MentionedUsers.First().Mention;
            }

            var totalEarnings = await new PayoutManager().GetHistory(discordMentionToCheck);
            await message.Channel.SendMessageAsync($"{discordMentionToCheck} has earned an all time amount of {totalEarnings} gold.");

        }
    }
}

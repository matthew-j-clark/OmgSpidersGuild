using Discord.WebSocket;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    public class HistoryCommand : IBotCommand

    {
        public string StartsWithKey => "!history";
        public string Description => "Get the total historical earnings (paid out and not) for a user.";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var stringSpaced = message.Content.Split(' ');
            var userTarget = message.Author.Mention;
            if (stringSpaced.Length == 2 && message.MentionedUsers.Any())
            {
                userTarget = message.MentionedUsers.First().Mention;
            }
            else if (stringSpaced.Length == 2)
            {
                userTarget = stringSpaced[1];
            }

            var totalEarnings = await new PayoutManager().GetHistory(userTarget);
            await message.Channel.SendMessageAsync($"{userTarget} has earned an all time amount of {totalEarnings} gold.");

        }
    }
}

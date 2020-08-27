using Discord.WebSocket;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.SaleCommands
{
    public class CalculatePayoutsCommand : IBotCommand
    {
        public string StartsWithKey => "!calculatepayouts";
        public string Description => "Retrieves the list of who still needs to be paid out.";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            if (!message.Author.Username.Contains("SealSlicer"))
            {
                await message.Channel.SendMessageAsync("Unauthorized user access of command \"calculatepayouts\"");
                
            }

            var payouts = await new PayoutManager().GetPayouts();

            var output = new StringBuilder();
            output.AppendLine("```");
            output.AppendLine("----------------------------");
            output.AppendLine("|   Player   | Amount Owed |");
            output.AppendLine("----------------------------");
            foreach(var payout in payouts.OrderBy(x=>x.Key))
            {
                output.AppendLine(string.Format("|{0,12}|{1,13}|", payout.Key, payout.Value));                
            }

            output.AppendLine("----------------------------");
            output.AppendLine("```");
            await message.Channel.SendMessageAsync(output.ToString());

        }
    }
}

using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    public class CalculatePayoutsCommand : AuthorizedCommand
    {        
        public const string Description = "Retrieves the list of who still needs to be paid out.";

        [Command(ignoreExtraArgs: true, text: "calculatepayouts")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            var payouts = await new PayoutManager().GetPayouts();

            var output = new StringBuilder();
            output.AppendLine("```");
            output.AppendLine("--------------------------------");
            output.AppendLine("|     Player     | Amount Owed |");
            output.AppendLine("--------------------------------");
            foreach(var payout in payouts.OrderBy(x=>x.Key))
            {
                output.AppendLine(string.Format("|{0,16}|{1,13}|", payout.Key, payout.Value));                
            }

            output.AppendLine("--------------------------------");
            output.AppendLine("If your toon is surrounded in **, you need to do !registermain and !claim for your toons");
            output.AppendLine("```");
            
            await message.Channel.SendMessageAsync(output.ToString());

        }
    }
}

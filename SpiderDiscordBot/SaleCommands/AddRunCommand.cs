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
    public class AddRunCommand : IBotCommand
    {
        public string StartsWithKey => "!addrun";
        public string Description => "Adds a run to the database.Format: \n" +
            "!addrun title \n" +
            "gold amount after ad cuts\n" +
            "cutValue- defaults to 1\n"+
            "player1\n" +
            "player2\n" +
            "cutValue2 - allows multiple cut values\n" +
            "player3\n" +
            "etc";

        public async Task ProcessMessageAsync(SocketMessage message)
        { 
            var lines = message.ToString().Split('\n');

            if (lines.Length < 3)
            {
                await message.Channel.SendMessageAsync("Invalid run format. Not enough information to determine run details.");
                return;
            }

            var titleLine = lines[0].Split(" ", 2);

            if (titleLine.Length != 2)
            {
                await message.Channel.SendMessageAsync("Invalid run format. Title should be on first line.");
                return;
            }
            var title = titleLine[1];

            var goldAmountLine = lines[1];
            var goldAmount = 0L;

            if (!long.TryParse(goldAmountLine, out goldAmount))
            {
                await message.Channel.SendMessageAsync("Invalid run format. The gold amount is not a number.");
                return;
            }

            var playerAndCutEntryList = lines.Skip(2).ToArray();

            var runId = await new RunManager().AddRunAsync(title, goldAmount, playerAndCutEntryList);
            await message.Channel.SendMessageAsync($"Run #{runId} recorded successfully! ");

        }
       
    }
}

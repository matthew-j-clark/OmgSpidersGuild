using Discord.WebSocket;

using OmgSpiders.DiscordBot.Authorization;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.SaleCommands
{
    [AuthorizedGroup("Banana Spider")]
    public class UpdateRunCommand : IBotCommand
    {
        public string StartsWithKey => "!updaterun";
        public string Description => "Updates a run in the database.Format: \n" +
            "!updaterun title \n" +
            "RunId\n"+
            "gold amount after ad cuts\n" +
            "player1\n" +
            "player2\n" +
            "etc";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var lines = message.ToString().Split('\n');
            if (lines.Length < 4)
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
            var runNumberLine = lines[1];
            if (!int.TryParse(runNumberLine, out int runId))
            {
                await message.Channel.SendMessageAsync("Invalid run format. The Run ID is not a number.");
                return;
            }

            var title = titleLine[1];

            var goldAmountLine = lines[2];
            var goldAmount = 0L;

            if (!long.TryParse(goldAmountLine, out goldAmount))
            {
                await message.Channel.SendMessageAsync("Invalid run format. The gold amount is not a number.");
                return;
            }

            var playerList = lines.Skip(3).ToArray();

            try
            {
                var newRunId = await new RunManager().UpdateRun(title, goldAmount, runId, playerList);
                await message.Channel.SendMessageAsync($"Run #{newRunId} recorded successfully!");
            }
            catch(Exception ex)
            {

            }
        }
    }
}

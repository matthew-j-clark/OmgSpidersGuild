using Discord.WebSocket;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.SaleCommands
{
    public class RunDetails : IBotCommand
    {
        public string StartsWithKey => "!rundetails";
        public string Description => "!rundetails list to get the list of runs or !rundetails number to get run details.";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var stringSpaced = message.Content.Split(' ');
            if (stringSpaced.Length != 2)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!rundetails list\" or \"!rundetails number\"");
                return;
            }

            var resultBuilder = new StringBuilder();
            if (stringSpaced[1].Equals("list", StringComparison.OrdinalIgnoreCase))
            {
                var runList = await new RunManager().ListRuns();

                resultBuilder.AppendLine("```");

                foreach (var run in runList)
                {
                    resultBuilder.AppendLine($"RunId: {run.Id}. Title: {run.RunName}. Gold Amount: {run.GoldTotalAfterAdCut}. Date Added: {run.RunDate}.");
                }

                resultBuilder.AppendLine("```");                
            }
            else if (int.TryParse(stringSpaced[1], out var runId))
            {
                var runDetails = await new RunManager().GetRunDetails(runId);

                if (runDetails == null)
                {
                    resultBuilder.AppendLine($"{runId} not found, try \"!rundetails list\" to get a list of runs.");
                    return;
                }
                else
                {
                    resultBuilder.AppendLine($"RunId: {runDetails.Id}. Title: {runDetails.RunName}.Gold Amount: {runDetails.GoldTotalAfterAdCut}. Date Added: {runDetails.RunDate}.");
                    resultBuilder.AppendLine("Players Involved:");
                    foreach (var player in runDetails.SaleRunParticipation.Select(x => x.Player.DiscordMention ?? x.Player.PlayerName).Distinct())
                    {
                        resultBuilder.AppendLine(player);
                    }
                }
            }

            await message.Channel.SendMessageAsync(resultBuilder.ToString());


        }
    }
}

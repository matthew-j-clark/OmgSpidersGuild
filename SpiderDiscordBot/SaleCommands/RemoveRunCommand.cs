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
    public class RemoveRunCommand : IBotCommand
    {
        public string StartsWithKey => "!removerun";
        public string Description => "Used to delete a run !removerun runid";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var messageSplit = message.Content.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (messageSplit.Length != 2)
            {
                await message.Channel.SendMessageAsync($"Invalid usage of !removerun, bad format");
            }

            if (!int.TryParse(messageSplit[1], out int runId))
            {
                await message.Channel.SendMessageAsync($"Invalid usage of !removerun, runId is not a number.");
            }

            await new RunManager().RemoveRunAsync(runId);
            await message.Channel.SendMessageAsync($"Run #{runId} removed successfully! ");
        }
    }
}

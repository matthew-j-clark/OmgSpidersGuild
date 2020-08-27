using Discord.WebSocket;

using SpiderSalesDatabase.SaleRunOperations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.SaleCommands
{
    public class PaidCommand : IBotCommand
    {
        public string StartsWithKey => "!paid";
        public string Description => "Set the player as completely paid out. No partial payments.";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            if (!message.Author.Username.Contains("SealSlicer"))
            {
                await message.Channel.SendMessageAsync("Unauthorized user access of command \"paid\"");
                return;
            }
            var stringSpaced=message.Content.Split(' ');
            if(stringSpaced.Length!=2)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!paid playerName\"");
                return;
            }

            
            await new PayoutManager().PayoutPlayer(message.MentionedUsers.First().Mention);
            await message.Channel.SendMessageAsync($"Payout complete for {stringSpaced[1]}.");
        }
    }
}

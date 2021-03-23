using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

using SpiderSalesDatabase.SaleRunOperations;
using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    [AuthorizedGroup("Banana Spider")]
    public class PaidCommand : IBotCommand
    {
        public string StartsWithKey => "!paid";
        public string Description => "Set the player as completely paid out. No partial payments. Comma Separated";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var stringSpaced = message.Content.Split(new char []{' ', '\n'}, 2);
            if(stringSpaced.Length!=2)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!paid player1,player2 or !paid player1\nplayer2\"");
                return;
            }
            var payoutTargetsString = stringSpaced[1];
            var payoutTargets = payoutTargetsString.Split(',','\n').Select(x => x.Trim()).ToArray();
            var mentionedUsersList = message.MentionedUsers.ToList();
            var payoutManager = new PayoutManager();
            var playerManager = new PlayerManager();
            for (int idx =0; idx<payoutTargets.Length;++idx)
            {
                var currentTarget = payoutTargets[idx];
                if(string.IsNullOrWhiteSpace(currentTarget))
                {
                    continue;
                }
                if(currentTarget.Contains("@"))
                {
                    currentTarget = message.MentionedUsers.First(x => x.Username.Equals(currentTarget.Trim('@'), StringComparison.OrdinalIgnoreCase)).Mention;
                }
                
                await payoutManager.PayoutPlayer(currentTarget);
                await message.Channel.SendMessageAsync($"Payout complete for {await playerManager.GetDiscordMentionForCharacter(currentTarget)}.");                
            }
        }
    }
}

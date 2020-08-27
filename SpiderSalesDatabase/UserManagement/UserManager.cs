using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSalesDatabase.UserManagement
{
    public class UserManager
    {
        public async Task<string> ClaimPlayer(string player, string discordMention, string friendlyName)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                var existingMap = ctx.PlayerList.FirstOrDefault(x => x.PlayerName == player);
                if(existingMap==null)
                {
                    var newRegistration = new PlayerList() { PlayerName = player, DiscordMention = discordMention,FriendlyName=friendlyName };
                    ctx.PlayerList.Add(existingMap);
                }
                if (!string.IsNullOrEmpty(existingMap.DiscordMention) && discordMention!=existingMap.DiscordMention)
                {
                    return $"The mapping already exists currently {existingMap} owns {player}.";
                }
                else
                {
                    existingMap.DiscordMention = discordMention;
                    existingMap.FriendlyName = friendlyName;
                    ctx.PlayerList.Update(existingMap);
                }
                await ctx.SaveChangesAsync();
                return $"{player} has been claimed by {discordMention} with friendlyname {friendlyName}";

            }
        }
    }
}

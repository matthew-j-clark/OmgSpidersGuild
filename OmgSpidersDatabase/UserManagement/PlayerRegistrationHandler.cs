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
    public class PlayerRegistrationHandler
    {
        public async Task<string> ClaimPlayer(string player, string discordMention, string friendlyName)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                var existingMap = ctx.PlayerList.FirstOrDefault(x => x.PlayerName == player);
                if(existingMap==null)
                {
                    var newRegistration = new PlayerList() { PlayerName = player, DiscordMention = discordMention,FriendlyName=friendlyName };
                    ctx.PlayerList.Add(newRegistration);
                }
                else if (!string.IsNullOrEmpty(existingMap.DiscordMention) && discordMention!=existingMap.DiscordMention)
                {
                    return $"The mapping already exists currently {existingMap.DiscordMention} owns {player}.";
                }
                else
                {
                    existingMap.DiscordMention = discordMention;
                    existingMap.FriendlyName = friendlyName;
                    ctx.PlayerList.Update(existingMap);
                }
                await ctx.SaveChangesAsync();
                var result = $"{player} has been claimed by {discordMention} with friendlyname {friendlyName}";

                result+= "\n"+await RegisterMain(discordMention, player,true);

                return result;
            }
        }

        public async Task<string> RegisterMain(string discordMention, string mainName, bool onlyIfNew = false)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                bool changeMade = false;
                var existingMap = ctx.MainRegistration.FirstOrDefault(x => x.DiscordMention == discordMention);
                if (existingMap == null)
                {
                    var newRegistration = new MainRegistration() { MainName = mainName, DiscordMention = discordMention};
                    ctx.MainRegistration.Add(newRegistration);
                    changeMade = true;
                }
                else if (!string.IsNullOrEmpty(existingMap.DiscordMention) && discordMention != existingMap.DiscordMention)
                {
                    return $"The mapping already exists currently {existingMap.DiscordMention} owns {mainName}.";
                }
                else if(!onlyIfNew)
                {
                    existingMap.DiscordMention = discordMention;
                    existingMap.MainName = mainName;
                    ctx.MainRegistration.Update(existingMap);
                    changeMade = true;
                }
                await ctx.SaveChangesAsync();
                if (changeMade)
                {
                    return $"{mainName} has been registered as the main for {discordMention} and will receive future payouts.";
                }
                return string.Empty;
                
            }
            
        }
    }
}

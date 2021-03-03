using Microsoft.EntityFrameworkCore;

using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSalesDatabase.SaleRunOperations
{
    public class PayoutManager
    {
        public async Task<Dictionary<string, long>> GetPayouts()
        {
            var payouts = new Dictionary<string, long>();
            using (var ctx = new OmgSpidersDbContext())
            {
                var payoutNeeded = ctx.SaleRunParticipation.Include(x => x.Player).Include(x=>x.Run).Where(x => x.Paid == false).ToList();
                var runIds = payoutNeeded.Select(x => x.RunId).Distinct();
                var runMap = ctx.SaleRun.Where(x => runIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x);
                var playerIds = payoutNeeded.Select(x => x.PlayerId).Distinct();
                var mainMap = ctx.MainRegistration.ToDictionary(x => x.DiscordMention, x => x);

                foreach (var payoutEntry in payoutNeeded)
                {
                    var playerEntry = payoutEntry.Player;
                    var mainEntry = playerEntry.DiscordMention!=null&&mainMap.ContainsKey(playerEntry.DiscordMention) ? mainMap[playerEntry.DiscordMention].MainName: null;
                    var playerNameOrFriendlyName = string.IsNullOrEmpty(playerEntry.FriendlyName) ? playerEntry.PlayerName : playerEntry.FriendlyName;
                    var payoutKey = string.IsNullOrEmpty(mainEntry) ? $"**{playerNameOrFriendlyName}**" : mainEntry;

                    if (!payouts.ContainsKey(payoutKey))
                    {
                        payouts[payoutKey] = 0;
                    }
                    var run = runMap[payoutEntry.RunId];
                    payouts[payoutKey] += this.CalculatePlayerCutForRun(payoutEntry);
                }
            }

            return payouts;
        }

        public async Task PayoutPlayer(string userTarget)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                IQueryable<SaleRunParticipation> payoutNeeded = null;

                userTarget = UserUtilities.ConvertUserTargetToDiscordMention(userTarget, ctx);

                payoutNeeded = ctx.SaleRunParticipation.Where(x => x.Paid == false && x.Player.DiscordMention == userTarget);
                foreach (var runEntry in payoutNeeded)
                {
                    runEntry.Paid = true;
                }

                ctx.SaleRunParticipation.UpdateRange(payoutNeeded);
                await ctx.SaveChangesAsync();

            }
        }

       

        public async Task<long> GetBalance (string userTarget)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                userTarget = UserUtilities.ConvertUserTargetToDiscordMention(userTarget, ctx);

                if(string.IsNullOrEmpty(userTarget))
                {
                    return 0;
                }

                var payoutNeeded = ctx.SaleRunParticipation.Include(x => x.Player).Include(x => x.Run).Where(x => x.Paid == false && x.Player.DiscordMention==userTarget).ToList();

                var totalOwed = payoutNeeded.Sum(x => CalculatePlayerCutForRun(x));
                return totalOwed;
            }
        }
        private long CalculatePlayerCutForRun(SaleRunParticipation participationEntry)
        {
            var totalCutCount = participationEntry.Run.SaleRunParticipation.Sum(x => x.CutValue);
            return (long)(participationEntry.Run.GoldTotalAfterAdCut / totalCutCount * participationEntry.CutValue);
        }
        public async Task<long> GetHistory(string userTarget)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                userTarget = UserUtilities.ConvertUserTargetToDiscordMention(userTarget, ctx);
                var payoutNeeded = ctx.SaleRunParticipation.Include(x => x.Player).Include(x => x.Run).Where(x => x.Player.DiscordMention == userTarget).ToList();

                var totalOwed = payoutNeeded.Sum(x => x.Run.GoldTotalAfterAdCut / x.Run.PlayerCount);
                return totalOwed.Value;
            }
        }
    }
    
}

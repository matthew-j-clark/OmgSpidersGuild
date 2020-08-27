﻿using System;
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
                var payoutNeeded = ctx.SaleRunParticipation.Where(x => x.Paid == false);
                var runIds = payoutNeeded.Select(x => x.RunId).Distinct();
                var runMap = ctx.SaleRun.Where(x => runIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x);


                foreach (var payoutEntry in payoutNeeded)
                {
                    if (!payouts.ContainsKey(payoutEntry.Player))
                    {
                        payouts[payoutEntry.Player] = 0;
                    }
                    var run = runMap[payoutEntry.RunId];
                    payouts[payoutEntry.Player] += (long)(run.GoldTotalAfterAdCut / run.PlayerCount);
                }
            }

            return payouts;
        }

        public async Task PayoutPlayer(string player)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                var payoutNeeded = ctx.SaleRunParticipation.Where(x => x.Paid == false && x.Player == player);
                
                foreach(var runEntry in payoutNeeded)
                {
                    runEntry.Paid = true;
                }

                ctx.SaleRunParticipation.UpdateRange(payoutNeeded);
                await ctx.SaveChangesAsync();
                
            }
        }
    }
    
}
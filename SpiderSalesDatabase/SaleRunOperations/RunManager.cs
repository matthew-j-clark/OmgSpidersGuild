using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSalesDatabase.SaleRunOperations
{
    public class RunManager
    {
        public async Task<List<SaleRun>> ListRuns()
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                return ctx.SaleRun.ToList();
            }
        }

        public async Task<SaleRun> GetRunDetails(int runId)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                return ctx.SaleRun.Include(x => x.SaleRunParticipation).ThenInclude(x => x.Player).FirstOrDefault(x=>x.Id==runId);
            }
        }

        public async Task RemoveRunAsync(int runId)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                var saleRunToRemove = ctx.SaleRun.Include(x => x.SaleRunParticipation).FirstOrDefault(x=>x.Id==runId);
                if(saleRunToRemove==null)
                {
                    throw new InvalidOperationException($"Run does not exist {runId}");
                }

                ctx.SaleRun.Remove(saleRunToRemove);

                
                await ctx.SaveChangesAsync();
                
            }
        }

        public async Task<int> AddRunAsync(string title, long goldAmount, string[] playerList)
        {
            for (int idx = 0; idx < playerList.Length; ++idx)
            {
                playerList[idx] = playerList[idx].Trim();
            }

            using (var ctx = new OmgSpidersDbContext())
            {
                var saleRun = new SaleRun()
                {
                    RunName = title,
                    GoldTotalAfterAdCut = goldAmount,
                    RunDate = (DateTime?)DateTime.Now,
                    PlayerCount = playerList.Length
                };

                ctx.SaleRun.Add(saleRun);
                await ctx.SaveChangesAsync();

                saleRun = ctx.SaleRun.Single(
                    x => x.GoldTotalAfterAdCut == saleRun.GoldTotalAfterAdCut
                    && x.RunName == saleRun.RunName
                    && x.RunDate == saleRun.RunDate);

                this.AddNewPlayersNoCommit(playerList, ctx);
                await ctx.SaveChangesAsync();
                // load the players we need now.

                foreach (var player in playerList)
                {
                    var playerEntity = ctx.PlayerList.Single(x => x.PlayerName == player);
                    ctx.SaleRunParticipation.Add(new SaleRunParticipation { RunId = saleRun.Id, PlayerId = playerEntity.Id });
                }

                await ctx.SaveChangesAsync();
                return saleRun.Id;
            }
        }

        public async Task<int> UpdateRun(string title, long goldAmount, int runId, string[] playerList)
        {
            await this.RemoveRunAsync(runId);

            return await this.AddRunAsync(title, goldAmount, playerList);
        }

        private void AddNewPlayersNoCommit(string[] playerList, OmgSpidersDbContext ctx)
        {
            var playersInDb = ctx.PlayerList.ToArray().Select(x => x.PlayerName).Intersect(playerList, StringComparer.OrdinalIgnoreCase);

            var missingPlayers = playerList.Except(playersInDb.Select(x => x), StringComparer.OrdinalIgnoreCase);

            ctx.PlayerList.AddRange(missingPlayers.Select(x => new PlayerList() { PlayerName = x }));

        }
    }
}

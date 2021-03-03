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

        public async Task<int> AddRunAsync(string title, long goldAmount, string[] playerAndCutEntryList)
        {
            for (int idx = 0; idx < playerAndCutEntryList.Length; ++idx)
            {
                playerAndCutEntryList[idx] = playerAndCutEntryList[idx].Trim();
            }

            using (var ctx = new OmgSpidersDbContext())
            {
                var saleRun = new SaleRun()
                {
                    RunName = title,
                    GoldTotalAfterAdCut = goldAmount,
                    RunDate = (DateTime?)DateTime.Now,
                    PlayerCount = playerAndCutEntryList.Length
                };

                ctx.SaleRun.Add(saleRun);
                await ctx.SaveChangesAsync();

                saleRun = ctx.SaleRun.Single(
                    x => x.GoldTotalAfterAdCut == saleRun.GoldTotalAfterAdCut
                    && x.RunName == saleRun.RunName
                    && x.RunDate == saleRun.RunDate);

                this.AddNewPlayersNoCommit(playerAndCutEntryList, ctx);
                await ctx.SaveChangesAsync();
                // load the players we need now.
                AssignPlayerCuts(playerAndCutEntryList, ctx, saleRun);

                await ctx.SaveChangesAsync();
                return saleRun.Id;
            }
        }

        private static void AssignPlayerCuts(string[] playerAndCutEntryList, OmgSpidersDbContext ctx, SaleRun saleRun)
        {
            var cutValue = 1.0f;
            for (var i = 0; i < playerAndCutEntryList.Length; i++)
            {
                var entry = playerAndCutEntryList[i];              
                if (ShouldAddACutBecauseThisEntryIsNotAFloat(cutValue, out cutValue, entry))
                {
                    AddCutForPlayerForTargetRun(ctx, saleRun, entry, cutValue);
                }

            }
        }

        private static bool ShouldAddACutBecauseThisEntryIsNotAFloat(float currentCutValue,out float cutValue, string entry)
        {            
            if(float.TryParse(entry, out cutValue) || string.IsNullOrEmpty(entry))
            {
                return false;
            }
            else
            {
                cutValue = currentCutValue;
                return true;
            }
        }

        private static void AddCutForPlayerForTargetRun(OmgSpidersDbContext ctx, SaleRun saleRun, string player, float cutValue)
        {
            var playerEntity = ctx.PlayerList.Single(x => x.PlayerName == player);

            ctx.SaleRunParticipation.Add(new SaleRunParticipation { RunId = saleRun.Id, PlayerId = playerEntity.Id, CutValue=cutValue });
        }

        public async Task<int> UpdateRun(string title, long goldAmount, int runId, string[] playerAndCutEntryList)
        {
            await this.RemoveRunAsync(runId);

            return await this.AddRunAsync(title, goldAmount, playerAndCutEntryList);
        }

        private void AddNewPlayersNoCommit(string[] playerList, OmgSpidersDbContext ctx)
        {
            var playersInDb = ctx.PlayerList.ToArray().Select(x => x.PlayerName).Intersect(playerList, StringComparer.OrdinalIgnoreCase);

            var missingPlayers = playerList.Except(playersInDb.Select(x => x), StringComparer.OrdinalIgnoreCase)
                .Where(x=>!decimal.TryParse(x, out var dontCareWeOnlyWantTheTryParseResult));

            ctx.PlayerList.AddRange(missingPlayers.Select(x => new PlayerList() { PlayerName = x }));

        }
    }
}

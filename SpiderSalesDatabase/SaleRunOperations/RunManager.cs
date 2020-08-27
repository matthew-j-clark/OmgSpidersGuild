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
        public async Task<int> AddRunAsync(string title, long goldAmount, string[] playerList)
        {
            var playerListAsEntities = playerList.Select(x => new PlayerList() { PlayerName = x });
            using (var ctx = new OmgSpidersDbContext())
            {
                var saleRun = new SaleRun() 
                { 
                    RunName = title, 
                    GoldTotalAfterAdCut = goldAmount, 
                    RunDate = (DateTime?)DateTime.Now, 
                    PlayerCount=playerList.Length 
                };

                ctx.SaleRun.Add(saleRun);
                await ctx.SaveChangesAsync();

                saleRun=ctx.SaleRun.Single(
                    x => x.GoldTotalAfterAdCut == saleRun.GoldTotalAfterAdCut
                    && x.RunName == saleRun.RunName
                    && x.RunDate == saleRun.RunDate);

                this.AddNewPlayersNoCommit(playerList, playerListAsEntities, ctx);
                
                
                foreach(var player in playerListAsEntities)
                {
                    ctx.SaleRunParticipation.Add(new SaleRunParticipation { RunId = saleRun.Id, Player = player.PlayerName });
                }

                await ctx.SaveChangesAsync();
                return saleRun.Id;
            }
        }

        private void AddNewPlayersNoCommit(string[] playerList, IEnumerable<PlayerList> playerListAsEntities, OmgSpidersDbContext ctx)
        {
            var playersInDb = ctx.PlayerList.ToArray()
                                .Intersect(playerListAsEntities)
                                .Select(x => x.PlayerName);

            var missingPlayers = playerList.Except(playersInDb.Select(x => x), StringComparer.OrdinalIgnoreCase);

            ctx.PlayerList.AddRange(missingPlayers.Select(x => new PlayerList() { PlayerName = x }));
        }
    }
}

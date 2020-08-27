using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class PlayerList
    {
        public PlayerList()
        {
            SaleRunParticipation = new HashSet<SaleRunParticipation>();
        }

        public int Id { get; set; }
        public string PlayerName { get; set; }
        public string DiscordMention { get; set; }
        public string FriendlyName { get; set; }

        public virtual ICollection<SaleRunParticipation> SaleRunParticipation { get; set; }
    }
}

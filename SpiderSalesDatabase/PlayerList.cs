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

        public string PlayerName { get; set; }

        public virtual ICollection<SaleRunParticipation> SaleRunParticipation { get; set; }
    }
}

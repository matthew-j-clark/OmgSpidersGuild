using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class SaleRun
    {
        public SaleRun()
        {
            SaleRunParticipation = new HashSet<SaleRunParticipation>();
        }

        public int Id { get; set; }
        public string RunName { get; set; }
        public long? GoldTotalAfterAdCut { get; set; }
        public DateTime? RunDate { get; set; }
        public int? PlayerCount { get; set; }

        public virtual ICollection<SaleRunParticipation> SaleRunParticipation { get; set; }
    }
}

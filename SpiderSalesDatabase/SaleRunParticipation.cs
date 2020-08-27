using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class SaleRunParticipation
    {
        public int Id { get; set; }
        public string Player { get; set; }
        public int RunId { get; set; }
        public long? Bonus { get; set; }
        public long? Penalty { get; set; }
        public bool? Paid { get; set; }
        public int RunnerCount { get; set; }

        public virtual PlayerList PlayerNavigation { get; set; }
        public virtual SaleRun Run { get; set; }
    }
}

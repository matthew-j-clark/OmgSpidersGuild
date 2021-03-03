using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class SaleRunParticipation
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int RunId { get; set; }
        public long? Bonus { get; set; }
        public long? Penalty { get; set; }
        public bool? Paid { get; set; }
        public double CutValue { get; set; }
        public virtual PlayerList Player { get; set; }
        public virtual SaleRun Run { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class SaleRun
    {
        public override bool Equals(object obj)
        {
            return ((SaleRun)obj).Id == this.Id;
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}

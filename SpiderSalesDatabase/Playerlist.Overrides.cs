using Microsoft.EntityFrameworkCore.Migrations;

using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class PlayerList
    {
   
        public override bool Equals(object obj)
        {
            return ((PlayerList)obj).PlayerName.Equals(this.PlayerName, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.PlayerName.GetHashCode();
        }
    }
}

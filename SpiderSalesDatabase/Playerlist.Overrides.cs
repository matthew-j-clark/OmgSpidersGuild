using Microsoft.EntityFrameworkCore.Migrations;

using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class PlayerList
    {
   
        public override bool Equals(object obj)
        {
            return ((PlayerList)obj).Id == this.Id || ((PlayerList)obj).PlayerName==this.PlayerName;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}

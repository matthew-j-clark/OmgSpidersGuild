using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSalesDatabase
{
    public partial class RoleAssignmentReaction
    {
        public int RoleAssignmentId { get; set; }
        public string RoleId { get; set; }
        public string EmoteReference { get; set; }
        public string MessageId { get; set; }
        public string GuildId { get; set; }
        public string ChannelId { get; set; }
    }
}

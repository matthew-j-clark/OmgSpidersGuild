using System;
using System.Collections.Generic;

namespace SpiderSalesDatabase
{
    public partial class RoleAssignmentReaction
    {
        public int RoleAssignmentId { get; set; }
        public string RoleId { get; set; }
        public byte[] EmoteReference { get; set; }
        public string MessageId { get; set; }
        public string GuildId { get; set; }
        public string ChannelId { get; set; }
    }
}

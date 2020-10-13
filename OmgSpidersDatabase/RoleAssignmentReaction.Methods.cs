using SharedModels;

using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSalesDatabase
{
    public partial class RoleAssignmentReaction
    {
       public MessageIdentifier GetMessageIdentifier()
        {
            return new MessageIdentifier { MessageId = this.MessageId, GuildId = this.GuildId, ChannelId = this.ChannelId };
        }
    }
}

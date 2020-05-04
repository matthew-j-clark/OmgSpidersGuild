using System.Collections;
using System.Collections.Generic;
using Discord.Rest;

namespace KeySaleDiscordBot.Interfaces
{
    public interface IBotPendingGroupTracker
    {
        bool AddGroup(IGroupFormationHandler message);
        bool RemoveGroup(ulong id);
        bool RemoveGroup(IGroupFormationHandler message);
        bool TryGetGroup(ulong id, out IGroupFormationHandler message);

        IEnumerable<IGroupFormationHandler> GetAllGroups();
    }
}
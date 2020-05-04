using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Discord.Rest;
using KeySaleDiscordBot.Interfaces;

namespace KeySaleDiscordBot
{
    public class BotPendingPendingGroupTracker : IBotPendingGroupTracker
    {
        public BotPendingPendingGroupTracker()
        {
            this.Messages = new ConcurrentDictionary<ulong, IGroupFormationHandler>();
        }

        private ConcurrentDictionary<ulong, IGroupFormationHandler> Messages{ get; set; }

        public bool AddGroup(IGroupFormationHandler handler) => 
            this.Messages.TryAdd(handler.MessageToTrack.Id, handler);

        public bool RemoveGroup(IGroupFormationHandler message) 
            => this.RemoveGroup(message.MessageToTrack.Id);

        public bool TryGetGroup(ulong id, out IGroupFormationHandler message) => 
            this.Messages.TryGetValue(id, out message);

        public IEnumerable<IGroupFormationHandler> GetAllGroups() => 
            this.Messages.Values;

        public bool RemoveGroup(ulong id) => 
            this.Messages.TryRemove(id, out var unused);
    }
}
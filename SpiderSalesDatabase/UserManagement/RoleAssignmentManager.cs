﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using SharedModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpiderSalesDatabase.UserManagement
{
    public class RoleAssignmentManagerFactory
    {
        private static readonly object padlock = new object();
        private static RoleAssignmentManager ManagerSingleton;
        public RoleAssignmentManager GetRoleAssignmentManager()
        {
            lock (padlock)
            {
                if (ManagerSingleton == null)
                {
                    ManagerSingleton = new RoleAssignmentManager();
                }
                return ManagerSingleton;
            }
        }
    }

    public class RoleAssignmentManager:MemCacheWithEvent<string>
    {
        public const string RoleMapKey = "RoleMap";
        public RoleAssignmentMap RoleMap => this.GetItemByType<RoleAssignmentMap>(RoleMapKey);
        
        public async Task AddRoleAssignmentAsync(string messageId, string emote, string roleToGrant, string guildId, string channelId)
        {
            using (var ctx = new OmgSpidersDbContext())
            {
                var existingEntities = ctx.RoleAssignmentReaction.Where(x => x.MessageId == messageId && x.EmoteReference == emote);
                if (existingEntities.Any())
                {
                    var firstEntity = existingEntities.First();
                    // overwrite for now
                    firstEntity.EmoteReference = emote;
                    firstEntity.RoleId = roleToGrant;
                }
                else
                {
                    ctx.RoleAssignmentReaction.Add(new RoleAssignmentReaction
                    {
                        MessageId = messageId,
                        EmoteReference = emote,
                        RoleId = roleToGrant,
                        GuildId=guildId,
                        ChannelId=channelId
                    });
                }
                await ctx.SaveChangesAsync();
                this.InvalidateItem<RoleAssignmentMap>(RoleMapKey);
            }
        }

        public RoleAssignmentMap GetRoleAssignmentMap()
        {
            var cachedRoleMap = this.RoleMap;
            if (cachedRoleMap!=null)
            {
                return cachedRoleMap; 
            }

            var roleMap = new RoleAssignmentMap();
            using (var ctx = new OmgSpidersDbContext())
            {
                foreach (var reaction in ctx.RoleAssignmentReaction)
                {
                    var messageIdentifier = reaction.GetMessageIdentifier();

                    if (roleMap.ContainsKey(messageIdentifier))
                    {
                        roleMap[messageIdentifier][reaction.EmoteReference] = reaction.RoleId;
                    }
                    else
                    {
                        roleMap[messageIdentifier] =
                            new EmoteRoleMap(){
                                {
                                    reaction.EmoteReference, reaction.RoleId
                                }
                            };
                    }
                }
            }

            this.AddOrUpdateItem(RoleMapKey, roleMap);
            return roleMap;
        }

        public async Task RemoveRoleAssignmentAsync(string messageId, string emote, string roleToGrant)
        {
            using (var ctx = new OmgSpidersDbContext())
            {

                var existingEntity = await ctx.RoleAssignmentReaction.FirstOrDefaultAsync(x => x.MessageId == messageId && x.EmoteReference == emote && x.RoleId == roleToGrant);
                if (existingEntity == null)
                {
                    return;
                }

                ctx.RoleAssignmentReaction.Remove(existingEntity);

                await ctx.SaveChangesAsync();
                this.InvalidateItem<RoleAssignmentMap>(RoleMapKey);
            }
        }
    }
}

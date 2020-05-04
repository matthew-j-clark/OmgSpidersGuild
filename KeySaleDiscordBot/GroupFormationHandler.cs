using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using KeySaleDiscordBot.Interfaces;
// ReSharper disable PossibleMultipleEnumeration

namespace KeySaleDiscordBot
{
   

    public class GroupFormationHandler:  IGroupFormationHandler
    {
        public RestUserMessage MessageToTrack { get; set; }
        public GroupFormationStatus Status { get; set; } = GroupFormationStatus.SalePosted;
        public GroupMembers GroupMembers { get; set; } = new GroupMembers();
        public Guid RunId { get; private set; }
        public ulong AdvertiserId { get; private set; }
        public string Details { get; set; }

        public GroupFormationHandler(ulong advertiserId)
        {
            this.AdvertiserId = advertiserId;
            this.RunId = Guid.NewGuid();
        }
        public async Task ReactionRemoved(
            Cacheable<IUserMessage, ulong> message, 
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (this.GroupMembers.RemoveReaction(reaction))
            {
                this.Status = GroupFormationStatus.GroupFormed;
            }
        }

        public async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, 
            ISocketMessageChannel channel, 
            SocketReaction reaction)
        {
            if (reaction.Emote.Name == BotEmotes.Cancel)
            {
                this.Status = GroupFormationStatus.Canceled;
                return;
            }

            if (this.GroupMembers.AddReaction(reaction))
            {
                this.Status = GroupFormationStatus.GroupFormed;
            }
        }
    }

    public class GroupMembers
    {
        private static object lockRoot = new object();
        private SortedUserList PrioritizedUsers { get; set; } = new SortedUserList();
        private ulong[] CurrentGroup => new[] {this.Tank, this.Healer, this.Dps2, this.Dps1, this.KeyHolder};
        
        private SortedUserList TankReactions => this.PrioritizedUsers.GetByReaction(BotEmotes.Tank);
        private SortedUserList DpsReactions=> this.PrioritizedUsers.GetByReaction(BotEmotes.Dps);
        private SortedUserList HealerReactions=> this.PrioritizedUsers.GetByReaction(BotEmotes.Healer);

        private SortedUserList KeyReactions=> this.PrioritizedUsers.GetByReaction(BotEmotes.Key);
        public  ulong Tank { get; set; }
        public  ulong Healer{get; set;}
        public  ulong Dps1{get; set; }
        public ulong Dps2 { get; set; }
        public ulong KeyHolder { get; set; }

        public bool AddReaction(SocketReaction reaction)
        {
            // see if we have the guy
            if (!this.PrioritizedUsers.TryGetSpecificUser(reaction.UserId, out var user))
            {
                user=this.PrioritizedUsers.Add(new UserAndReactions(reaction));
            }
            else
            {
                user.AddReaction(reaction.Emote.Name);
            }

            this.ResetGroup();
            return this.ProcessGroup();
        }

        public bool RemoveReaction(SocketReaction reaction)
        {
            if (!this.PrioritizedUsers.TryGetSpecificUser(reaction.UserId, out var user))
            {
               // noop
               return this.IsGroupValid;
            }

            var reactionName = reaction.Emote.Name;
            user.RemoveReaction(reactionName);
            this.ResetGroup();
            return this.ProcessGroup();
        }

        private void ResetGroup()
        {
            this.Tank = 0;
            this.Healer = 0;
            this.Dps1 = 0;
            this.Dps2 = 0;
            this.KeyHolder = 0;
        }

        private bool ProcessGroup()
        {
            if (!this.IsGroupValid)
            {
                lock (lockRoot)
                {
                    if (!this.IsGroupValid)
                    {
                        this.BuildGroup(this.KeyReactions.GetAllUsersSorted(),
                            this.TankReactions.GetAllUsersSorted(),
                            this.DpsReactions.GetAllUsersSorted(),
                            this.HealerReactions.GetAllUsersSorted());
                    }
                }
            }

            return this.IsGroupValid;
        }

        private void BuildGroup(
            IEnumerable<UserAndReactions> keyUsers, 
            IEnumerable<UserAndReactions> tankUsers, 
            IEnumerable<UserAndReactions> dpsUsers, 
            IEnumerable<UserAndReactions> healerUsers)
        {
            if (this.IsGroupValid)
            {
                return;
            }

            var topKey = keyUsers.FirstOrDefault();
            var topTank = tankUsers.FirstOrDefault();
            var topDps = dpsUsers.FirstOrDefault();
            var secondDps = dpsUsers.Skip(1).FirstOrDefault();
            var topHealer = healerUsers.FirstOrDefault();

            if (topKey == null
                  && topTank== null
                  && topDps == null
                  && topHealer == null
                  && secondDps==null)
            {
                return;
            }
            
            this.KeyHolder = topKey?.UserId ?? this.KeyHolder;
            this.Tank = topTank?.UserId ?? this.Tank;
            this.Dps1 = topDps?.UserId ?? this.Dps1;
            this.Dps2 = secondDps?.UserId ?? this.Dps2;
            this.Healer = topHealer?.UserId ?? this.Healer;
            if (this.IsGroupValid)
            {
                return;
            }

            if (healerUsers.Skip(1).Any())
            {
                this.BuildGroup(keyUsers, tankUsers, dpsUsers, healerUsers.Skip(1));
            }

            if (this.IsGroupValid)
            {
                return;
            }
            
            if (dpsUsers.Skip(1).Any())
            {
                this.BuildGroup(keyUsers, tankUsers, dpsUsers.Skip(1), healerUsers);
            }

            if (this.IsGroupValid)
            {
                return;
            }

            if (tankUsers.Skip(1).Any())
            {
                this.BuildGroup(keyUsers, tankUsers.Skip(1), dpsUsers, healerUsers);
            }

            if (this.IsGroupValid)
            {
                return;
            }

            if (keyUsers.Skip(1).Any())
            {
                this.BuildGroup(keyUsers.Skip(1), tankUsers, dpsUsers, healerUsers);
            }
        }


        public bool IsGroupComplete => this.Tank!=0 && this.Healer!=0&&this.Dps1!=0&&this.Dps2!=0&&this.KeyHolder!=0;

        public bool IsGroupValid => this.IsGroupComplete&&this.CurrentGroup.Where(x=>x!=0).Distinct().Count() == 4;
    }

    public class SortedUserList
    {
        
        private ConcurrentDictionary<ulong, UserAndReactions> userList= new ConcurrentDictionary<ulong, UserAndReactions>();
        
        public UserAndReactions Add(UserAndReactions user)
        {
            this.userList.TryAdd(user.UserId, user);
            return user;
        }

        public bool TryGetSpecificUser(ulong userId, out UserAndReactions user)
            => this.userList.TryGetValue(userId, out user);
       
        public IEnumerable<UserAndReactions> GetAllUsersSorted()
        {
            return this.userList.Values.OrderBy(x=>x.FirstReactionTime);
        }

        public void Remove(ulong userId)
        {
            this.userList.TryRemove(userId, out var unused);
        }

        public SortedUserList GetByReaction(string reaction)
        {
            var users = this.userList.Values.Where(x => x.ReactionList.ContainsKey(reaction)).OrderBy(x=>x.FirstReactionTime);
            var finalList = new SortedUserList();
            foreach (var userAndReactions in users)
            {
                finalList.Add(userAndReactions);
            }
            return finalList;
        }
    }

    public class UserAndReactions
    {

        public UserAndReactions(SocketReaction reaction)
        {
            this.UserId= reaction.UserId;
            this.ReactionList=new ConcurrentDictionary<string,int>();
            this.AddReaction(reaction.Emote.Name);
        }

        public ulong UserId { get; set; }

        public ConcurrentDictionary<string,int> ReactionList { get; set; }

        public long FirstReactionTime = DateTime.UtcNow.ToFileTimeUtc();

        public void AddReaction(string emoteName)
        {
            this.ReactionList.TryAdd(emoteName, 0);
        }
        public void RemoveReaction(string emoteName)
        {
            this.ReactionList.TryRemove(emoteName, out var unused);
        }
    }
}
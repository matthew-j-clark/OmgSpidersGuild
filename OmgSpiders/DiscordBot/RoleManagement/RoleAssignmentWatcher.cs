using Discord;
using Discord.WebSocket;

using SharedModels;

using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.RoleManagement
{
    public class RoleAssignmentWatcher : IBotPassiveWatcher
    {
        public bool IsInitialized { get; set; }
        public DiscordSocketClient Client { get; private set; }
        public RoleAssignmentManager RoleHandler { get; private set; }
        public RoleAssignmentMap RoleMap { get; private set; }
        public List<string> MessageList { get; private set; }

        public async Task Initialize(DiscordSocketClient client)
        {
            this.Client = client;
            this.RoleHandler = new RoleAssignmentManagerFactory().GetRoleAssignmentManager();
            var setupAttemptCount = 0;
            while (this.RoleMap == null)
            {
                try
                {
                    this.RoleMap = this.RoleHandler.GetRoleAssignmentMap();
                }
                catch
                {
                    ++setupAttemptCount;
                    if (setupAttemptCount > 3)
                    {
                        throw;
                    }
                }
            }
            this.IsInitialized = true;
            this.RoleHandler.CacheInvalidation += CacheInvalidation;
            await Task.CompletedTask;
        }

        private void CacheInvalidation(object sender, CacheInvalidationEventArgs e)
        {
            this.RoleMap = this.RoleHandler.GetRoleAssignmentMap();
        }

        public async Task Shutdown()
        {
            await Task.CompletedTask;
        }

        public async Task Startup()
        {
            this.Client.ReactionAdded += ReactionAdded;
            this.Client.ReactionRemoved += ReactionRemoved;
            await Task.CompletedTask;
        }

        private async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            var messageIdentifier = this.GetMessageIdentifier(message, channel);
            if (this.RoleMap.TryGetValue(messageIdentifier, out var emoteRoleMap)
                && emoteRoleMap.TryGetValue(reaction.Emote.Name, out var roleToGrant))
            {
                var role = (channel as SocketGuildChannel).
                    Guild.Roles.FirstOrDefault(x => x.Id == ulong.Parse(roleToGrant));
                await ((SocketGuildUser)reaction.User.Value).RemoveRoleAsync(role);
            }
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            var messageIdentifier = this.GetMessageIdentifier(message, channel);
            if (this.RoleMap.TryGetValue(messageIdentifier, out var emoteRoleMap)
                && emoteRoleMap.TryGetValue(reaction.Emote.Name, out var roleToGrant))
            {
                var role = (channel as SocketGuildChannel).
                    Guild.Roles.FirstOrDefault(x => x.Id == ulong.Parse(roleToGrant));

                await ((SocketGuildUser)reaction.User.Value).AddRoleAsync(role);

            }
        }

        private MessageIdentifier GetMessageIdentifier(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel)
        {
            return new MessageIdentifier
            {
                MessageId = message.Id.ToString(),
                ChannelId = channel.Id.ToString(),
                GuildId = (channel as SocketGuildChannel).Guild.Id.ToString()
            };
        }


    }
}

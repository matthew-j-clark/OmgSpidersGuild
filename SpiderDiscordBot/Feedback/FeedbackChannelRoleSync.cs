﻿using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Feedback
{
    public class FeedbackChannelRoleSync : IBotPassiveWatcher
    {

        private DiscordSocketClient Client;
        public bool IsInitialized { get; set; }
        private SocketGuild Guild { get; set; }
        private IEnumerable<SocketGuildUser> Raiders { get; set; }
        private SocketCategoryChannel FeedbackCategory { get; set; }

        private SocketCategoryChannel ArchiveCategory;
        private IDictionary<string, ITextChannel> RaiderChannels;
        private IDictionary<string, ITextChannel> ArchiveChannels;
        private static object SyncRoot = new object();
        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
        public async Task Initialize(DiscordSocketClient client)
        {
            this.Client = client;

            this.Client.GuildMemberUpdated += UserUpdated;
            
            this.Guild = this.Client.Guilds.First(x => x.Id == OmgSpidersBotDriver.OmgSpidersGuildId);
            await this.Guild.DownloadUsersAsync();
            this.Raiders = this.Guild.Users.Where(x => x.Roles.Any(x => x.Id == FeedbackCommon.TrialRoleId || x.Id==FeedbackCommon.MainRaiderRoleId));
            
            this.FeedbackCategory = this.Guild.GetFeedbackCategory();
            this.ArchiveCategory = this.Guild.GetFeedbackArchiveCategory();
            this.RaiderChannels = FeedbackCommon.GetExistingChannelsAndTopics(FeedbackCategory);
            this.ArchiveChannels = FeedbackCommon.GetExistingChannelsAndTopics(ArchiveCategory);

            await SetupPerRaiderChannels();
            await ArchiveChannelsForNonRaiders();

            await SortChannelsByName();

        }

        private async Task SortChannelsByName()
        {
            lock(SyncRoot)
            { 
                var orderedList = this.RaiderChannels.OrderBy(x => x.Value.Name).ToList();
                var startingPosition = this.RaiderChannels.Min(x => x.Value.Position);
                for (int i = 0; i < orderedList.Count; ++i)
                {

                    orderedList[i].Value.ModifyAsync(x => x.Position = startingPosition + i - 1).Wait();
                }
            }

            await Task.CompletedTask;
        }

        private async Task ArchiveChannelsForNonRaiders()
        {

           var channelsToRemove =  this.RaiderChannels.Where(x =>
            {
                try
                {
                    var user = this.Guild.GetUser(x.Value.GetFeedbackUserId());
                    if(this.IsUserNoLongerRaider(user))
                    {
                        return true;
                    }
                }
                catch
                {
                    //remove
                    return true;
                }
                return false;
            });

            foreach(var channel in channelsToRemove)
            {
                await ArchiveChannelAndUpdateTracking(channel.Key, channel.Value);
                
            }
            
        }

        private async Task SetupPerRaiderChannels()
        {
            var newRaiders = this.FindRaidersWithMissingChannels();
            await this.CreateNewRaiderChannels(newRaiders);

        }

        private async Task CreateNewRaiderChannels(IEnumerable<SocketGuildUser> newRaiders)
        {
            foreach (var raider in newRaiders)
            {
                await CreateOrReactivateRaiderChannel(raider);
            }
        }

        private async Task CreateNewRaiderChannel(SocketGuildUser raider)
        {
            var topic = this.GetUserIdString(raider);
            var channel = await this.Guild.CreateTextChannelAsync(raider.Nickname ?? raider.Username, props =>
              {
                  props.CategoryId = FeedbackCommon.FeedbackCategoryId;
                  props.Topic = topic;

              });

            await AddPermissionsOverride(raider, channel);

            this.RaiderChannels.Add(topic, channel);
        }

        private async Task AddPermissionsOverride(SocketGuildUser raider, ITextChannel channel)
        {
            var permissions = new OverwritePermissions(
                              addReactions: PermValue.Allow,
                              readMessageHistory: PermValue.Allow,
                              sendMessages: PermValue.Allow,
                              viewChannel: PermValue.Allow);
            await channel.AddPermissionOverwriteAsync(raider, permissions);
        }

        private IEnumerable<SocketGuildUser> FindRaidersWithMissingChannels()
        {
            var raidersMissingChannels = new List<SocketGuildUser>();
            foreach (var raider in this.Raiders)
            {
                if (!RaiderChannelExists(raider))
                {
                    raidersMissingChannels.Add(raider);
                }
            }

            return raidersMissingChannels;
        }

        private bool RaiderChannelExists(SocketGuildUser raider)
        {
            return this.RaiderChannels.ContainsKey(raider.Id.ToString());
        }


        private async Task UserUpdated(SocketUser oldUser, SocketUser updatedUser)
        {
            var updatedGuildUser = updatedUser as SocketGuildUser;
            var oldGuildUser = oldUser as SocketGuildUser;
            if (this.IsUserNewRaider(updatedGuildUser, oldGuildUser))
            {
                await this.CreateOrReactivateRaiderChannel(updatedGuildUser);
            }
            else if (this.IsUserNoLongerRaider(updatedGuildUser))
            {
                await this.ArchiveRaiderChannel(updatedGuildUser);
            }
            await SortChannelsByName();
        }

        private async Task CreateOrReactivateRaiderChannel(SocketGuildUser raiderUser)
        {
            string userId = GetUserIdString(raiderUser);

            if (ArchiveChannels.ContainsKey(userId))
            {
                await this.ReactivateRaiderChannel(raiderUser, this.ArchiveChannels[userId]);
            }
            else
            {
                await this.CreateNewRaiderChannel(raiderUser);
            }
            await SortChannelsByName();
        }

        private async Task ReactivateRaiderChannel(SocketGuildUser raiderUser, ITextChannel raiderChannel)
        {
            var raiderUserId = this.GetUserIdString(raiderUser);
            await this.AddPermissionsOverride(raiderUser, raiderChannel);
            await MoveChannelToFeedbackCategory(raiderChannel);
            this.RaiderChannels.Add(raiderUserId, raiderChannel);
            this.ArchiveChannels.Remove(raiderUserId);
        }

        private static async Task MoveChannelToFeedbackCategory(ITextChannel raiderChannel)
        {
            await MoveChannel(raiderChannel, FeedbackCommon.FeedbackCategoryId);
        }

        private static async Task MoveChannel(ITextChannel raiderChannel, ulong categoryId)
        {
            await raiderChannel.ModifyAsync(props =>
            {
                props.CategoryId = new Optional<ulong?>(categoryId);
            });
        }

        private string GetUserIdString(SocketGuildUser raiderUser)
        {
            return raiderUser.Id.ToString();
        }

        private async Task ArchiveRaiderChannel(SocketGuildUser noLongerRaiderUser)
        {
            var userId = GetUserIdString(noLongerRaiderUser);
            if (this.RaiderChannels.ContainsKey(userId))
            {
                var channel = this.RaiderChannels[userId];
                await channel.RemovePermissionOverwriteAsync(noLongerRaiderUser);
                await ArchiveChannelAndUpdateTracking(userId, channel);
            }
        }

        private async Task ArchiveChannelAndUpdateTracking(string userId, ITextChannel channel)
        {
            await MoveChannelToArchiveCategory(channel);
            this.RaiderChannels.Remove(userId);
            this.ArchiveChannels.Add(userId, channel);
        }

        private static async Task MoveChannelToArchiveCategory(ITextChannel raiderChannel)
        {
            await MoveChannel(raiderChannel, FeedbackCommon.FeedbackArchiveCategoryId);
        }

        private bool IsUserNoLongerRaider(SocketGuildUser updatedGuildUser)
        {
            return !this.UserIsRaider(updatedGuildUser);
        }
        private bool IsUserNewRaider(SocketGuildUser updatedGuildUser, SocketGuildUser oldGuildUser)
        {
            return this.UserIsRaider(updatedGuildUser) && !this.UserIsRaider(oldGuildUser);
        }

        private bool UserIsRaider(SocketGuildUser user)
        {
            return user.Roles.Any(x => x.Id == FeedbackCommon.TrialRoleId || x.Id == FeedbackCommon.MainRaiderRoleId) ;
        }
    }
}


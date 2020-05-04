using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using KeySaleDiscordBot.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KeySaleDiscordBot
{
    public class KeysaleBot
    {
        private IBotPendingGroupTracker messageTracker { get; set; }

        public KeysaleBot(IBotPendingGroupTracker messageTracker, IConfiguration configuration)
        {
            this.messageTracker = messageTracker;
            this.AppConfiguration = configuration;
        }

        public IConfiguration AppConfiguration { get; set; }

        public async Task InitializeBot()
        {
            var botConfig = new DiscordSocketConfig { MessageCacheSize = 100 };
            this.Client = new DiscordSocketClient(botConfig);

            this.Client.Log += this.Log;
            this.Client.MessageReceived += this.MessageReceived;
            this.Client.MessageUpdated += this.MessageUpdated;
            this.Client.ReactionAdded += this.ReactionAdded;
            this.Client.ReactionRemoved += this.ReactionRemoved;
            
            await this.Client.LoginAsync(TokenType.Bot,
                this.AppConfiguration["DiscordToken"]);
        }
        private bool ActionFromBot(ulong userId)
        {
            return userId == this.Client.CurrentUser.Id;
        }

        private async Task ReactionRemoved(
            Cacheable<IUserMessage, ulong> message, 
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (this.ActionFromBot(reaction.UserId))
            {
                return;
            }

            if (this.messageTracker.TryGetGroup(message.Id, out var groupState))
            {
                await groupState.ReactionRemoved(message, channel, reaction);
            }
            var fullMessage = await message.DownloadAsync();
            await fullMessage.ModifyAsync(x => x.Content = this.BuildInProgressMessage(groupState));
            
        }

        private async Task ReactionAdded(
            Cacheable<IUserMessage, ulong> message, 
            ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            if (ActionFromBot(reaction.UserId))
            {
                return;
            }
            if (this.messageTracker.TryGetGroup(message.Id, out var groupState))
            {
                await groupState.ReactionAdded(message, channel, reaction);
                var fullMessage = await message.DownloadAsync();
                await fullMessage.ModifyAsync(x => x.Content = this.BuildInProgressMessage(groupState));
            }

            if (groupState.Status == GroupFormationStatus.GroupFormed)
            {
                await channel.SendMessageAsync(
                    $"Group formed for run: {groupState.RunId}\n"
                    + $"Tank: {this.Client.GetUser(groupState.GroupMembers.Tank).Mention}\n"
                    + $"Healer: {this.Client.GetUser(groupState.GroupMembers.Healer).Mention}\n"
                    + $"DPS: {this.Client.GetUser(groupState.GroupMembers.Dps1).Mention}\n"
                    + $"DPS: {this.Client.GetUser(groupState.GroupMembers.Dps2).Mention}\n"
                    + $"Key: {this.Client.GetUser(groupState.GroupMembers.KeyHolder).Mention}\n" +
                    $"Advertiser: {this.Client.GetUser(groupState.AdvertiserId).Mention}");
                this.messageTracker.RemoveGroup(groupState);
            }
            else if (groupState.Status == GroupFormationStatus.Canceled)
            {
                await channel.SendMessageAsync("------------------------------------------------\n" +
                                         $"Run: {groupState.RunId} has been canceled by the advertiser\n" +
                                         $"------------------------------------------------");
                this.messageTracker.RemoveGroup(groupState);
            }
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> messageOld, SocketMessage messageNew,
            ISocketMessageChannel channel)
        {
            if (this.ActionFromBot(messageNew.Author.Id))
            {
                return;
            }
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (this.ActionFromBot(message.Author.Id))
            {
                return;
            }

            // process message
            var channelNames = this.AppConfiguration["KeySalesChannelNames"].ToLowerInvariant().Split(',');
            if (!channelNames.Contains(message.Channel.Name.ToLowerInvariant()))
            {
                // not in our correct channel.
                return;
            }

            if (!message.Content.StartsWith(this.AppConfiguration["CommandPrefix"]))
            {
                return;
            }

            var roles = ((SocketGuildChannel) message.Channel).Guild.Roles;
            var validRoles = this.AppConfiguration["KeySalesPostRole"].ToLowerInvariant().Split(',');
            var validUser=roles.Where(x => validRoles.Contains(x.Name.ToLowerInvariant())).SelectMany(x=>x.Members).Any(x=>x.Username==message.Author.Username);
            //x.Name 
            if (!validUser)
            {
              await  message.Channel.SendMessageAsync(
                    $"{message.Author.Mention} is not permitted to create runs. You need one of the following roles: \"{this.AppConfiguration["KeySalesPostRole"]}\"  to post runs.");
            }
            else
            {
                var groupState = new GroupFormationHandler(message.Author.Id);
                groupState.Details = message.Content.Replace(this.AppConfiguration["CommandPrefix"], string.Empty);

                var responseMessage = await message.Channel.SendMessageAsync(BuildInProgressMessage(groupState));
                groupState.MessageToTrack = responseMessage;

                this.messageTracker.AddGroup(groupState);
                await responseMessage.AddStandardReactions();
            }

            await message.DeleteAsync();
        }

        private string BuildInProgressMessage(IGroupFormationHandler groupState)
        {
           return $"----------------------------------------------------------------\n" +
                  $"{(groupState.Status==GroupFormationStatus.SalePosted?"Current Group For: ": "Group Formed For: ")}{groupState.RunId}\n"
                  +$"----------------------------------------------------------------\n"
                  +$"{groupState.Details}\n"
                  +$"----------------------------------------------------------------\n"
                + $"Tank: {(groupState.GroupMembers.Tank == 0 ? string.Empty : this.Client.GetUser(groupState.GroupMembers.Tank).Username)}\n"
                + $"Healer: {(groupState.GroupMembers.Healer == 0 ? string.Empty : this.Client.GetUser(groupState.GroupMembers.Healer).Username)}\n"
                + $"DPS: {(groupState.GroupMembers.Dps1 == 0 ? string.Empty : this.Client.GetUser(groupState.GroupMembers.Dps1).Username)}\n"
                + $"DPS: {(groupState.GroupMembers.Dps2==0?string.Empty:this.Client.GetUser(groupState.GroupMembers.Dps2).Username)}\n"
                + $"Key: {(groupState.GroupMembers.KeyHolder == 0 ? string.Empty : this.Client.GetUser(groupState.GroupMembers.KeyHolder).Username)}\n" +
                  $"Advertiser: {(groupState.AdvertiserId == 0 ? string.Empty : this.Client.GetUser(groupState.AdvertiserId).Username)}";
        }

        public async Task RunBot(CancellationToken cancellationToken)
        {
            await this.Client.StartAsync();
            
            // Block this task until the program is closed.
            await Task.Delay(-1, cancellationToken);
        }

        public DiscordSocketClient Client { get; set; }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
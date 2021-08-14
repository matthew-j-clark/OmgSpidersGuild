using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Feedback
{
    public class FeedbackModeration : IBotPassiveWatcher
    {
        
        private DiscordSocketClient Client;
        private SocketGuild Guild;
        private SocketTextChannel TeamFeedbackChannel;
        private const string ThumbsUp = "👍";
        private const string ThumbsDown = "👎";
        private const string FeedbackAccepted = "🇦";
        private const string FeedbackDeclined = "🇩";

        public bool IsInitialized { get; set; }

        public async Task Initialize(DiscordSocketClient client)
        {
            this.Client = client;
            this.Guild = this.Client.Guilds.First(x => x.Id == OmgSpidersBotDriver.OmgSpidersGuildId);
            this.TeamFeedbackChannel = this.Guild.GetTextChannel(FeedbackCommon.TeamFeedbackChannelId);
            this.Client.MessageReceived += MessageReceived;
            this.Client.ReactionAdded += ReactionAdded;
            await Task.CompletedTask;
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot)
            {
                return;
            }

            if (channel.Id != FeedbackCommon.TeamFeedbackChannelId)
            {
                return;
            }

            var message = await messageCacheable.GetOrDownloadAsync();
            if (BotRemovedModeration(message))
            {
                return;
            }

            await this.ProcessModerationReaction(message, reaction);
        }

        private static bool BotRemovedModeration(IUserMessage message)
        {
            return !message.Reactions.Any(x => x.Value.IsMe);
        }

        private async Task ProcessModerationReaction(IUserMessage message, SocketReaction reaction)
        {
            if(reaction.Emote.Name==ThumbsUp && MessageHasThreeTotalThumbsUp(message))
            {
                await this.SendMessageToTargetFeedbackChannel(message);
                await this.RemoveModeration(message);
                await message.AddReactionAsync(new Emoji(FeedbackAccepted));
            }
            else if (reaction.Emote.Name == ThumbsDown)
            {
                await this.RemoveModeration(message);
                await message.AddReactionAsync(new Emoji(FeedbackDeclined));
            }
        }

        private bool MessageHasThreeTotalThumbsUp(IUserMessage message)
        {
            return message.Reactions.Where(x => x.Key.Name == ThumbsUp).Count() > 3;
        }

        private async Task RemoveModeration(IUserMessage message)
        {            
            var targetUser = MentionUtils.MentionUser(message.MentionedUserIds.First());
            await message.RemoveAllReactionsAsync();            
        }

        private async Task SendMessageToTargetFeedbackChannel(IUserMessage message)
        {
            var mentionedUserId = message.MentionedUserIds.First();
            var channels = FeedbackCommon.GetExistingChannelsAndTopics(FeedbackCommon.GetFeedbackCategory(this.Guild));
            var targetChannel = FeedbackCommon.GetFeedbackChannelForUser(mentionedUserId, channels);
            await targetChannel?.SendMessageAsync($"{targetChannel.GetUserMentionForChannel()} you have received new feedback!" +
                $"\n{GetFeedbackString(message)}");
        }      

        private string GetFeedbackString(IUserMessage message)
        {
            return string.Join("\n",message.Content.Split('\n').Skip(1));
        }

        private async Task MessageReceived(SocketMessage feedbackMessage)
        {
            if (feedbackMessage.Channel.Id != FeedbackCommon.TeamFeedbackChannelId || feedbackMessage.Author.IsBot)
            {
                return;
            }

            await this.SetupMessageForModeration(feedbackMessage);
        }

        private async Task SetupMessageForModeration(SocketMessage feedbackMessage)
        {
            if (this.IsFeedbackFormatOk(feedbackMessage))
            {
                await this.AddReactionsForModeration(feedbackMessage);
            }
            else
            {
                await feedbackMessage.Author.SendMessageAsync("Format of your feedback message is invalid. Message must contain exactly one @ mention of a current raider. \n" +
                    $"{feedbackMessage.Content}");
                // await feedbackMessage.DeleteAsync();

            }
        }

        private async Task AddReactionsForModeration(SocketMessage feedbackMessage)
        {
            await feedbackMessage.AddReactionAsync(new Emoji(ThumbsUp));
            await feedbackMessage.AddReactionAsync(new Emoji(ThumbsDown));
            
        }

        private bool IsFeedbackFormatOk(SocketMessage feedbackMessage)
        {
            if(feedbackMessage.MentionedUsers.Count!=1)
            {
                return false;
            }    

           // if(feedbackMessage.Content.Split('\n').Length<=1)
           // {
           //     return false;
          //  }
            return true;
        }

        public async Task Shutdown()
        {
            await Task.CompletedTask;
        }
    }
}

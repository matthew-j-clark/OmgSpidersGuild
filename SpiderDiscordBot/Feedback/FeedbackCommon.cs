using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Feedback
{
    public static class FeedbackCommon
    {
        public const ulong FeedbackCategoryId = 847886022071287888;
        public const ulong FeedbackArchiveCategoryId = 847901892314136626;
        public const ulong TeamFeedbackChannelId = 847934287935569980;
        public const ulong TrialRoleId = 689925919728599061;
        public const ulong MainRaiderRoleId = 689925874488836237;
        public static IDictionary<string, ITextChannel> GetExistingChannelsAndTopics(SocketCategoryChannel category)
        {
            var channelsAsSocketText = category.Channels.Select(x => x as ITextChannel).Where(x => x.Id != TeamFeedbackChannelId);
            var topicsWithUserId = channelsAsSocketText.ToDictionary(x => x.Topic, x => x);
            return topicsWithUserId;
        }
        public static ITextChannel GetFeedbackChannelForUser(ulong mentionedUserId, IDictionary<string, ITextChannel> channels)
        {
            if (channels.ContainsKey(mentionedUserId.ToString()))
            {
                return channels[mentionedUserId.ToString()];
            }

            return null;
        }

        public static SocketCategoryChannel GetFeedbackCategory(this SocketGuild guild)
        {
            return guild.GetCategoryChannel(FeedbackCategoryId);
        }
        public static SocketCategoryChannel GetFeedbackArchiveCategory(this SocketGuild guild)
        {
            return guild.GetCategoryChannel(FeedbackCategoryId);
        }

        public static string GetUserMentionForChannel(this ITextChannel channel)
        {
            return MentionUtils.MentionUser(ulong.Parse(channel.Topic));
        }

        public static ulong GetFeedbackUserId (this ITextChannel channel)
        {
            return ulong.Parse(channel.Topic);
        }
    }

}

using Discord;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderDiscordBot.Feedback
{
    public static class FeedbackCommon
    {
        public const ulong FeedbackCategoryId = 847886022071287888;
        public const ulong FeedbackArchiveCategoryId = 847901892314136626;
        public static IDictionary<string, ITextChannel> GetExistingChannelsAndTopics(SocketCategoryChannel category)
        {
            var channelsAsSocketText = category.Channels.Select(x => x as ITextChannel);
            var topicsWithUserId = channelsAsSocketText.ToDictionary(x => x.Topic, x => x);
            return topicsWithUserId;
        }

        public static SocketCategoryChannel GetFeedbackCategory(this SocketGuild guild)
        {
            return guild.GetCategoryChannel(FeedbackCategoryId);
        }
        public static SocketCategoryChannel GetFeedbackArchiveCategory(this SocketGuild guild)
        {
            return guild.GetCategoryChannel(FeedbackCategoryId);
        }
    }

}

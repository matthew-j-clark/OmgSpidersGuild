﻿using Discord;
using Discord.Commands;

using SpiderDiscordBot.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Feedback
{
    [AuthorizedGroup("Banana Spider")]
    public class FeedbackReminder : AuthorizedCommand
    {
        public const string SelfFeedbackRemindAllDescription = "!selffeedbackremindall - send all raiders a reminder to provide self evaluation for the week.";

        [Command(ignoreExtraArgs: true, text: "selffeedbackremindall")]
        [Summary(SelfFeedbackRemindAllDescription)]
        public async Task RemindSelfFeedbackAll()
        {
            var channels = this.Context.Guild.GetFeedbackCategory().Channels.Select(x=>x as ITextChannel);
            foreach(var channel in channels)
            {
                var userToPing = channel.Topic;
                var message = $"{MentionUtils.MentionUser(ulong.Parse(channel.Topic))}: With raid complete for the week please take this opportunity to review logs and evaluate your performance. Some things to look for while reviewing. \n" +
                    $"- Things you did well and want to continue doing\n" +
                    $"- Causes of early deaths, and how they can be prevented.\n" +
                    $"- Rotational/CD optimizations.\n" +
                    $"Remember the goal of this exercise is to improve performance over time, and fix issues, not create excuses or lay blame. \n" + 
                    $"Feel free to submit these items via the feedback tool to show your thought process.\n" +
                    $"Post any strategy changes or adjustments that you think would be beneficial for bosses either to the boss channel or directly to an officer."; 

                await channel.SendMessageAsync(message);
            }
        }


        public const string TeamFeedbackDescription = "!teamfeedbackreminder - send all raiders a reminder to provide evaluation for another player for the week.";

        [Command(ignoreExtraArgs: true, text: "teamfeedbackreminder")]
        [Summary(TeamFeedbackDescription)]
        public async Task RemindTeamFeedback()
        {
            var channels = this.Context.Guild.GetFeedbackCategory().Channels.Select(x => x as ITextChannel);
            foreach (var channel in channels)
            {
                var userToPing = channel.Topic;
                var message = $"{MentionUtils.MentionUser(ulong.Parse(channel.Topic))}: Review your team's performance for the week. " +
                    $"Please provide your evaluation for the week in {MentionUtils.MentionChannel(FeedbackCommon.TeamFeedbackChannelId)}. \n" +                    
                    $"Remember that the focus is on improvement and getting better over time, and not on playing a blame game.\n" +
                    $"Format should be:\n" +
                    $"@mention of the player, or mention the {OmgSpidersBotDriver.BananaRoleMention} for team wide or officer feedback.\n" +
                    $"Something they did well.\n" +
                    $"Optionally: an opportunity to improve for next week.";


                await channel.SendMessageAsync(message);
            }
        }

        public const string SelfFeedbackReminderMissingDescription = "!selffeedbackremindermissing - send all raiders who haven't posted in their feedback channels since the last bot message.";

        [Command(ignoreExtraArgs: true, text: "selffeedbackremindermissing")]
        [Summary(SelfFeedbackReminderMissingDescription)]
        public async Task SelfFeedbackReminderMissing()
        {
            var channels = FeedbackCommon.GetExistingChannelsAndTopics(this.Context.Guild.GetFeedbackCategory()).Select(x=>x.Value);            

            foreach (var channel in channels)
            {
                await RemindRaiderIfNoFeedbackSubmitted(channel);
            }
        }

        private static async Task RemindRaiderIfNoFeedbackSubmitted(ITextChannel channel)
        {
            bool messagesFromChannelTarget = await RaiderSentFeedbackAfterLastBotMessage(channel);
            if (!messagesFromChannelTarget)
            {
                await channel.SendMessageAsync($"{MentionUtils.MentionUser(channel.GetFeedbackUserId())} Please provide your weekly feedback.");
            }
        }

        private static async Task<bool> RaiderSentFeedbackAfterLastBotMessage(ITextChannel channel)
        {
            var messages = await channel.GetMessagesAsync(100).FlattenAsync();
            long latestBotMessageTime = GetLatestBotMessageTime(messages);
            return messages.Any(x => x.Author.Id == channel.GetFeedbackUserId() && x.CreatedAt.ToUnixTimeSeconds() >= latestBotMessageTime);
        }

        private static long GetLatestBotMessageTime(IEnumerable<IMessage> messages)
        {
            var botMax = messages.Max(x => x.Author.Id==OmgSpidersBotDriver.SpiderBotUserId?x.CreatedAt.ToUnixTimeSeconds():0);
            return botMax;
        }      
    }
}

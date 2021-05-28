using Discord;
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
        public const string SelfFeedbackDescription = "!selffeedbackreminder  will send all raiders a reminder to provide self evaluation for the week.";

        [Command(ignoreExtraArgs: true, text: "selffeedbackreminder")]
        [Summary(SelfFeedbackDescription)]
        public async Task RemindSelfFeedback()
        {
            var channels = this.Context.Guild.GetFeedbackCategory().Channels.Select(x=>x as ITextChannel);
            foreach(var channel in channels)
            {
                var userToPing = channel.Topic;
                var message = $"{MentionUtils.MentionUser(ulong.Parse(channel.Topic))}: Review your performance for the week. Please provide your self evaluation for the week below. \n" +
                    $"Include two things:\n" +
                    $"Something you did well.\n" +
                    $"An opportunity to improve for next week.\n" +
                    $"Remember that the focus is on improvement and getting better over time, and not on playing a blame game.";

                await channel.SendMessageAsync(message);
            }
        }


        public const string TeamFeedbackDescription = "!teamfeedbackreminder  will send all raiders a reminder to provide evaluation for another player for the week.";

        [Command(ignoreExtraArgs: true, text: "teamfeedbackreminder")]
        [Summary(TeamFeedbackDescription)]
        public async Task RemindTeamFeedback()
        {
            var channels = this.Context.Guild.GetFeedbackCategory().Channels.Select(x => x as ITextChannel);
            foreach (var channel in channels)
            {
                var userToPing = channel.Topic;
                var message = $"{MentionUtils.MentionUser(ulong.Parse(channel.Topic))}: Review your team's performance for the week. Please provide your evaluation for the week below. \n" +                    
                    $"Remember that the focus is on improvement and getting better over time, and not on playing a blame game.\n" +
                    $"Format should be:\n" +
                    $"@mention of the player\n" +
                    $"Something they did well.\n" +
                    $"Optionally: an opportunity to improve for next week.";


                await channel.SendMessageAsync(message);
            }
        }

    }
}

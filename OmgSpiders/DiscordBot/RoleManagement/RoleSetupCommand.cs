using Discord;
using Discord.WebSocket;

using OmgSpiders.DiscordBot.Authorization;

using SpiderSalesDatabase.SaleRunOperations;
using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmgSpiders.DiscordBot.RoleManagement
{
    [AuthorizedGroup("Banana Spider")]
    public class RoleSetupCommand : IBotCommand
    {
        public string StartsWithKey => "!rolesetup";
        public string Description => "!rolesetup messageId emote roleToGrant";

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            var parts = message.Content.Split(" ",StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length!=4)
            {
                await message.Channel.SendMessageAsync("Invalid usage of rolesetup: Incorrect parameter count");
            }

            var messageId = parts[1];
            var emote = parts[2];
            var guildId = (message.Channel as SocketGuildChannel)?.Guild?.Id ?? 0;
            var channelId = message.Channel.Id;
            var roleToGrant = message.MentionedRoles.FirstOrDefault()?.Id;
            if(roleToGrant == null)
            {
                await message.Channel.SendMessageAsync($"Invalid usage of rolesetup: third argument should be a role, instead found {roleToGrant}");
            }

            await new RoleAssignmentManagerFactory().GetRoleAssignmentManager().AddRoleAssignmentAsync(messageId.ToString(), emote, roleToGrant.ToString(), guildId.ToString(),channelId.ToString());

            var reactionMessage = await (message.Channel as SocketGuildChannel)
                .Guild.GetTextChannel(channelId).GetMessageAsync(ulong.Parse(messageId));
            await reactionMessage.AddReactionAsync(new Emoji(emote));

            await message.DeleteAsync();

        }
    }
}

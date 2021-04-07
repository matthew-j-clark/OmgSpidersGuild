using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

using SpiderSalesDatabase.UserManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SaleCommands
{
    public class ClaimPlayerCommand : AuthorizedCommand
    {        
        public const string Description = "Used to claim an alt or character";

        [Command(ignoreExtraArgs: true, text: "claim")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            var stringSpaced = message.Content.Split(' ');
            if (stringSpaced.Length != 2 && stringSpaced.Length != 3)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!claim playerName\"");
                return;
            }
            var discordName = message.Author.Mention;

            if (stringSpaced.Length == 3)
            {
                var authResult = await new AuthorizedGroupAttribute("Banana Spider").CheckPermissionsAsync(this.Context, null, null);
                if (!authResult.IsSuccess)
                {
                    await message.Channel.SendMessageAsync("Only authorized uses can use the format !claim charactername @mention");
                    return;
                }

                discordName = message.MentionedUsers.FirstOrDefault().Mention;

                if (discordName == null)
                {
                    await message.Channel.SendMessageAsync("Invalid format of command, should be \"!claim charactername @mention\" when using assignment.");
                }
            }            

            var playerToClaim = stringSpaced[1];
            var result = await new PlayerManager().ClaimPlayer(playerToClaim, discordName, message.Author.Username);
            await message.Channel.SendMessageAsync(result);
        }
    }
}

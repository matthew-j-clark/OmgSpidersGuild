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
    public class RegisterMainCommand : AuthorizedCommand
    {        
        public const string Description = "Register your main toon for payouts \"!registermain <charactername>\"";

        [Command(ignoreExtraArgs: true, text: "registermain")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            var stringSpaced = message.Content.Split(' ');
            if (stringSpaced.Length != 2 && stringSpaced.Length!=3)
            {
                await message.Channel.SendMessageAsync("Invalid format of command, should be \"!registermain charactername\"");
                return;
            }

            var discordName = message.Author.Mention;

            if (stringSpaced.Length == 3)
            {
                var authResult = await new AuthorizedGroupAttribute("Banana Spider").CheckPermissionsAsync(this.Context, null, null);
                if(!authResult.IsSuccess)
                {
                    await message.Channel.SendMessageAsync("Only authorized uses can use the format !registermain charactername @mention");
                    return;
                }

                discordName = message.MentionedUsers.FirstOrDefault().Mention;

                if(discordName==null)
                {
                    await message.Channel.SendMessageAsync("Invalid format of command, should be \"!registermain charactername @mention\" when using assignment.");
                }
            }

           
            var mainToRegister = stringSpaced[1];
            var result = await new PlayerManager().RegisterMain(discordName, mainToRegister);
            await message.Channel.SendMessageAsync(result);
        }
    }
}

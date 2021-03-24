using Discord.Commands;
using Discord.WebSocket;

using SharedModels;

using SpiderDiscordBot.Authorization;

using SpidersGoogleSheetsIntegration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SignupCommands
{
    public class UpdateSignupCommand : AuthorizedCommand
    {        
        public const string Description = "Same arguments as !signup, used to adjust the signup for your characters.";

        [Command(ignoreExtraArgs: true, text: "updatesignup")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            var arguments = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (arguments.Length != 6)
            {
                await message.Channel.SendMessageAsync("Invalid usage of !updatesignup. Use like this: " + Description);
                return;
            }

            string run, character;
            CharacterClass charClassParsed;
            List<RaidRole> rolesParsed;
            bool canFunnel, isValid;
            (isValid, run, character, charClassParsed, rolesParsed, canFunnel)
                        = await SignupCommand.ValidateAndParseSignupArguments(message, arguments);

            var sheetsClient = new SheetsClient();
            await sheetsClient.Initialize();

            try
            {
                await sheetsClient.UpdateSignupAsync(character, rolesParsed.Contains(RaidRole.Tank), rolesParsed.Contains(RaidRole.Healer), rolesParsed.Contains(RaidRole.Dps), charClassParsed, canFunnel, false, message.Author.Username);
                await message.Channel.SendMessageAsync($"Successfully updated: {character} for {message.Author.Mention} for {run}!");
            }
            catch (Exception ex)
            {
                await message.Channel.SendMessageAsync($"Update attempt resulted in error, @sealslicer please check into it.");
                throw;
            }
        }
    }
}

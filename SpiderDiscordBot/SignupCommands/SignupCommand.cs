using Discord.Commands;
using Discord.WebSocket;

using SharedModels;

using SpiderDiscordBot.Authorization;

using SpidersGoogleSheetsIntegration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace SpiderDiscordBot.SignupCommands
{
    public class SignupCommand : AuthorizedCommand
    {        
        public const string Description = "Used to signup for a run:\n" +
            "!signup runType/Id Character Class RolesCommaSeparated canFunnel optionalDiscordMentionForToonOwner\n" +
            "ex: !signup heroic Thwackdaddy Paladin Tank,Healer Yes \n" +
            "ex: !signup heroic Thwackdaddy Paladin Tank,Healer Yes  @Sealslicer";            

        public static string[] RunTypes => new[] { "Heroic" };

        [Command(ignoreExtraArgs: true, text: "signup")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;
            var arguments = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (arguments.Length != 6 && arguments.Length !=7)
            {
                await message.Channel.SendMessageAsync("Invalid usage of !signup. Use like this: " + Description);
                return;
            }

            string run, character;
            CharacterClass charClassParsed;
            List<RaidRole> rolesParsed;
            bool canFunnel,isValid;
            (isValid, run, character, charClassParsed, rolesParsed, canFunnel) 
                = await ValidateAndParseSignupArguments(message, arguments);

            if(!isValid)
            {
                return;
            }
            var sheetsClient = new SheetsClient();
            await sheetsClient.Initialize();

            try
            {
                var mainUser = message.Author;
                if(message.MentionedUsers.Any())
                {
                    mainUser = message.MentionedUsers.First();                    
                }

                await sheetsClient.AddSignupAsync(character, rolesParsed.Contains(RaidRole.Tank), rolesParsed.Contains(RaidRole.Healer), rolesParsed.Contains(RaidRole.Dps), charClassParsed, canFunnel, false, mainUser.Username);
                await message.Channel.SendMessageAsync($"Successfully signed up: {character} for {mainUser.Mention} for {run}!");
            }
            catch(SheetsSignupException ex)
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention} - {ex.Message}");
            }
            catch (Exception ex)
            {
                await message.Channel.SendMessageAsync($"Signup attempt resulted in error, @sealslicer please check into it.");
                throw;
            }
            
        }

        internal static async 
            Task<(bool isValid,
                string run, 
                string character, 
                CharacterClass characterClass, 
                List<RaidRole> rolesParsed,
                bool canFunnel)> 
            ValidateAndParseSignupArguments(SocketMessage message, string[] arguments)            
        {            
            var run = arguments[1];
            var character = arguments[2];
            var charClass = arguments[3];
            var roles = arguments[4].Split(',');
            if (!RunTypes.Contains(run, StringComparer.OrdinalIgnoreCase))
            {
                await message.Channel.SendMessageAsync("Invalid usage of !signup or !updatesignup. Invalid 'runType/id' value.");
                return (false, string.Empty, string.Empty,CharacterClass.None,null,false);
            }

            if (!Enum.TryParse<CharacterClass>(charClass, true, out var charClassParsed))
            {
                await message.Channel.SendMessageAsync($"Invalid usage of !signup or !updatesignup. Invalid 'Character Class' value. \n" +
                    $"Valid Values are: {string.Join(", ", Enum.GetNames(typeof(CharacterClass)))}");
                return (false, string.Empty, string.Empty, CharacterClass.None, null, false); ;
            }

            var rolesParsed = new List<RaidRole>();
            foreach (var role in roles)
            {
                if (!Enum.TryParse<RaidRole>(role, true, out var roleParsed))
                {
                    await message.Channel.SendMessageAsync($"Invalid usage of !signup or !updatesignup. Invalid 'Role' value. \n" +
                        $"Valid Values are: {string.Join(", ", Enum.GetNames(typeof(RaidRole)))}");
                    return (false, string.Empty, string.Empty, CharacterClass.None, null, false); ;
                }
                rolesParsed.Add(roleParsed);
            }

            if (arguments[5].Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                arguments[5] = "true";
            }
            else if (arguments[5].Equals("no", StringComparison.OrdinalIgnoreCase))
            {
                arguments[5] = "false";
            }

            if (!bool.TryParse(arguments[5], out var canFunnel))
            {
                await message.Channel.SendMessageAsync("Invalid usage of !signup or !updatesignup. Invalid 'canFunnel' value.");
                return (false, string.Empty, string.Empty, CharacterClass.None, null, false); ;
            }

            return (true, run, character, charClassParsed, rolesParsed, canFunnel);
        }
    }
}

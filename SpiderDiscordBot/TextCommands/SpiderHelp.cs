using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using SpiderDiscordBot.Authorization;

namespace SpiderDiscordBot.TextCommands
{
    public class SpiderHelp: AuthorizedCommand
    {
        const int DiscordMaxMessageLength = 2000;
        
        public const string Description = "Prints this help message";

        [Command(ignoreExtraArgs: true, text: "spiderhelp")]
        [Summary(Description)]
        public async Task ProcessMessageAsync()
        {
            var message = this.Context.Message;

            var commandMethods = from t in Assembly.GetExecutingAssembly().GetTypes()
                           where t.BaseType==typeof(AuthorizedCommand)
                           select t.GetMembers();

            var commandMethodsFlattened= commandMethods.SelectMany(x => x);
            var commandList = from t in commandMethodsFlattened
                              where t.CustomAttributes.Any(x => x.AttributeType == typeof(CommandAttribute))
                                    && t.CustomAttributes.Any(x => x.AttributeType == typeof(SummaryAttribute))            
                              select t;

            var commandSummaryDictionary=
                commandList.ToDictionary(x=>x.CustomAttributes.First(y => y.AttributeType == typeof(CommandAttribute)).ConstructorArguments.First().Value as string,
              x => x.CustomAttributes.First(y => y.AttributeType == typeof(SummaryAttribute)).ConstructorArguments.First().Value as string);

            var helpList = commandSummaryDictionary.Select(x => $"{x.Key} : {x.Value}");
            
            var outputBuilder = new StringBuilder();
            foreach(var helpEntry in helpList)
            {
                if(outputBuilder.Length+helpEntry.Length+1>=DiscordMaxMessageLength)
                {
                    await SendHelpStringMessage(message, outputBuilder);
                    outputBuilder.Clear();
                }
                else
                {
                    outputBuilder.AppendLine(helpEntry);
                }
            }
            
            await SendHelpStringMessage(message, outputBuilder);
        }

        private static async Task SendHelpStringMessage(SocketMessage message, StringBuilder outputBuilder)
        {
            var helpString = $"```{outputBuilder.ToString()}```";
            await message.Channel.SendMessageAsync(helpString);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

using OmgSpiders.DiscordBot.Authorization;
using OmgSpiders.DiscordBot.ImageCommands;

namespace OmgSpiders.DiscordBot
{
    public class OmgSpidersBotDriver
    {
        private readonly DiscordSocketClient client;
        private Task botTask;
        internal static Dictionary<string, IBotCommand> CommandList { get; private set; }
        private CancellationToken cancellation;
        
        public OmgSpidersBotDriver()
        {
            this.client = new DiscordSocketClient();

            this.client.Log += LogAsync;
            this.client.Ready += ReadyAsync;
            this.client.MessageReceived += MessageReceivedAsync;
        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{this.client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            //(message.Channel as SocketGuildChannel).Guild.Roles.First(x => x.Name == "Banana Spider");
            // The bot should never respond to itself or another bot
            if (message.Author.Id == this.client.CurrentUser.Id || message.Author.IsBot)
            {
                return;
            }

            var commandKey = message.Content.Split(" ").First();
            if (!commandKey.StartsWith('!'))
            {
                return;
            }
            using (message.Channel.EnterTypingState())
            {
                var bananaRole = (message.Channel as SocketGuildChannel).Guild.Roles.First(x => x.Name.Equals("Banana Spider", StringComparison.OrdinalIgnoreCase));
                RestUserMessage messageToDelete = null;
                try
                {
                    if (CommandList.TryGetValue(commandKey, out var command))
                    {
                        messageToDelete = await message.Channel.SendMessageAsync("Processing Command");
                        Authorize(command, message.Author);                       
                    }
                }
                catch(UnauthorizedCommandUsageException ex)
                {
                    await message.Channel.SendMessageAsync(ex.Message);
                }
                catch (Exception ex)
                {
                    
                    await this.LogAsync(new LogMessage(LogSeverity.Error, commandKey, "exception", ex));
                    await message.Channel.SendMessageAsync($"Error in command {message.Content}. {bananaRole.Mention} please take a look.");
                }
                finally
                {
                    await messageToDelete.DeleteAsync();
                }
            }

        }

        public void Authorize(IBotCommand command, SocketUser user)
        {

            var guildUser = user as SocketGuildUser;
            var authorizedAttribute = command.GetType().CustomAttributes
                .FirstOrDefault(x =>
                x.AttributeType== typeof(AuthorizedGroupAttribute));
            
            if (authorizedAttribute==null)
            {
                return;
            }            
            if(guildUser==null && authorizedAttribute!=null)
            {
                return ;
            }

            var authorizedRoles = authorizedAttribute.ConstructorArguments[0].Value as IEnumerable<CustomAttributeTypedArgument>;
            var authorizedRoleNames = authorizedRoles.Select(arg => (string)arg.Value);
      
            var userRoleNames = guildUser.Roles.Select(x => x.Name);
            var userRoleIds = guildUser.Roles.Select(x => x.Id.ToString());

            var isAuth= userRoleNames.Any(name => authorizedRoleNames.Any(authRole => authRole.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                   || userRoleIds.Any(name => authorizedRoleNames.Any(authRole => authRole.Equals(name, StringComparison.InvariantCultureIgnoreCase)));            

            if(!isAuth)
            {
                throw new UnauthorizedCommandUsageException(command.StartsWithKey,authorizedRoleNames);
            }
        }

        public void StartBot()
        {
            this.cancellation = new CancellationToken();
            this.RegisterCommands();
            this.botTask = Task.Run(this.RunBot, cancellation);
        }

        private void RegisterCommands()
        {
            var commands = 
                from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.GetInterfaces().Contains(typeof(IBotCommand))
                      && t.GetConstructor(Type.EmptyTypes) != null
                select Activator.CreateInstance(t) as IBotCommand;
            commands = commands.Union(GenericImageList.CommandList);
            CommandList = commands.ToDictionary(x => x.StartsWithKey, x=>x, StringComparer.OrdinalIgnoreCase);
            
        }

        public void StopBot()
        {
            
        }

        private async Task RunBot()
        {
            await this.client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("OmgSpidersBotToken"));
            
            await this.client.StartAsync();
            await this.client.SetGameAsync("!spiderhelp to list commands");
            // Block the program until it is closed.
            await Task.Delay(-1, this.cancellation);
        }

    }


}

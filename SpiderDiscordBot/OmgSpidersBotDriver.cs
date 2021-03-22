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

using SpiderDiscordBot.Authorization;
using SpiderDiscordBot.ImageCommands;

namespace SpiderDiscordBot
{
    public class OmgSpidersBotDriver
    {
        private readonly DiscordSocketClient Client;
        private Task botTask;
        internal static Dictionary<string, IBotCommand> CommandList { get; private set; }
        public IEnumerable<IBotPassiveWatcher> Watchers { get; private set; }

        private CancellationToken cancellation;

        public OmgSpidersBotDriver()
        {            
            this.Client = new DiscordSocketClient();
            this.Client.Log += LogAsync;
            this.Client.Ready += ReadyAsync;
            this.Client.MessageReceived += MessageReceivedAsync;                        
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
            Console.WriteLine($"{this.Client.CurrentUser} is connected!");          
            return Task.CompletedTask;
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself or another bot
            await MessageRecievedWithRetry(message,0,2);
        }

        private async Task MessageRecievedWithRetry(SocketMessage message, int attempt, int maxRetries)
        {
            if (message.Author.Id == this.Client.CurrentUser.Id || message.Author.IsBot)
            {
                return;
            }

            var commandKey = message.Content.Split(" ").First();
            if (!commandKey.StartsWith('!'))
            {
                return;
            }

            var bananaRole = (message.Channel as SocketGuildChannel).Guild.Roles.First(x => x.Name.Equals("Banana Spider", StringComparison.OrdinalIgnoreCase));
            RestUserMessage messageToDelete = null;
            IDisposable typingState = null;
            try
            {
                if (CommandList.TryGetValue(commandKey, out var command))
                {
                    typingState = message.Channel.EnterTypingState();
                    messageToDelete = await message.Channel.SendMessageAsync("Processing Command");
                    Authorize(command, message.Author);
                    await command.ProcessMessageAsync(message);
                }
            }
            catch (UnauthorizedCommandUsageException ex)
            {
                await message.Channel.SendMessageAsync(ex.Message);
            }
            catch (Exception ex)
            {
                await this.LogAsync(new LogMessage(LogSeverity.Error, commandKey, "exception", ex));
                if (attempt <= maxRetries)
                {
                    await MessageRecievedWithRetry(message, attempt+1, maxRetries);

                }
                else
                {
                    await message.Channel.SendMessageAsync($"Error in command {message.Content}.\n" +
                        $"Error message: {ex.Message}\n" +
                        $" {bananaRole.Mention} please take a look.");
                }
            }
            finally
            {
                typingState?.Dispose();
                await messageToDelete.DeleteAsync();
            }
        }

        public void Authorize(IBotCommand command, SocketUser user)
        {

            var guildUser = user as SocketGuildUser;
            var authorizedAttribute = command.GetType().CustomAttributes
                .FirstOrDefault(x =>
                x.AttributeType == typeof(AuthorizedGroupAttribute));

            if (authorizedAttribute == null)
            {
                return;
            }
            if (guildUser == null && authorizedAttribute != null)
            {
                return;
            }

            var authorizedRoles = authorizedAttribute.ConstructorArguments[0].Value as IEnumerable<CustomAttributeTypedArgument>;
            var authorizedRoleNames = authorizedRoles.Select(arg => (string)arg.Value);

            var userRoleNames = guildUser.Roles.Select(x => x.Name);
            var userRoleIds = guildUser.Roles.Select(x => x.Id.ToString());

            var isAuth = userRoleNames.Any(name => authorizedRoleNames.Any(authRole => authRole.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                   || userRoleIds.Any(name => authorizedRoleNames.Any(authRole => authRole.Equals(name, StringComparison.InvariantCultureIgnoreCase)));

            if (!isAuth)
            {
                throw new UnauthorizedCommandUsageException(command.StartsWithKey, authorizedRoleNames);
            }
        }

        public void StartBot()
        {
            this.cancellation = new CancellationToken();
            this.RegisterCommands();
            this.RegisterWatchers();
            this.botTask = Task.Run(this.RunBot, cancellation);
        }

        private void RegisterWatchers()
        {
            var watchers =
               from t in Assembly.GetExecutingAssembly().GetTypes()
               where t.GetInterfaces().Contains(typeof(IBotPassiveWatcher))
                     && t.GetConstructor(Type.EmptyTypes) != null
               select Activator.CreateInstance(t) as IBotPassiveWatcher;
            this.Watchers = watchers;
        }

        private void RegisterCommands()
        {            
            var commands =
                from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.GetInterfaces().Contains(typeof(IBotCommand))
                      && t.GetConstructor(Type.EmptyTypes) != null
                select Activator.CreateInstance(t) as IBotCommand;
            commands = commands.Union(GenericImageList.CommandList);
            CommandList = commands.ToDictionary(x => x.StartsWithKey, x => x, StringComparer.OrdinalIgnoreCase);

        }

        public void StopBot()
        {

        }

        private async Task RunBot()
        {
            await this.Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("OmgSpidersBotToken"));

            await this.Client.StartAsync();
            await this.Client.SetGameAsync("!spiderhelp to list commands");

            foreach(var watcher in this.Watchers)
            {
                await watcher.Initialize(this.Client);
                await watcher.Startup();
            }

            // Block the program until it is closed.
            await Task.Delay(-1, this.cancellation);
        }

    }


}

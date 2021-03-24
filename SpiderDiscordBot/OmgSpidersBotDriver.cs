using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
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
        public IEnumerable<IBotPassiveWatcher> Watchers { get; private set; }
        public CommandService CommandService { get; }
        public static string BananaRoleMention { get; private set; }

        private CancellationToken cancellation;

        public OmgSpidersBotDriver()
        {
            this.Client = new DiscordSocketClient();
            this.Client.Log += LogAsync;
            this.Client.Ready += ReadyAsync;
            this.Client.MessageReceived += MessageReceivedAsync;
            this.CommandService = new CommandService();
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

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself or another bot
            await MessageRecievedWithRetry(message, 0, 2);
        }

        private async Task MessageRecievedWithRetry(SocketMessage socketMessage, int attempt, int maxRetries)
        {
            var message = socketMessage as SocketUserMessage;
            if (message == null || message.Author.Id == this.Client.CurrentUser.Id || message.Author.IsBot)
            {
                return;
            }

            var commandKey = message.Content.Split(' ', '\n').First();
            var argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasCharPrefix('/', ref argPos) ||
                message.HasMentionPrefix(this.Client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            if (string.IsNullOrEmpty(OmgSpidersBotDriver.BananaRoleMention))
            {
                OmgSpidersBotDriver.BananaRoleMention =
                    (message.Channel as SocketGuildChannel).Guild.Roles.First(x => x.Name.Equals("Banana Spider", StringComparison.OrdinalIgnoreCase)).Mention;
            }

            RestUserMessage messageToDelete = null;
            IDisposable typingState = null;
            var commandContext = new SocketCommandContext(this.Client, message as SocketUserMessage);

            try
            {
                typingState = message.Channel.EnterTypingState();
                messageToDelete = await message.Channel.SendMessageAsync("Processing Command");

                await this.CommandService.ExecuteAsync(commandContext, argPos, null);
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
                    await MessageRecievedWithRetry(message, attempt + 1, maxRetries);
                }
                else
                {
                    await message.Channel.SendMessageAsync($"Error in command {message.Content}.\n" +
                        $"Error message: {ex.Message}\n" +
                        $" {OmgSpidersBotDriver.BananaRoleMention} please take a look.");
                }
            }
            finally
            {
                typingState?.Dispose();
                await messageToDelete?.DeleteAsync();
            }
        }

        public void StartBot()
        {
            this.cancellation = new CancellationToken();           
            this.RegisterWatchers();
            this.botTask = Task.Run(this.RunBot, cancellation);
            this.CommandService.AddModulesAsync(Assembly.GetExecutingAssembly(), null).Wait();
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

        public void StopBot()
        {

        }

        private async Task RunBot()
        {
            await this.Client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("OmgSpidersBotToken"));

            await this.Client.StartAsync();
            await this.Client.SetGameAsync("!spiderhelp to list commands");

            foreach (var watcher in this.Watchers)
            {
                await watcher.Initialize(this.Client);
                await watcher.Startup();
            }

            // Block the program until it is closed.
            await Task.Delay(-1, this.cancellation);
        }
    }
}

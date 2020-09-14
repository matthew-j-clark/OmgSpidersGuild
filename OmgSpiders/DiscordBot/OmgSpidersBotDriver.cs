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
            using (message.Channel.EnterTypingState())
            {
                if (message.Author.Id == this.client.CurrentUser.Id || message.Author.IsBot)
                {
                    return;
                }

                var commandKey = message.Content.Split(" ").First();
                if (!commandKey.StartsWith('!'))
                {
                    return;
                }
                RestUserMessage messageToDelete = null;
                try
                {
                    if (CommandList.TryGetValue(commandKey, out var command))
                    {
                        messageToDelete = await message.Channel.SendMessageAsync("Processing Command");
                        await command.ProcessMessageAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    var bananaRole = (message.Channel as SocketGuildChannel).Guild.Roles.First(x => x.Name.Equals("Banana Spider", StringComparison.OrdinalIgnoreCase));
                    await this.LogAsync(new LogMessage(LogSeverity.Error, commandKey, "exception", ex));
                    await message.Channel.SendMessageAsync($"Error in command {message.Content}. {bananaRole.Mention} please take a look.");
                }
                finally
                {
                    await messageToDelete.DeleteAsync();
                }
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

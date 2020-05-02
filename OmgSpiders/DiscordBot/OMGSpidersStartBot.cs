using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OmgSpiders.DiscordBot.ImageCommands;

namespace OmgSpiders.DiscordBot
{
    public class OmgSpidersStartBot
    {
        private readonly DiscordSocketClient client;

        private Task botTask;
        private Dictionary<string, IBotCommand> CommandList;
        private CancellationToken cancellation;
        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        public OmgSpidersStartBot()
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
            // The bot should never respond to itself.
            if (message.Author.Id == this.client.CurrentUser.Id)
            {
                return;
            }

            var commandKey = message.Content.Split(" ").First();
            if (!commandKey.StartsWith('!'))
            {
                return;
            }

            if (this.CommandList.TryGetValue(commandKey, out var command))
            {
                await command.ProcessMessageAsync(message);
            }
            else
            {
                await message.Channel.SendMessageAsync($"Invalid Spider Command: {commandKey}");
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
            this.CommandList = new Dictionary<string, IBotCommand>(StringComparer.OrdinalIgnoreCase);
            new NutButtonImage().RegisterToCommandList(this.CommandList);
        }



        public void StopBot()
        {

        }

        private async Task RunBot()
        {
            await this.client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("OmgSpidersBotToken"));

            await this.client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1, this.cancellation);
        }

    }


}

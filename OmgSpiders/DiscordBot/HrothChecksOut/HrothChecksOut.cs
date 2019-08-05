using System.Globalization;
using System.Threading;

namespace OmgSpiders.DiscordBot.HrothChecksOut
{
    using System;
    using System.Threading.Tasks;
    using Discord;
    using Discord.WebSocket;
    public class HrothChecksOut
    {
        private readonly DiscordSocketClient client;

        private Task botTask;

        private CancellationToken cancellation ;
        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        public HrothChecksOut()
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
                return;

            if (message.Content.StartsWith("!checksout", true, CultureInfo.InvariantCulture))
            {
                await message.Channel.SendMessageAsync("https://cdn.discordapp.com/attachments/543312192095911947/607747794404507698/checks_out.gif");
            }
        }

        public void StartBot()
        {
            this.cancellation=new CancellationToken();
           this.botTask=Task.Run(this.RunBot);
        }

        public void StopBot()
        {
            
        }

        private async Task RunBot()
        {
            await this.client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("HrothChecksOutToken"));

            await this.client.StartAsync();

            // Block the program until it is closed.
            await Task.Delay(-1, this.cancellation);
        }

    }

   
}
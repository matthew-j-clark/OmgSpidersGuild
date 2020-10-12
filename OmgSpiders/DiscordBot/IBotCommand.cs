using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace OmgSpiders.DiscordBot
{
    public interface IBotCommand
    {
        string StartsWithKey { get; }
        string Description { get; }
        Task ProcessMessageAsync(SocketMessage message);
    }

    public interface IBotCommandWithInitialize : IBotCommand
    {
        bool IsInitialized { get; set; }
        void Initialize();
    }

    public interface IBotPassiveWatcher
    {
        bool IsInitialized { get; set; }
        Task Initialize(DiscordSocketClient client);
        Task Startup();
        Task Shutdown();
    }

}
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace SpiderDiscordBot
{   
    public interface IBotPassiveWatcher
    {
        bool IsInitialized { get; set; }
        Task Initialize(DiscordSocketClient client);
        Task Startup();
        Task Shutdown();
    }

}
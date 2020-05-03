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
}
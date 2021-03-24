using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpiderDiscordBot.Authorization
{    
    public abstract class AuthorizedCommand: ModuleBase<SocketCommandContext>
    {
        public AuthorizedCommand()
        {
           
        }
    }
}

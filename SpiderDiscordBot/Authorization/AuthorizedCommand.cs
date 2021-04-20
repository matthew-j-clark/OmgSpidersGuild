using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Authorization
{    
    public abstract class AuthorizedCommand: ModuleBase<SocketCommandContext>
    {
        public AuthorizedCommand()
        {
           
        }

        protected async Task<bool> CheckRoleMembershipAsync(params string [] rolesToCheck)
        {
            var result = await new AuthorizedGroupAttribute(rolesToCheck).CheckPermissionsAsync(this.Context, null, null);
            return result.IsSuccess;
        }
    }
}

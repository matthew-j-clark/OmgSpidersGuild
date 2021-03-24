using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Authorization
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class AuthorizedGroupAttribute: PreconditionAttribute
    {
        public AuthorizedGroupAttribute(params string[] groupNamesOrIds)
        {
            this.GroupNames = groupNamesOrIds;
        }

        public string[] GroupNames { get; }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var guildUser = context.User as SocketGuildUser;
            var authorizedRoleNames = this.GroupNames;

            if (authorizedRoleNames == null)
            {
                return await Task.FromResult(PreconditionResult.FromSuccess());
            }

            if (guildUser == null)
            {
                return await Task.FromResult(PreconditionResult.FromError("Not called by a real user"));
            }

            var userRoleNames = guildUser.Roles.Select(x => x.Name);
            var userRoleIds = guildUser.Roles.Select(x => x.Id.ToString());

            var isAuth = userRoleNames.Any(name => authorizedRoleNames.Any(authRole => authRole.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                   || userRoleIds.Any(name => authorizedRoleNames.Any(authRole => authRole.Equals(name, StringComparison.InvariantCultureIgnoreCase)));

            if (!isAuth)
            {
                return await Task.FromResult(PreconditionResult.FromError(new UnauthorizedCommandUsageException(command.Name, authorizedRoleNames)));
            }

            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}

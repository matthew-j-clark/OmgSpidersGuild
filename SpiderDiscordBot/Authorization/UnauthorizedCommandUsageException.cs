using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Authorization
{
    public class UnauthorizedCommandUsageException:Exception
    {
        public UnauthorizedCommandUsageException(AuthorizedCommand command, IEnumerable<string> authorizedRoles):this(GetGroupName(command),authorizedRoles)
        {
            

        }

        public UnauthorizedCommandUsageException(string commandKey, IEnumerable<string> authorizedRoles)
           : base($"Unauthorized usage of command. Valid roles for {commandKey} are: {string.Join(',', authorizedRoles)}.")
        {

        }

        private static string GetGroupName(AuthorizedCommand command)
        {
            var groupAttribute = command.GetType().CustomAttributes
                          .FirstOrDefault(x =>
                          x.AttributeType == typeof(GroupAttribute));
            var groupName = groupAttribute.ConstructorArguments[0].Value as string;
            return groupName;
        }

       
    }
}

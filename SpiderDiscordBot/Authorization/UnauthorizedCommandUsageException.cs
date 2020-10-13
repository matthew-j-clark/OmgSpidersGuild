using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Authorization
{
    public class UnauthorizedCommandUsageException:Exception
    {
        public UnauthorizedCommandUsageException(string commandKey,IEnumerable<string> authorizedRoles)
            : base($"Unauthorized usage of command. Valid roles for {commandKey} are: {string.Join(',',authorizedRoles)}.")
        {

        }
    }
}

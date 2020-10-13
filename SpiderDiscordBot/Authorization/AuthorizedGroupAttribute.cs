using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpiderDiscordBot.Authorization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizedGroupAttribute:Attribute
    {
        public AuthorizedGroupAttribute(params string[] groupNamesOrIds)
        {
            this.GroupNames = groupNamesOrIds;
        }

        public string[] GroupNames { get; }
    }
}

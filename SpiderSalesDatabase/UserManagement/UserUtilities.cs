using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderSalesDatabase.UserManagement
{
    public class UserUtilities
    {
        public static string ConvertUserTargetToDiscordMention(string userTarget, OmgSpidersDbContext ctx)
        {
            if (userTarget.StartsWith("<@!"))
            {
                //noop 
            }
            else
            {
                userTarget = ctx.MainRegistration.FirstOrDefault(x => x.MainName.Equals(userTarget)).DiscordMention;
            }

            return userTarget;
        }
    }
}

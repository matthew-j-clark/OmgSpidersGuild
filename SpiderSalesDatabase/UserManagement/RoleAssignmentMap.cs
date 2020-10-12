using SharedModels;

using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderSalesDatabase.UserManagement
{
    public class RoleAssignmentMap: Dictionary<MessageIdentifier,EmoteRoleMap>
    {
    }

    public class EmoteRoleMap : Dictionary<string, string> { }
}

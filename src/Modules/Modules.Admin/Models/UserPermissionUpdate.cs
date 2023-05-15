using System;
using System.Collections.Generic;
using System.Text;
using InfrastructureCore.Models.Menu;

namespace Modules.Admin.Models
{
    public class UserPermissionUpdate
    {
        public string UserId { get; set; }

        public List<SYUserAccessMenus> PermissionUpdate { get; set; }
        //public List<SYUserAccessMenus> MenuList { get; set; }
    }
}

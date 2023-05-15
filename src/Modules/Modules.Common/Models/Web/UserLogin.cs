using Modules.Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Common.Models.Web
{
    public class UserLogin
    {
        public string UserID { get; set; }
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        public string MenuGroupID { get; set; }
        public int SiteID { get; set; }

        /// <summary>
        /// List of Menus user has access
        /// </summary>
        public List<MenuLogin> lstMenu { get; set; }

        /// <summary>
        /// MenuAccessRights (all)
        /// </summary>
        public List<ToolbarView> lstToolbar { get; set; }
    }
}

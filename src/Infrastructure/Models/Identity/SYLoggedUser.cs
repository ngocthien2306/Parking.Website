using InfrastructureCore.Models.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore.Models.Identity
{
    /// <summary>
    /// Authorized SYUser Model
    /// </summary>
    public class SYLoggedUser
    {
        public string UserID { get; set; }
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string SystemUserType { get; set; }   
        public string Password { get; set; }
        public string MenuGroupID { get; set; }
        public int SiteID { get; set; }
        //add
        public string UserTypeName { get; set; }
        /// <summary>
        /// List of Menus user has access
        /// </summary>
        public List<SYMenu> AuthorizedMenus { get; set; }

        /// <summary>
        /// MenuAccessRights (all)
        /// </summary>
        public List<SYMenuAccess> MenuAccessList { get; set; }
    }
}

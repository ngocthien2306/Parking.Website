using System;

namespace Modules.Common.Models.Web
{
    public class User
    {
        public string UserID { get; set; } // guild id
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        public int SiteID { get; set; }
        public bool IsBlock { get; set; }
        public int IsCount { get; set; }
        public DateTime? LastBlock { get; set; }
    }
}

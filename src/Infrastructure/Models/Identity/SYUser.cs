using Microsoft.AspNetCore.Identity;
using System;

namespace InfrastructureCore.Models.Identity
{
    public class SYUser
    {
        public string UserID { get; set; } // guild id
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get
            {
                return FirstName + " " + LastName;
            }
        }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string Password { get; set; }
        public int SiteID { get; set; }
        public string UseYN { get; set; }
        public bool? IsBlock { get; set; }
        public bool? PoMessage { get; set; }


        /// <summary>
        /// Login failed count
        /// </summary>
        public int? IsCount { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public DateTime? LastBlock { get; set; }
        public DateTime? LastPassChange { get; set; }
        //public DateTime? LastLogin { get; set; }
        public string SystemUserType { get; set; }
        public string PartnerName { get; set; }
        public string GroupName { get; set; }
        

    }
}

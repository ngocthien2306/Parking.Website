using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore.Models.Identity
{
    public class SYUsersInGroup
    {
        public int GROUP_ID { get; set; }
        public string USER_CODE { get; set; }
        public int SITE_ID { get; set; }

        public string USER_ID { get; set; }
        public string GROUP_NAME { get; set; }
    }
}

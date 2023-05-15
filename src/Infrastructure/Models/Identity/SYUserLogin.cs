using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore.Models.Identity
{
    public class SYSYLoggedUser
    {
        /// <summary>
        /// Refer to USER_ID in SYUser Table
        /// </summary>
        public string USER_ID { get; set; }

        public string PROV_NAME { get; set; }

        /// <summary>
        /// Login Name for eachi provider
        /// </summary>
        public string PROV_LOG { get; set; }

        /// <summary>
        /// Provider display name, like FaceBook, Local, Twitter..
        /// </summary>
        public string PROV_DISP_NAME { get; set; }
    }
}

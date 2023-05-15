using Microsoft.AspNetCore.Identity;
using System;

namespace InfrastructureCore.Models.Identity
{
    public class SYAuditLogTracking
    {
        public int No { get; set; }
        public int EVENT_ID { get; set; }
        public DateTime DATE_LOG { get; set; }
        public string SOURCE_IP { get; set; }
        public string HEADER_MAP { get; set; }
        public string PATH { get; set; }
        public string ACTION_TYPE { get; set; }
        public string OLD_DATA { get; set; }
        public string NEW_DATA { get; set; }
        public string USER_INFO { get; set; }
        public string URL { get; set; }
        public string MESSAGE { get; set; }
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public int? SITE_ID { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_AccountMgt
    {
        public int no { get; set; }
        public string userId { get; set; }
        public string userType { get; set; }
        public string userTypeName { get; set; }
        public int countUserMgt { get; set; }
        public DateTime? loginTime { get; set; }
        public DateTime? registDate { get; set; }
        public string memo { get; set; }
        public bool approveReject { get; set; }
        public int storeNo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_UserStore
    {
        public int no { get; set; }
        public string userId { get; set; }
        public string userType { get; set; }
        public string password { get; set; }
        public string userName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public DateTime? birthday{ get; set; }
        public bool gender { get; set; }
        public int storeNo { get; set; }
        public string storeName { get; set; }
        public string userStatus { get; set; }
        public bool approveReject { get; set; }
        public DateTime? registDate { get; set; }
        public string desc { get; set; }
    }
    public class KIO_HistoryUserStore
    {
        public int no { get; set; }
        public string userId { get; set; }
        public int storeNo { get; set; }
        public string userName { get; set; }
        public DateTime birthday { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public DateTime updatedAt { get; set; }


    }
}

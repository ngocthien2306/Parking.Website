using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_StoreMaster
    {
        public int no { get; set; }
        public string locationName { get; set; }
        public int storeNo { get; set; }
        public string location { get; set; }
        public string storeName { get; set; }
        public string  bizNumber { get; set; }
        public string zipCode { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string bizPhoneNumber { get; set; }
        public DateTime? openDate { get; set; }
        public string memo { get; set; }
        public DateTime? registDate { get; set; }
        public DateTime? monitoringStartime { get; set; }
        public DateTime? monitoringEndtime { get; set; }
        public string siteName { get; set; }
        public string siteType { get; set; }
        public int capacity { get; set; }


    }


    public class tblStoreMasterInfo
    {
        public int StoreNo { get; set; }
        public string Location { get; set; }
        public string StoreName { get; set; }
        public string? BizNumber { get; set; }
        public string? ZipCode { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? BizPhoneNumber { get; set; }
        public string? Memo { get; set; }
        public DateTime RegistDate { get; set; }
        public DateTime? MonitoringStartime { get; set; }
        public DateTime? MonitoringEndtime { get; set; }
    }
    public class KIO_UseRegisteredStore : KIO_StoreMaster
    {
        public string userId { get; set; }

    }
}

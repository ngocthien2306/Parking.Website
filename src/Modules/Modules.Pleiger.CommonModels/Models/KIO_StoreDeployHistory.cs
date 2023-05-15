using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_StoreDeployHistory
    {
        public int no { get; set; }
        public int soundDeplyHistNo { get; set; }
        public string storeName { get; set; }
        public string deviceName { get; set; }
        public DateTime deployTime { get; set; }
        public bool deployResult { get; set; }
        public int soundNo { get; set; }
        public string location { get; set; }
    }
}

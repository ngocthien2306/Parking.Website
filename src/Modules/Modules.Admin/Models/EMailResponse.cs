using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class EMailResponse
    {
        public string PoNumber { get; set; }
        public string UserPONumber { get; set; }

        public string Url { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public List<MES_PurchaseDetail> listItem { get; set; }

        
    }
}

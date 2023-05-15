using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_StoreNo
    {
        public int storeNo { get; set; }
    }

    public class tblStoreEnvironmentSettingInfo
    {
        //-------------------------------------------------------------

        private object _id;
        public int EnvironmentSettingNo { get; set; }
        public int StoreNo { get; set; }
        public bool CertifCriteria { get; set; }
        public bool PhoneInput { get; set; }
        public int SimilarityRateApproval { get; set; }
        public bool AuthAfterCompleted { get; set; }
        public bool AuthAfterCardId { get; set; }

        public bool EId { get; set; }
        public bool UseScanner { get; set; }
        public bool UseCamera { get; set; }

    }
}

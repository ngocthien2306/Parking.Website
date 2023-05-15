using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels.Models
{
    public class KIO_StoreEnvSettings
    {
        public int environmentSettingNo { get; set; }
        public int storeNo { get; set; }
        public bool certifCriteria { get; set; }
        public bool phoneInput { get; set; }
        public int similarityRateApproval { get; set; }
        public bool authAfterCompleted { get; set; }
        public bool authAfterCardId { get; set; }
        public bool eId { get; set; }
        public bool useScanner { get; set; }
        public bool useCamera { get; set; }

    }
}

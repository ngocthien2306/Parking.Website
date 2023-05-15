using System.Collections.Generic;

namespace Modules.Admin.Models
{
    public class SYPageElementAction
    {
        public int ACT_PEL_ID { get; set; }
        public string ACT_PEL_NM { get; set; }
        public string TARGET_PEL_ID { get; set; }
        public string SP_NM { get; set; }
        //public string PARAM_SP { get; set; }
        //public string PARAM_FLD { get; set; }
        //public int PARAM_TYP { get; set; }
        public List<SYPageElementActionDetails> lstDetails { get; set; }
        public int MyProperty { get; set; }
    }
}

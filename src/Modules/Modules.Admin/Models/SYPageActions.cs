using System.Collections.Generic;

namespace Modules.Admin.Models
{
    public class SYPageActions
    {
        public int PAG_ID { get; set; }
        public int ACT_ID { get; set; }
        public string ACT_NM { get; set; }
        public string ACT_FN { get; set; }      //SCRIPT NAME (*FUNCTION NAME)
        public string ACT_LC { get; set; }        //ACTION SCRIPT FILE URL
        public string ACT_TYP { get; set; }     //SCR/MAP
        public string ACT_PEL_TYP { get; set; }
        public bool IS_INIT { get; set; }
        public string PAG_KEY { get; set; }
        public string ACT_TYPE { get; set; }
        public List<SYPageActionDetails> listActionDetail { get; set; }
        public List<SYClearElements> listClearPEL { get; set; }
    }
}

using System.Collections.Generic;

namespace Modules.Admin.Models
{
    public class SYDataMap
    {
        public int MAP_ID { get; set; }
        public string MAP_PEL_ID { get; set; }
        public int MAP_PAG_ID { get; set; }
        public string MAP_SPNM { get; set; }
        public string MAP_SPTYPE { get; set; }
        public string MAP_CNNAME { get; set; } // Minh: 2020-06-10: add connection type(Framework, C1, C2)
        public string PEL_TYP { get; set; } // TuanNguyen : 2020-04-22 need check type when render function save action 
        public List<SYDataMapDetails> lstMapDetail { get; set; }
    }
}

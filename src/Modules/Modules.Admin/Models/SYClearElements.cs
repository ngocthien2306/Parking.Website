using System.Collections.Generic;

namespace Modules.Admin.Models
{
    public class SYClearElements
    {
        public int PAG_ID { get; set; }
        public int ACT_ID { get; set; }
        public string PEL_ID { get; set; }
        public string PEL_TYP { get; set; }
        public int PAG_ID_SRC { get; set; }
        public List<SYClearElements> listChild { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class MESWorkHistory
    {
        public int  No { get; set; }
        public int WorkDoneQty { get; set; }
        public DateTime? WorkDoneTime { get; set; }
    }
}

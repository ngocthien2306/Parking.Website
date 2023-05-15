using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class MESIssueItemPart
    {
        public int No { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? ReqQty { get; set; }
        public int StkQty { get; set; }
        public int? RealQty { get; set; }
        public string CategoryName { get; set; }
        public string ItemClassCode { get; set; }
        public string Note { get; set; }
    }
}

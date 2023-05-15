using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Models
{
    public class MES_BOM
    {
        public int Id { get; set; }
        public int ItemLevel { get; set; }
        public string ItemCode { get; set; }
        public string ParentItemCode { get; set; }
        public string NameKor { get; set; }
        public string NameEng { get; set; }
        public int Qty { get; set; }
        public int ParentItemLevel { get; set; }
        public string ItemClass { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public int TotalQty { get; set; }
        
    }
}

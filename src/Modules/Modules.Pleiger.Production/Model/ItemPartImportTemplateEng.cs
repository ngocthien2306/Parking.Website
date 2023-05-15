using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class ItemPartImportTemplateEng
    {
        [ColumName("No")]
        public int No { get; set; }

        [ColumName("Category")]
        public string CategoryName { get; set; }

        [ColumName("Item Class Code")]
        public string ItemClassCode { get; set; }

        [ColumName("Item Code")]
        public string ItemCode { get; set; }
        
        [ColumName("Item Name")]
        public string ItemName { get; set; }

        [ColumName("Real Inventory Qty")]
        public int? RealQty { get; set; }

        [ColumName("Request Qty")]
        public int? ReqQty { get; set; }
      
        [ColumName("Note")]
        public string Note { get; set; }
    }
}

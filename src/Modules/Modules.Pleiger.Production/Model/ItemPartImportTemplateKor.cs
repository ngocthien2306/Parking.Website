using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class ItemPartImportTemplateKor
    {
        [ColumName("No")]
        public int No { get; set; } 
        
        [ColumName("품번")]
        public string ItemCode { get; set; }
        
        [ColumName("품명")]
        public string ItemName { get; set; }
        
        [ColumName("수량")]
        public int? ReqQty { get; set; }
        
        public int StkQty { get; set; }
        
        [ColumName("재고수량")]
        public int? RealQty { get; set; }
        
        [ColumName("품목분류")]
        public string CategoryName { get; set; }
        
        [ColumName("품목구분코드")]
        public string ItemClassCode { get; set; }
        
        [ColumName("메모")]
        public string Note { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class MES_VirtualWarehouse
    {
        public int No { get; set; } // for display No in view
        public int VirtualWareHouseId { get; set; }
        public string VirtualWareHouseName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? ItemQty { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Creater { get; set; }
        public string Status { get; set; }
        public DateTime? CloseDate { get; set; }
        public string FileID { get; set; }
    }
}

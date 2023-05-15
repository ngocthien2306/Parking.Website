using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
   public class MES_WarehouseManagement
    {
        public int No { get; set; } // for display No in view
        public string WarehouseCode { get; set; }
        public string WarehouseType { get; set; }
        public string WarehouseName { get; set; }
        public string InternalExternal { get; set; }
        public string Manager { get; set; }
        public string Status { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }


        //USE FOR PAGE MES_ITEM MANAGEMENT IN WH ITEM GRID
        public string ID { get; set; }
        public string Name { get; set; }
    }
}

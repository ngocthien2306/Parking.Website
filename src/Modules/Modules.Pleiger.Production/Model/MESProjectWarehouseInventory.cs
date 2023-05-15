using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class MESProjectWarehouseInventory
    {
        public int No { get; set; }
        public string WarehouseName { get; set; }
        
        public string UserSalesOrderProjectCode { get; set; }
        public string ProductionProjectName { get; set; }
        
        public string SalesOrderProjectName { get; set; }
        public string ProjectOrderType { get; set; }
        public string PartnerCode { get; set; }
        public string ProductionProject { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ProjectStatus { get; set; }
        public int OrderQuantity { get; set; }
        public int ProdcnDoneQty { get; set; }
        public int DeliveryQty { get; set; }
        public int StockQty { get; set; }
        public int Qty { get; set; }

        public string ProjectStatusName { get; set; }
        public string PartnerName { get; set; }
        public string CategoryName { get; set; }
        public string WHFromCodeName { get; set; }
        public string WHToCodeName { get; set; }
        public string ProjectCode { get; set; }
        public string WarehouseCode { get; set; }



    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class MESProductionResultExportTemplate
    {
        public int No { get; set; }
        public string ProjectCode { get; set; }
        public string UserProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatusName { get; set; }
        public string SalesOrderProjectName { get; set; }
        public DateTime? PlanDeliveryDate { get; set; }
        public string ProductType { get; set; }
        public string ProductTypeName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string DomesticForeign { get; set; }
        public int OrderQuantity { get; set; }
        public string CustomerName { get; set; }
        public string SalesClassification { get; set; }
        public string SalesClassificationName { get; set; }
        public string ProjectOrderType { get; set; }
        public string ProjectOrderTypeName { get; set; }
        public int ProdcnDoneQty { get; set; }
        public int DeliveryTotalQty { get; set; }
        public string ProdcnLineCode { get; set; }
        public string ProductLineName { get; set; }
        public int AssignedQty { get; set; }
        public int ProdDoneQty { get; set; } // Done of production line
        public string ProdcnLineState { get; set; }
        public string ProdcnLineStateName { get; set; }
        public string Manager { get; set; }
        public int WorkDoneQty { get; set; }
        public DateTime? WorkDoneTime { get; set; }
        public string CustWHCode { get; set; }
        public string WarehouseName { get; set; }
        public string CustWH { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int DeliverQty { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Modules.Pleiger.Models
{
    public class MES_InventoryCheckVO
    {
        public int No { get; set; }
        public string WHCode { get; set; }
        public string WHName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int CurrentStockQty { get; set; }
        public int StockQtyUploaded { get; set; }// use for stock Qty when upload file check inventory
        public int CheckQty { get; set; }
        public int DiffQty { get; set; }// save value
        public int DiffQtyDisplay { get; set; }
        public string Remark { get; set; }
        public DateTime? StockDate { get; set; }
        public DateTime? CheckDate { get; set; }
        public string State { get; set; }
    }
    public class MES_InventoryCheckExcelVO
    {
        [ColumName("No")]
        [JsonProperty(Order = 1)]
        public int No { get; set; }

        [ColumName("Warehouse Code")]
        [JsonProperty(Order = 2)]
        public string WHCode { get; set; }

        [ColumName("Warehouse Name")]
        [JsonProperty(Order = 3)]
        public string WHName { get; set; }

        [ColumName("Category Code")]
        [JsonProperty(Order = 4)]
        public string CategoryCode { get; set; }

        [ColumName("Category Name")]
        [JsonProperty(Order = 5)]
        public string CategoryName { get; set; }

        [ColumName("Item Code")]
        [JsonProperty(Order = 6)]
        public string ItemCode { get; set; }

        [ColumName("Item Name")]
        [JsonProperty(Order = 7)]
        public string ItemName { get; set; }

        [ColumName("MES System Quantity")]
        [JsonProperty(Order = 8)]
        public int MESSystemQty { get; set; }

        [ColumName("Offline Check Quantity")]
        [JsonProperty(Order = 9)]
        public int OfflineCheckQty { get; set; }

        //public int DiffQty { get; set; }
        [ColumName("Remark")]
        [JsonProperty(Order = 10)]
        public string Remark { get; set; }
        [ColumName("Inventory Date")]
        [JsonProperty(Order = 11)]
        public DateTime? StockDate { get; set; }
        [ColumName("Check Date")]
        [JsonProperty(Order = 12)]
        public DateTime? CheckDate { get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.All,
                       AllowMultiple = true)]  // Multiuse attribute.  
    public class ColumNameAttribute : System.Attribute
    {
        string name;

        public ColumNameAttribute(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }
    }
}

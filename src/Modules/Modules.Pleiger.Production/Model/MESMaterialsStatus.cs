using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Model
{
    public class MESMaterialsStatus
    {
        public int No { get; set; }
        public int InOutType { get; set; }
        public string InOutTypeNm { get; set; }
        public string UserProjectCode { get; set; }
        public string UserPONumber { get; set; }
        public string CategoryName { get; set; }
        public string ItemCode { get; set; }
        public DateTime? InOutDate { get; set; }
        public string ItemName { get; set; }
        public int RealQuantity { get; set; }
        public int InOutQty { get; set; }
        public int SafetyQuantity { get; set; }
        public string PartnerName { get; set; }
        public decimal CostOfItem { get; set; }
        public decimal TotalCost { get; set; }
        public string InOutCharge { get; set; }
        public string OutTargetLine { get; set; }
        public string WarehouseName { get; set; }
    }
}

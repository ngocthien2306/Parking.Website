using System;

namespace Modules.Pleiger.CommonModels
{
    public class MES_PurchaseDeliveryOrder
    {
        public int NO { get; set; }
        public string ProjectName { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemUnit { get; set; }
        public string PleigerRemark { get; set; }
        public int? TotalPOQty { get; set; }
        public int DeliveryQty { get; set; }
        public string UserPONumber { get; set; }
        public string PONumber { get; set; }
        public DateTime? PlanDeliveryDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DeliveryDateTo { get; set; }
        public DateTime? PartnerDeliveryDate { get; set; } // Dat add 2022-1-26
    }
}

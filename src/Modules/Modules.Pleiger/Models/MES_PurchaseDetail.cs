using System;

namespace Modules.Pleiger.Models
{
    public class MES_PurchaseDetail
    {
        public int No { get; set; }
        public string PONumber { get; set; }
        public string ProjectCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int Qty { get; set; }
        public int POQty { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal Amt { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public int DeliverQty { get; set; }
        public string LeadTimeType { get; set; }
        public decimal LeadTime { get; set; }
        public DateTime PlanCompleteDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; }
        public string PleigerRemark { get; set; }
        public string PORemark { get; set; }
        public string PartnerAcceptor { get; set; }
        public bool PrintingStatus { get; set; }
        public string MonetaryUnit { get; set; }
        public int? TotalPOQty { get; set; }


    }
}

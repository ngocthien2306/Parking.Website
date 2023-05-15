using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Models
{
    public class MES_Purchase
    {
        public int No { get; set; }
        public string PONumber { get; set; }
        public string UserPONumber { get; set; }
        public string ProjectCode { get; set; }
        public string ItemCode { get; set; }
        public string PartnerName { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int OrderQuantity { get; set; }
        public int POQty { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal Amt { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public int DeliverQty { get; set; }
        public string? LeadTimeType { get; set; }
        //public decimal LeadTime { get; set; }
        public decimal? LeadTime { get; set; }
        public DateTime? ArrivalRequestDate { get; set; }
        public string PurchaseUserName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? StartPurchaseDate { get; set; }
        public DateTime? EndPurchaseDate { get; set; }

        public DateTime? PlanCompleteDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string PleigerRemark { get; set; }
        //public string PORemark { get; set; }
        public string PartnerAcceptor { get; set; }
        public string PartnerCode { get; set; }
        public bool PrintingStatus { get; set; }
        public string State { get; set; }
        public string UserProjectCode { get; set; }
    }
}

using System;

namespace Modules.Pleiger.Models
{
    public class MES_ItemPO
    {
        public int? No { get; set; }

        public string PONumber { get; set; }

        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }

        public string StatusCode { get; set; }
        public string Status { get; set; }

        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        public int POQty { get; set; }
        public int? DeliverQty { get; set; }

        public string MonetaryUnit { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalPrice { get; set; } 
        public int RealQty { get; set; } 

        public string LeadTimeType { get; set; }
        public decimal LeadTime { get; set; }

        public string ItemStatusName { get; set; }
        public string ItemStatus { get; set; }

        public string PleigerRemark { get; set; }
        public string PORemark { get; set; }
        public string PlanDeliverDate { get; set; }

        public string ArrivalRequestDate { get; set; }
        public string UserPONumber { get; set; }
        public decimal? Amt { get; set; }
        public decimal? Tax { get; set; }
        public DateTime? PlanCompleteDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string PartnerAcceptor { get; set; }
        public bool PrintingStatus { get; set; }
        public string Created_By { get; set; }
        public DateTime? Created_At { get; set; }
        public string Updated_By { get; set; }
        public DateTime? Updated_At { get; set; }
        public string ProjectCode { get; set; }



    }
}

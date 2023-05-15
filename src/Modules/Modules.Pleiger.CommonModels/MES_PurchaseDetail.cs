using System;
using System.ComponentModel.DataAnnotations;

namespace Modules.Pleiger.CommonModels
{
    public class MES_PurchaseDetail
    {
        public int No { get; set; }
        public string PONumber { get; set; }
        public string UserPONumber { get; set; }
        public string ProjectCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int Qty { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public int POQty { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal Amt { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public int DeliverQty { get; set; }
        public string LeadTimeType { get; set; }
        public decimal LeadTime { get; set; }
        public DateTime? PlanCompleteDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; }
        public string PleigerRemark { get; set; }
        public string PleigerRemark2 { get; set; }
        public string PORemark { get; set; }
        public string PartnerAcceptor { get; set; }
        public bool PrintingStatus { get; set; }
        public string MonetaryUnit { get; set; }
        public int? TotalPOQty { get; set; }
        public int? MaximumreturnQty { get; set; }
        public string WHFromCode { get; set; }
        public string WHToCode { get; set; }
        public DateTime? Created_At { get; set; }
        public string Created_By { get; set; }
        public DateTime? ArrivalRequestDate { get; set; }
        public string UserCreate { get; set; }

        

    }
}

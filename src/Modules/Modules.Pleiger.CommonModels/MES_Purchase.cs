using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
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
        public string PleigerRemark2 { get; set; }        
        public string PartnerAcceptor { get; set; }
        public string PartnerCode { get; set; }
        public bool PrintingStatus { get; set; }
        public string State { get; set; }
        public string UserProjectCode { get; set; }
        public string ProjectName { get; set; }        
        public string SalesClassificationName { get; set; }
        public string Category { get; set; }
        public int DelayDay { get; set; }
        public int GoodsReceiptQty { get; set; }
        public int QtyLeft { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ReceiptPrice { get; set; }
        // Quan add columns ExportExcel PurchaseOrderList
        public string CategoryEx                { get; set; }
        public string UserPONumberEx            { get; set; }
        public string ProjectNameEx             { get; set; }
        public string PleigerRemarkEx           { get; set; }
        public DateTime? PurchaseDateEx         { get; set; }
        public DateTime? ArrivalRequestDateEx   { get; set; }
        public DateTime? DeliveryDateEx         { get; set; }
        public int DelayDayEx                   { get; set; }
        public string PartnerNameEx             { get; set; }
        public string ItemCodeEx                { get; set; }
        public string ItemNameEx                { get; set; }
        public int POQtyEx                      { get; set; }
        public int GoodsReceiptQtyEx            { get; set; }
        public int QtyLeftEx                    { get; set; }
        public string UnitEx                    { get; set; }
        public decimal ItemCostEx               { get; set; }
        public decimal TotalPriceEx             { get; set; }
        public decimal ReceiptPriceEx           { get; set; }
        public string PleigerRemark2Ex          { get; set; }
        public string PONumberEx                { get; set; }
        public string DelayDelivery { get; set; }
        public DateTime? PartnerDeliveryDate { get; set; }
        public DateTime? PartnerPlanDeliveryDate { get; set; }
    }
}

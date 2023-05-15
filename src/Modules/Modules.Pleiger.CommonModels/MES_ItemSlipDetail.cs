using System;

namespace Modules.Pleiger.CommonModels
{
    public class MES_ItemSlipDetail
    {
        public int No { get; set; }
        public string SlipNumber { get; set; }
        public int? Seq { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int? Qty { get; set; }
        public int? POQty { get; set; }

        public decimal? Cost { get; set; }
        public decimal? Amt { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TaxAmt { get; set; }
        public string Remark { get; set; }
        public DateTime? InOutYMD { get; set; }
        public string RelNumber { get; set; }
        public int? RelSeq { get; set; }
        public string ReturnReason { get; set; }
        public string OptionItem1 { get; set; }
        public string OptionItem2 { get; set; }
        public int? Position { get; set; }
        public bool? Invoice { get; set; }
        public decimal? InCost { get; set; }
        public int? CheckedQty { get; set; }
        public int? CheckedReslt { get; set; }
        public string CheckedState { get; set; }
        public int? RealQty { get; set; }
        public int? DeliverQty { get; set; } // DeliverQty khi nhap Goods Receipt PO or xuất hàn Good Issues
        public string UserProjectCode { get; set; }
        public string UserPONumber { get; set; }

        public int? TotalPOQty { get; set; }
        public int? ReceivedPOQty { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public int? MaximumreturnQty { get; set; }
        public string WHFromCode { get; set; }
        public string WHToCode { get; set; }
        public string SlipNumberGoodReciep { get; set; }
        public string POStatus { get; set; }
        public string PleigerRemark { get; set; }
        public string PleigerRemark2 { get; set; }

        // Thien
        public string QrCode { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerCeo { get; set; }
        public string Ceo { get; set; }
        public decimal? Total { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? Vat { get; set; }
        public decimal? SumVat { get; set; }
        public string RegistNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }//PlanDeliveryDate
    }
}

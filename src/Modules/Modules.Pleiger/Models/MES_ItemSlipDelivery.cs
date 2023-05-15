using System;

namespace Modules.Pleiger.Models
{
    public class MES_ItemSlipDelivery
    {
        public int? No { get; set; }
        public string SlipNumber { get; set; }
        public DateTime? SlipYMD { get; set; }
        public string SlipType { get; set; }
        public bool? IsGiftSlip { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string WHFromCode { get; set; }
        public string WHToCode { get; set; }
        public string RelNumber { get; set; }
        public string UserCreated { get; set; }
        public string TaxFlag { get; set; }
        public decimal? TotalAmt { get; set; }
        public decimal? TaxAmt { get; set; }
        public decimal? TotalTaxAmt { get; set; }
        public string Remark { get; set; }
        public string PaymentType { get; set; }
        public bool? ReqClosed { get; set; }
        public int? CheckedReslt { get; set; }
        public string CheckedState { get; set; }
        public string SlipNumExt { get; set; }
        public string ProjectCode { get; set; }
        public int? Seq { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int? Qty { get; set; }
        public int? POQty { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Amt { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TaxAmtDtl { get; set; }
        public string RemarkDtl { get; set; }
        public DateTime? InOutYMD { get; set; }
        public string RelNumberDtl { get; set; }
        public int? RelSeq { get; set; }
        public string ReturnReason { get; set; }
        public string OptionItem1 { get; set; }
        public string OptionItem2 { get; set; }
        public int? Position { get; set; }
        public bool? Invoice { get; set; }
        public decimal? InCost { get; set; }
        public int? CheckedQty { get; set; }
        public int? CheckedResltDtl { get; set; }
        public string CheckedStateDtl { get; set; }
        public string State { get; set; }
    }
}

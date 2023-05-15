using System;

namespace Modules.Pleiger.Models
{
    public class MES_ItemSlipMaster
    {
        public int No { get; set; }
        public string SlipNumber { get; set; }
        public DateTime SlipYMD { get; set; }
        public string SlipType { get; set; }
        public bool? IsGiftSlip { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string WHFromCode { get; set; }
        public string WHFromName { get; set; }
        public string WHToCode { get; set; }
        public string WHToName { get; set; }
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
        public string UserPONumber { get; set; }
        public string UserProjectCode { get; set; }
        public string PONumber { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? ArrivalRequestDate { get; set; }
        public int CheckedGoodsReturn { get; set; }
        public string SlipNumberGoodReciep { get; set; }

        //bao add
        public int? TotalReceivedQty { get; set; } //total Received Qty
        public int? TotalPOQty { get; set; }  //total POQty 




    }
}

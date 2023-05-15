using System;
using System.Collections.Generic;

namespace Modules.Pleiger.CommonModels
{
    public class MES_SlipAndIssueNumber
    {
        public string SlipNumber { get; set; }
        public string MaterialIssue { get; set; }
    }
    public class MES_ItemSlipMaster
    {
        public int No { get; set; }
        public string SlipNumber { get; set; }
        public DateTime SlipYMD { get; set; }
        public string SlipType { get; set; }
        public bool? IsGiftSlip { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
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
        public string ProjectName { get; set; }
        public string PONumber { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? ArrivalRequestDate { get; set; }
        public int CheckedGoodsReturn { get; set; }
        public string SlipNumberGoodReceipt { get; set; }
        public string SlipNumberGoodIssues { get; set; }
        public int? TotalReceivedQty { get; set; } 
        public int? TotalPOQty { get; set; }  
        public int? Qty{ get; set; }
        public int? DeliverQty { get; set; }
        public string POStatus { get; set; }
        public DateTime? PartnerPlanDeliveryDate { get; set; }
        public DateTime? PartnerDeliveryDate { get; set; }
        public List<MES_ItemSlipDetail> SlipItem { get; set; }
        public int Confirm { get; set; }
        public string ConfirmCode { get; set; }
        
        public string RemarkAfterConfrimed { get; set; }
        public string RegistNumber { get; set; }
        public string Delay { get; set; }
        public string Ceo { get; set; }
    }
}

using System;

namespace Modules.Pleiger.Models
{
    public class MES_ItemSlipDetail
    {
        public int No { get; set; }
        public string SlipNumber { get; set; }
        public int? Seq { get; set; }
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
        public int? DeliverQty { get; set; } // DeliverQty khi nhap Goods Receipt PO
        public string UserProjectCode { get; set; }
        public string UserPONumber { get; set; }

        public int? TotalPOQty { get; set; }
        public int? ReceivedPOQty { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

    }
}

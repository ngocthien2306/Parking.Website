using System;

namespace Modules.Pleiger.CommonModels
{
    public class MES_ItemPartner
    {
        public int No { get; set; }
        public int ID { get; set; }
        public int RealQty { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string CodeOfPartner { get; set; }
        public string LeadTimeType { get; set; }
        public string LeadTimeTypeName { get; set; }
        public decimal LeadTime { get; set; }
        public string MonetaryUnit { get; set; }
        public string MonetaryUnitName { get; set; }
        public decimal ItemPrice { get; set; }
        public string PleigerRemark { get; set; }
        public string PleigerRemark2 { get; set; }
        public DateTime? ArrivalRequestDate { get; set; }
        public int? POQty{ get; set; }
    }
}

namespace Modules.Pleiger.Models
{
    public class MES_ItemPartner
    {
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
        public int No { get; set; }

    }
}

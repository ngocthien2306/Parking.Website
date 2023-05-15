namespace Modules.Pleiger.Models
{
    public class MES_WHItemStock
    {
        public string WHCode { get; set; }
        public string ItemCode { get; set; }
        public string Category { get; set; }
        public string PartnerCode { get; set; }
        public int? InitQty { get; set; }        
        public int? ProcQty { get; set; }
        public int? StockQty { get; set; } // RealQty
        public string UpdatedDate { get; set; }
    }
}

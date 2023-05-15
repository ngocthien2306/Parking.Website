using System;

namespace Modules.Pleiger.CommonModels
{
    public class MES_TransClosingItems
    {
        public int No { get; set; }
        public string TransCloseNo { get; set; }
        public int SeqNo { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WHCode { get; set; }
        public string WHName { get; set; }
        public int StockQty { get; set; }
        public int DifferenceQty { get; set; }
        public DateTime? StockDate { get; set; }
        public int CheckQty { get; set; }
        public DateTime? CheckDate { get; set; }
        public string Remark { get; set; }
    }
}

namespace Modules.Pleiger.Models
{
    public class MES_Item
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string NameKor { get; set; }
        public string NameEng { get; set; }
        public string ItemCodeName { get; set; }        
        public string Category { get; set; }

        public string ItemClassCode { get; set; }
        public string ClassNameKor { get; set; }
        public string ClassNameEng { get; set; }

        public string InspectionMethod { get; set; }
        public string InspectionMethodKor { get; set; }
        public string InspectionMethodEng { get; set; }

        public string Unit { get; set; }

        public int SafetyQuantity { get; set; }
        public int StkQty { get; set; }
        public int RealQty { get; set; }
    }
}

namespace Modules.Pleiger.CommonModels
{
    public class Respone
    {
        public string Status { get; set; }
    }
    public class MES_Item
    {
        public int? No { get; set; }

        public string ValueQuantity { get; set; }
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
        public string ProcessingClassification { get; set; }
        public string UseYN { get; set; }
        public string UseStatus { get; set; }
        public string Standard { get; set; }
        public int COUNT { get; set; }
        public int DELETEYN1 { get; set; }
        public int DELETEYN2 { get; set; }
        public int DELETEYN3 { get; set; }
        public int DELETEYN4 { get; set; }

        public byte[] QrCode { get; set; }
    }
}

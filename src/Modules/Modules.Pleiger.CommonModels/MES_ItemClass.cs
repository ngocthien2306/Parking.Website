namespace Modules.Pleiger.CommonModels
{
    public class MES_ItemClass
    {
        public int No { get; set; }
        public string ItemClassCode { get; set; }
        public string ItemUpCode { get; set; }
        public string ItemComCode { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public string ClassNameKor { get; set; }
        public string ClassNameEng { get; set; }
        public string ItemClassCodeName { get; set; }        
        public string Etc { get; set; }
        public string ItemCode { get; set; }
        public string NameKor { get; set; }

        //Lookup In MES_Item Management ItemGrid
        public string ID { get; set; }
        public string Name { get; set; }
    }
}

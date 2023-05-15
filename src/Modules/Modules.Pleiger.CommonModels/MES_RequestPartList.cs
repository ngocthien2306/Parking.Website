namespace Modules.Pleiger.CommonModels
{
    public class MES_RequestPartList
    {
        public int No { get; set; }
        public string RequestCode { get; set; }
        public string ItemCode { get; set; }
        public int ReqQty { get; set; }
        public int StkQty { get; set; }
        public int POQty { get; set; }       
        public int POFnQty { get; set; }
    }
}

namespace Modules.Admin.Models
{
    public class SYPageActionDetails
    {
        public int PAG_ID { get; set; }
        public int ACT_ID { get; set; }
        //public int MAP_ID { get; set; }
        public int SOURCE_ID { get; set; }
        public string ACT_TYPE { get; set; }
        public string MAP_ID_CONVERT { get; set; }
        public string ACT_FN { get; set; } // Function script name
        public int EXEC_SEQ { get; set; }
        public bool RUN_TRXN { get; set; }        //true/false - Transactional or not
        public SYDataMap dataMap { get; set; }
        public SYPageLayout dataPage { get; set; }
        public string PAG_KEY { get; set; }
        public int PAG_REDIRECT { get; set; }
    }
}

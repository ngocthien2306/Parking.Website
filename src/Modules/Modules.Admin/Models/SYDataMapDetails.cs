namespace Modules.Admin.Models
{
    public class SYDataMapDetails
    {
        public int MAP_ID { get; set; }
        public int MDTL_ID { get; set; }
        public int PAG_ID { get; set; }
        //public string PEL_ID { get; set; }
        public string MAP_TO { get; set; }
        public string MAP_FROM { get; set; }
        //public string MAP_FLD { get; set; }
        public int FLD_IO { get; set; }
        public string FLD_IO_CONVERT { get; set; }
        public string MDTL_DTYPE { get; set; }
        public string FLD_TYPE { get; set; }
        public string FRM_PEL_ID { get; set; }
        public string PEL_TYP { get; set; }
    }
}

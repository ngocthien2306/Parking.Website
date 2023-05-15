using System;

namespace Modules.Pleiger.CommonModels
{
    public class MES_TransClosingDtls
    {
        public int No { get; set; }
        public string TransCloseNo { get; set; }
        public string TransMonth { get; set; }
        public int ItemCount { get; set; }
        public DateTime? TransCloseDate { get; set; }
        public DateTime? DownloadDate { get; set; }
        public DateTime? UploadDate { get; set; }
        public string SearchConds { get; set; }
        public string Remark { get; set; }
        public string Created_By { get; set; }
    }
}

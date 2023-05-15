using Modules.Common.Models;

namespace Modules.Common.Models
{
    public class SYFileUploadMaster
    {
        public string FileID { get; set; }
        public string FileGuid { get; set; }
        public string FilePath { get; set; }
        public SYFileUpload FileDetail { get; set; }
    }
}

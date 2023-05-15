using System.Collections.Generic;

namespace Modules.Admin.Models
{
    public class PageElement
    {
        public string EleID { get; set; }
        public string EleName { get; set; }
        public string SeqNo { get; set; }
        public string Type { get; set; }
        public string PageID { get; set; }
        public string SPInsert { get; set; }
        public string SPUpdate { get; set; }
        public string SPDelete { get; set; }
        public List<PageElementDetail> lstElementDetail { get; set; }
        public List<PageMapping> lstMapping { get; set; }
        public PageElement()
        {
            lstElementDetail = new List<PageElementDetail>();
        }
    }
}

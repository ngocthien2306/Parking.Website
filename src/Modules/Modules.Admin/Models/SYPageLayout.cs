using System.Collections.Generic;

namespace Modules.Admin.Models
{
    public class SYPageLayout
    {
        public int PAG_ID { get; set; }
        public string PAG_KEY { get; set; }
        public string PAG_TYPE { get; set; }
        public string PAG_TITLE { get; set; }
        public int PAG_WDT { get; set; }
        public int PAG_HGT { get; set; }
        public string CUSTM_VIEW { get; set; }
        public int SITE_ID { get; set; }
        public List<SYPageLayElements> listPageElement { get; set; }
        public List<SYPageActions> listAction { get; set; }
        public List<SYToolbarActions> listToolbar { get; set; }
        public List<SYPageLayElementReference> listReference { get; set; }
        public SYPageLayout()
        {
            this.listPageElement = new List<SYPageLayElements>();
            this.listAction = new List<SYPageActions>();
            this.listToolbar = new List<SYToolbarActions>();
            this.listReference = new List<SYPageLayElementReference>();
        }
    }
}
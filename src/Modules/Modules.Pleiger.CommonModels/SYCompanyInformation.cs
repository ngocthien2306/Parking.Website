using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.CommonModels
{
    public class SYCompanyInformation
    {
        public int BusinessNumber { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Certificate { get; set; }
        public string SystemLicense { get; set; }
        public DateTime StartDate { get; set; }
        public bool UseApproval { get; set; }
        public string CompnayLogo { get; set; }
        public string SystemProvider { get; set; }
    }
}

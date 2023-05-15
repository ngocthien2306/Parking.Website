using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.UIRender.Models
{
    public class DynamicJsonModel
    {
        public string Site { get; set; }
        public IEnumerable<DynamicTable> DynamicTable { get; set; }
    }

    public class DynamicTable
    {
        public string PageName { get; set; }
        public string DBTblName { get; set; }
        public string ModelName { get; set; }
        public string AssemblyName { get; set; }
        public string SPName { get; set; }
        public string TemplateName { get; set; }

    }
}

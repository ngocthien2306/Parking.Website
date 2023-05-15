using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class ToolbarInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public int Sort { get; set; }
        public string Type { get; set; }
        public int MenuID { get; set; }
        public string Icon { get; set; }
    }
}

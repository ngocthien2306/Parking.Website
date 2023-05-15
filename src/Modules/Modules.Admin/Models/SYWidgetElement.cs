using InfrastructureCore.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class SYWidgetElement : HLCoreCrudModel
    {
        public int Id { get; set; }
        public string WidgetNumber { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }   
        public bool Status { get; set; }
        public string SPName { get; set; }
        public List<SYWidgetElementDetail> widgetElementDetail { get; set; }
    }
}

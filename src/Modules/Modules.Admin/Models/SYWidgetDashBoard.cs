using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Models
{
    public class SYWidgetDashBoard
    {
        public SYWidgetDashBoard()
        {
            dashBoardWidgets = new List<SYWidgetElement>();
        }
        public List<SYWidgetElement> dashBoardWidgets { get; set; } 
        //hoặc nhiều list của nhiều element phân biệt
    }
}

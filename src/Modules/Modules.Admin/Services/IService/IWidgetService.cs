using Modules.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Admin.Services.IService
{
    public interface IWidgetService
    {
        List<SYWidgetElement> GetAllWidgetMst();
        List<SYWidgetElementDetail> GetWidgetDtl(string widgetNumber);
        List<SYWidgetElementDetail> GetAllWidgetDtl();
    }
   
}

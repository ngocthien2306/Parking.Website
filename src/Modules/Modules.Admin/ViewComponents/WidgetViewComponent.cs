using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLFXFramework.WebHost.ViewComponents
{
    public class WidgetViewComponent : ViewComponent
    {
        private readonly IWidgetService _widgetService;
        public WidgetViewComponent(IWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string WidgetNumber) {

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var widgetDtl = _widgetService.GetWidgetDtl(WidgetNumber);
           
            return View(widgetDtl);              
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;

namespace Modules.Admin.Controllers
{
    public class WidgetMgtController : BaseController
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWidgetService _widgetService;
        private readonly IDynamicWidgetService _dynamicWidgetService;

        public WidgetMgtController(IHttpContextAccessor contextAccessor, 
            IWidgetService widgetService, IDynamicWidgetService dynamicWidgetService ) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _widgetService = widgetService;
            _dynamicWidgetService = dynamicWidgetService;
        }

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            if (CurrentUser == null)
            {
                return NotFound();
            }
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuID = curMenu != null ? curMenu.MenuID : 0;
            return View();
        }
        [HttpGet]
        public object GetListWidgetMST(DataSourceLoadOptions loadOptions)
        {
            var loadResult = _widgetService.GetAllWidgetMst();
            return DataSourceLoader.Load(loadResult, loadOptions); 
        }
        [HttpGet]
        public object GetWidgetDtl(string WidgetNumber)
        {
            var widgetDtl = _widgetService.GetWidgetDtl(WidgetNumber);
          //  dynamic data = _dynamicWidgetService.ExecuteProcedure(spname, lstParam, connectionType); //test thử
            return widgetDtl;
        }
        public IActionResult IndexVC(string WidgetNumber)
        {
            return ViewComponent("WidgetDtl", new { WidgetNumber = WidgetNumber });
        }
    }
}

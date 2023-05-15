using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Pleiger.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Modules.Pleiger.Controllers
{
    public class MESReportTestController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBoardManagementService _boardManagementService;
        private readonly IWidgetService _widgetService;
        public MESReportTestController(IHttpContextAccessor contextAccessor,
            IBoardManagementService boardManagementService, IWidgetService widgetServicee) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _boardManagementService = boardManagementService;
            _widgetService = widgetServicee;
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
            //GetAllWidgetDtl
            List<SYWidgetElement> listMst = _widgetService.GetAllWidgetMst();
            return View(listMst);
        }

      
    }
}

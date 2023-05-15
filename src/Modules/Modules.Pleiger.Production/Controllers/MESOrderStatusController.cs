using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Pleiger.Production.Controllers
{
    public class MESOrderStatusController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
    
        private readonly IMESOrderStatusService _orderStatusService;



        public MESOrderStatusController(IHttpContextAccessor contextAccessor, IMESOrderStatusService orderStatusService) : base(contextAccessor)
        {
          
            _orderStatusService = orderStatusService;
            _contextAccessor = contextAccessor;

        }
        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }


        //load Data
        [HttpGet]
        public IActionResult SearchOrderStatus(MES_OrderStatus orderStatus)
        {
            var result = _orderStatusService.searchOrderStatus(orderStatus);
            return Json(result);
        }
    }
}

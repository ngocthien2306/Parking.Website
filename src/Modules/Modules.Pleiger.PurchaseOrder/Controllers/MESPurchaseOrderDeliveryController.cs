using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using Newtonsoft.Json;
using System;

namespace Modules.Pleiger.PurchaseOrder.Controllers
{
    public class MESPurchaseOrderDeliveryController : BaseController
    {
        private readonly IMESPurchaseOrderDeliveryService _mESPurchaseOrderDeliveryService;
        private readonly IHttpContextAccessor _contextAccessor;

        public MESPurchaseOrderDeliveryController(IMESPurchaseOrderDeliveryService mESPurchaseOrderDeliveryService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._mESPurchaseOrderDeliveryService = mESPurchaseOrderDeliveryService;
            this._contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            SYMenuAccess pageSetting = new SYMenuAccess();
            ViewBag.PageSetting = pageSetting;

            return View();
        }

        [HttpGet]
        public IActionResult SearchPurchaseOrderDelivery(string DeliveryStart, string DeliveryEnd, string UserPONumber)
        {
            var result = _mESPurchaseOrderDeliveryService.SearchPurchaseOrderDelivery(DeliveryStart, DeliveryEnd, UserPONumber);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpGet]
        public IActionResult SearchDeliveryDetail(string PONumber)
        {
            var result = _mESPurchaseOrderDeliveryService.SearchDeliveryDetail(PONumber);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }
    }
}

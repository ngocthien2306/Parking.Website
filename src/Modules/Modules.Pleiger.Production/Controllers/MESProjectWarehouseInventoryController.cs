using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Pleiger.Production.Controllers
{
    public class MESProjectWarehouseInventoryController : BaseController
    {
        private readonly IMESProjectWarehouseInventoryService _mESProjectWarehouseInventoryService;
        private readonly IHttpContextAccessor _contextAccessor;

        public MESProjectWarehouseInventoryController(IMESProjectWarehouseInventoryService mESProjectWarehouseInventoryService , IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._mESProjectWarehouseInventoryService = mESProjectWarehouseInventoryService;
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
        public IActionResult SearchProjectWarehouseInventory(string WarehouseName, string ProductionProjectCode, string Category,string SalesOrderProjectCode)
        {
            var result = _mESProjectWarehouseInventoryService.SearchProjectWarehouseInventoryNew(WarehouseName, ProductionProjectCode, Category, SalesOrderProjectCode);
            //var data = JsonConvert.SerializeObject(result);
            //return Content(data, "application/json");
            return Json(result);
        }

        [HttpGet]
        public object GetComBoBoxWareHouseInventory(DataSourceLoadOptions loadOptions)
        {
            var data = _mESProjectWarehouseInventoryService.GetComBoBoxWareHouseInventory();
            //var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Json(data);
        }
        
        [HttpGet]
        public IActionResult GetProjectWarehouseInventoryDetail(string ProjectCode, string WarehouseCode)
        {
            var result = _mESProjectWarehouseInventoryService.GetProjectWarehouseInventoryDetail(ProjectCode, WarehouseCode);
          
            return Json(result);
        }
        [HttpPost]
        public IActionResult GetProjectWarehouseInventoryListDetail(string listWarehouseInventory)
        {
            var result = _mESProjectWarehouseInventoryService.GetProjectWarehouseInventoryListDetail(listWarehouseInventory);

            return Json(result);
        }
    }
}

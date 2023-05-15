using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
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
    public class MESMaterialsStatusController : BaseController
    {
        private readonly IMESMaterialsStatusService _MESMaterialsStatusService;
        private readonly IHttpContextAccessor _contextAccessor;

        public MESMaterialsStatusController(IMESMaterialsStatusService MESMaterialsStatusService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._MESMaterialsStatusService = MESMaterialsStatusService;
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
            return View();
        }

        [HttpGet]
        public object LoadCategoryCombobox(DataSourceLoadOptions loadOptions)
        {
            var result = _MESMaterialsStatusService.getCategoryCombobox("en");
            return DataSourceLoader.Load(result, loadOptions);
        }
        
        [HttpGet]
        public object LoadTypeRadio(DataSourceLoadOptions loadOptions)
        {
            var result = _MESMaterialsStatusService.getTypeRadio(CurrentLanguages.Substring(1));
            return DataSourceLoader.Load(result, loadOptions);
        }

        [HttpPost]
        public IActionResult SearchMaterialStatus(string StartDate, string EndDate, string InOutType, string Category, string ItemCode, string ItemName)
        {
            var result = _MESMaterialsStatusService.searchMaterialStatus(StartDate, EndDate, InOutType, Category, ItemCode, ItemName);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }
    }
}

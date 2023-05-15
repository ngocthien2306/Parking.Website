using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Controllers;
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
    public class MESProductionLineController : BaseController
    {
        private readonly IMESProjectWarehouseInventoryService _mESProjectWarehouseInventoryService;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IMESProductionLineService _MESProductionLineService;

        public MESProductionLineController(IMESProjectWarehouseInventoryService mESProjectWarehouseInventoryService , IHttpContextAccessor contextAccessor, IMESProductionLineService MESProductionLineService) : base(contextAccessor)
        {
            this._mESProjectWarehouseInventoryService = mESProjectWarehouseInventoryService;
            this._contextAccessor = contextAccessor;
            this._MESProductionLineService = MESProductionLineService;
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
        public IActionResult SearchProductLine(string InternalExternal, string MaterialWarehouseCode, string ProductionLineNameEng, string ProductionLineNameKor, string ProductionLineCode)
        {
            string whCode = InternalExternal == "Internal" ? "INEX01" : InternalExternal == "External" ? "INEX02" : "All";
            var result = _MESProductionLineService.searchProductionLines(whCode, MaterialWarehouseCode, ProductionLineNameEng, ProductionLineNameKor, ProductionLineCode);

            return Json(result);
        }


        [HttpGet]
        public IActionResult MaterialWarehouseCodeCombobox()
        {
            var result = _MESProductionLineService.MaterialWarehouseCodeCombobox();
            return Json(result);
        }


        [HttpGet]
        public IActionResult FinishWarehouseCodeCombobox()
        {
            var result = _MESProductionLineService.FinishWarehouseCodeCombobox();
            return Json(result);
        }


        [HttpGet]
        public IActionResult ProductManagerLineCombobox()
        {
            var result = _MESProductionLineService.ProductManagerLineCombobox();
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetPartnerComboboxCombobox()
        {
            var result = _MESProductionLineService.GetPartnerComboboxCombobox();
            return Json(result);
        }


       
        [HttpPost]
        public IActionResult SaveProductLine(string ArrIns, string ArrUpd, string ArrDel)
        {
            try
            {
                List<MES_ProductLine> ArrInsLst = JsonConvert.DeserializeObject<List<MES_ProductLine>>(ArrIns);
                List<MES_ProductLine> ArrUdpLst = JsonConvert.DeserializeObject<List<MES_ProductLine>>(ArrUpd);
                List<MES_ProductLine> ArrDelLst = JsonConvert.DeserializeObject<List<MES_ProductLine>>(ArrDel);

                var insertResult = _MESProductionLineService.CRUDProductLine(ArrInsLst, ArrUdpLst, ArrDelLst, CurrentUser.UserID);

                return Json(insertResult);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }

        }
    }
}

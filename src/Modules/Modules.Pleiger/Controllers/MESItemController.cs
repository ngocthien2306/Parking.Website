using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System;

namespace Modules.Pleiger.Controllers
{
    public class MESItemController : BaseController
    {
        #region Prop
        private readonly IItemClassService _mesItemClassService;
        private readonly IMESItemService _mesItemService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public MESItemController(IItemClassService mesItemClassService, IMESItemService mesItemService,
            IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._mesItemClassService = mesItemClassService;
            this._mesItemService = mesItemService;
            this._contextAccessor = contextAccessor;
        }
        #endregion
        /// <summary>
        /// Mapping with setting in DB page PARTNER_MGT_DETAIL_POPUP, type = G001C004, view custom name = ~/Views/MESPartner/PartnerDetail.cshtml
        /// </summary>
        /// <returns></returns>
        public IActionResult ItemAddonJS()
        {
            return View();
        }

        public IActionResult ItemPartnerAddonJS()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }


        [HttpPost]
        public JsonResult IsItemCodeMstExisted(string Key, string Value)
        {
            bool isValid = _mesItemService.IsItemCodeMstExisted(Value.ToString());
            return Json(isValid);
        }


        // Get list Item by Category
        [HttpGet]
        public IActionResult GetItemsByCategoryCode(DataSourceLoadOptions loadOptions, string categoryCode)
        {
            var data = _mesItemService.GetItemsByCategoryCode(categoryCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }


        // Get list Material
        [HttpGet]
        public IActionResult GetListMaterial(DataSourceLoadOptions loadOptions)
        {
            var data = _mesItemService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }


        // Get list Product, half product
        [HttpGet]
        public IActionResult GetListFinishItem(DataSourceLoadOptions loadOptions)
        {
            var data = _mesItemService.GetListFinishItem();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        // Get list Material
        [HttpGet]
        public IActionResult GetDataReferByItemCode(DataSourceLoadOptions loadOptions, string itemCode)
        {
            var data = _mesItemService.GetDetail(itemCode);
            return Json(data);
        }
        // Get list Item In WereHouse
        [HttpGet]
        public IActionResult GetItemInWareHouse(DataSourceLoadOptions loadOptions, string WareHouseCode)
        {
            var data = _mesItemService.GetItemInWareHouse(WareHouseCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        // Get list Item In WereHouse by ItemCode
        public IActionResult GetItemInWareHousebyItemCode(DataSourceLoadOptions loadOptions, string WareHouseCode, string ItemCode)
        {
            var data = _mesItemService.GetItemInWareHousebyItemCode(WareHouseCode, ItemCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult SearchItemInWareHouse(DataSourceLoadOptions loadOptions, string WareHouseCode,string ItemCode,string ItemName)
        {
            var data = _mesItemService.SearchItemInWareHouse(WareHouseCode, ItemCode, ItemName);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
    }
}

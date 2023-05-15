using ClosedXML.Excel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using InfrastructureCore.Web.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Modules.Pleiger.Controllers
{
    //[Authorize]
    public class MESBOMMgtController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESBOMMgtService _mESBOMMgtService;
        private readonly IItemClassService _itemClassService;
        private readonly IMESItemService _mESItemService;

        public MESBOMMgtController(IMESBOMMgtService mESBOMMgtService, IHttpContextAccessor contextAccessor, IItemClassService itemClassService, IMESItemService mESItemService) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _mESBOMMgtService = mESBOMMgtService;
            _itemClassService = itemClassService;
            _mESItemService = mESItemService;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            // ViewBag.MenuId = (curMenu) != null ? curMenu.MenuID : 67;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;

            return View();
        }

        [HttpGet]
        public object GetItemFinish(DataSourceLoadOptions loadOptions)
        {
            var data = _mESBOMMgtService.GetItemFinish();
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object LoadItemClass(DataSourceLoadOptions loadOptions)
        {
            var data = _mESBOMMgtService.LoadItemClass();
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object GetItemByClassCode(DataSourceLoadOptions loadOptions, string itemClassCode)
        {
            var data = _mESBOMMgtService.GetItemFinish();
            var dt = data.Where(m => m.ItemClass == itemClassCode).ToList();
            return DataSourceLoader.Load(dt, loadOptions);
        }

        #endregion

        #region SEARCH

        public IActionResult GetDataSearch(string ItemCode, string ParentItemLevel)
        {
            var result = _mESBOMMgtService.GetDataSearch(ItemCode, ParentItemLevel);

            return Content(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region POPUP

        public IActionResult BOMCRUDPopup(MES_BOM SelectedItem, string viewbagIndex, string viewName)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = viewbagIndex;
            var model = new MES_BOM();
            if (SelectedItem.Id != 0)
            {
                //model = _mESBOMMgtService.GetDataDetail(projectCode);
            }
            //return PartialView("BOMPopupCRUD", model);
            return PartialView(viewName, model);
        }

        public IActionResult GetItemClassCodeByCategory(DataSourceLoadOptions loadOptions, string categoryCode)
        {
            var result = _itemClassService.GetListItemClassByCategory(categoryCode);
            var loadResult = DataSourceLoader.Load(result, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult));
        }

        public IActionResult GetItemByItemClass(DataSourceLoadOptions loadOptions, string itemClassCode, string categoryCode)
        {
            var result = _mESBOMMgtService.GetListItem(itemClassCode, categoryCode);
            var loadResult = DataSourceLoader.Load(result, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult));
        }

        public IActionResult SearchItemPopup(DataSourceLoadOptions loadOptions, string categoryCode, string itemClassCode, string itemName, string itemCode)
        {
            var result = _mESBOMMgtService.SearchItemPopup(categoryCode, itemClassCode, itemName, itemCode);
            var loadResult = DataSourceLoader.Load(result, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult));
        }

        public IActionResult SearchItemPopupGetOther(DataSourceLoadOptions loadOptions, string categoryCode, string itemClassCode, string itemName, string itemCode)
        {
            var result = _mESBOMMgtService.SearchItemPopupGetOther(categoryCode, itemClassCode, itemName, itemCode);
            var loadResult = DataSourceLoader.Load(result, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult));
        }
        
        #endregion

        #region CRUD

        [HttpPost]
        public IActionResult InsertParentItem(MES_BOM mesBom)
        {
            var result = _mESBOMMgtService.InsertParentItem(mesBom);

            return Content(JsonConvert.SerializeObject(result));
        }

        public IActionResult InsertBOMItems(string InsertArr, string UpdateArr, string DeleteArr, string InsertArrFromGetOther)
        {
            List<MES_BOM> mES_BOMs_InsertArr = JsonConvert.DeserializeObject<List<MES_BOM>>(InsertArr);
            List<MES_BOM> mES_BOMs_UpdateArr = JsonConvert.DeserializeObject<List<MES_BOM>>(UpdateArr);
            List<MES_BOM> mES_BOMs_DeleteArr = JsonConvert.DeserializeObject<List<MES_BOM>>(DeleteArr);

            if(mES_BOMs_InsertArr.Any() == true || mES_BOMs_UpdateArr.Any() == true || mES_BOMs_DeleteArr.Any() == true)
            {
                var result = _mESBOMMgtService.InsertBOMItems(mES_BOMs_InsertArr, mES_BOMs_UpdateArr, mES_BOMs_DeleteArr);
                return Content(JsonConvert.SerializeObject(result));
            }    

            //TODO: not done push list json to db and insert recursive data
            if(!string.IsNullOrEmpty(InsertArrFromGetOther))
            {
                var result = _mESBOMMgtService.InsertBOMTreeJSON(InsertArrFromGetOther);
                return Content(JsonConvert.SerializeObject(result));
            }    

            return Content(JsonConvert.SerializeObject(new Result()));
        }    

        #endregion

    }
}
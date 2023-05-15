using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Modules.Pleiger.Controllers
{
    public class MESItemClassController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IItemClassService itemClassService;
        private readonly IMESComCodeService mESComCodeService;

        public MESItemClassController(IHttpContextAccessor contextAccessor, IItemClassService itemClassService, IMESComCodeService mESComCodeService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this.itemClassService = itemClassService;
            this.mESComCodeService = mESComCodeService;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.Thread = Guid.NewGuid().ToString("N");

            return View();
        }

        [HttpGet]
        public IActionResult GetListData()
        {
            var result = itemClassService.GetListData();
            var model = JsonConvert.SerializeObject(result);

            return Json(model);
        }

        // Check ItemClassCode
        [HttpGet]
        public IActionResult CheckItemClassCode(string itemClassCode, string itemComCode)
        {
            var result = false;
            var data = itemClassService.GetItemClassByCode(itemClassCode, itemComCode);

            result = data == null ? false : true;

            return Json(result);
        }

        // Get list Item Common Code
        [HttpGet]
        public IActionResult GetListItemComCode(string groupCode)
        {
            var data = mESComCodeService.GetListComCodeDTL(groupCode);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        // Get list Item Up Code
        [HttpGet]
        public IActionResult GetListItemUpCode(string itemComCode)
        {
            var data = itemClassService.GetListItemUpCode(itemComCode);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        // Get list Item Category
        [HttpGet]
        public IActionResult GetItemClassByCategory(DataSourceLoadOptions loadOptions)
        {
            var data = itemClassService.GetItemClassByCategory();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get list Item Category IMTP03,04
        [HttpGet]
        public IActionResult GetItemClassByCategory0304(DataSourceLoadOptions loadOptions)
        {
            var data = itemClassService.GetItemClassByCategory0304();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        
        #endregion

        #region "Insert - Update - Delete"

        // Save data item class
        [HttpPost]
        public IActionResult SaveItemClass(string itemClassCode, string itemComCode, string itemUpCode, string itemCategory, string classNameKor, string classNameEng, string etc)
        {
            var result = itemClassService.SaveItemClass(itemClassCode, itemComCode, itemUpCode, itemCategory, classNameKor, classNameEng, etc, CurrentUser.UserID);
            return Json(result);
        }

        // Delete Item Class
        [HttpPost]
        public IActionResult DeleteItemClass(string itemClassCode, string itemComCode)
        {
            var result = itemClassService.DeleteItemClass(itemClassCode, itemComCode);
            return Json(result);
        }

        #endregion


        #region Get data show in combo box-search form

        [HttpGet]
        public object GetListItemClassByCategory(DataSourceLoadOptions loadOptions, string categorySelected)
        {
            //var data = itemClassService.GetListItemClassByCategory(categorySelected);
            //var loadResult = DataSourceLoader.Load(data, loadOptions);
            //return Content(JsonConvert.SerializeObject(loadResult), "application/json");

            List<MES_ItemClass> data = new List<MES_ItemClass>();
            data = itemClassService.GetListItemClassByCategory(categorySelected);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object GetListItemClassSub1ByItemClass(DataSourceLoadOptions loadOptions, string categorySelected, string itemClassSelected)
        {
            //var data = itemClassService.GetListItemClassByCategory(categorySelected);
            //var loadResult = DataSourceLoader.Load(data, loadOptions);
            //return Content(JsonConvert.SerializeObject(loadResult), "application/json");

            List<MES_ItemClass> data = new List<MES_ItemClass>();
            data = itemClassService.GetListItemClassSub1ByItemClass(categorySelected, itemClassSelected);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object GetListItemClassSub2ByItemClassSub1(DataSourceLoadOptions loadOptions, string categorySelected, string itemClassSub1Selected)
        {
            //var data = itemClassService.GetListItemClassByCategory(categorySelected);
            //var loadResult = DataSourceLoader.Load(data, loadOptions);
            //return Content(JsonConvert.SerializeObject(loadResult), "application/json");

            List<MES_ItemClass> data = new List<MES_ItemClass>();
            data = itemClassService.GetListItemClassSub2ByItemClassSub1(categorySelected, itemClassSub1Selected);
            return DataSourceLoader.Load(data, loadOptions);
        }
        #endregion
    }
}

using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using InfrastructureCore.Web.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
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
    public class MESSaleProjectController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IMESItemService _mESItemService;
        private readonly IPurchaseService _purchaseService;
        private readonly IUserService _userService;

        public MESSaleProjectController(IHttpContextAccessor contextAccessor, 
            IMESComCodeService mesComCodeService, 
            IMESSaleProjectService mesSaleProjectService, 
            IMESItemService mESItemService,
            IPurchaseService purchaseService,
            IUserService _userService) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _mesComCodeService = mesComCodeService;
            _mesSaleProjectService = mesSaleProjectService;
            _mESItemService = mESItemService;
            _purchaseService = purchaseService;
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
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.UserCode = CurrentUser.UserCode;
  
            return View();
        }

        [HttpGet]
        public IActionResult SalesProjectCreatePopup(string projectCode,string viewbagIndex)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = viewbagIndex;
            var model = new MES_SaleProject();
            if (projectCode != null)
            {
                model = _mesSaleProjectService.GetDataDetail(projectCode);
            }
            return PartialView("SalesProjectCreatePopup", model);
        }
        //getItemByItemClassCode
        public object GetItemByItemClassCode(string itemClassCode, DataSourceLoadOptions loadOptions)
        {
            var data = _mESItemService.getItemsByItemClassCode(itemClassCode);
            return DataSourceLoader.Load(data, loadOptions);
            //return Json(data);
        }
        public object GetItemCodeNameByItemClassCode(string itemClassCode, DataSourceLoadOptions loadOptions)
        {
            var data = _mESItemService.getItemsByItemClassCode(itemClassCode);
            List<DynamicCombobox> list = new List<DynamicCombobox>();
            foreach (var item in data)
            {
                DynamicCombobox a = new DynamicCombobox();
                a.ID = item.ItemCode;
                a.Name = item.ItemCode + " - " + item.NameEng;
                list.Add(a);
            }
            return DataSourceLoader.Load(list, loadOptions);

           
        }
        [HttpGet]
        public object GetProjectStatus(DataSourceLoadOptions loadOptions)
        {
            var data = _mesSaleProjectService.GetProjectStatus();
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object GetUserProjectCode(DataSourceLoadOptions loadOptions)
        {
            var data = _mesSaleProjectService.GetUserProjectCode();
            return DataSourceLoader.Load(data, loadOptions);
        }
        /// get project status with no field all   
        [HttpGet]
        public object GetProjectStatusWithNoAll(DataSourceLoadOptions loadOptions)   
        {
            ///chuan bi xu ly
            return null;
        }

        [HttpGet]
        public IActionResult ShowPopupGenerateCode( string theadID)
        {
            //Show page popup PopupDataPageActionDetails 
            ViewBag.Thread = theadID;
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var year = date.Substring(8, 2);
            var month = date.Substring(3, 2);
            ViewBag.year = year;
            ViewBag.month = month;
            return PartialView("ShowGenerateCode");
        }

        [HttpGet]
        public IActionResult CheckProjectCodeDuplicate(string ProjectCode)
        {
            var data = _mesSaleProjectService.GetDataDetail(ProjectCode);
            return Json(new { result = data });
        }
        //Check Duplicate DuplicateCode
        [HttpGet]
        public IActionResult CheckDuplicate(string DuplicateCode, string Type)
        {
            var data = _mesSaleProjectService.CheckDuplicate(DuplicateCode, Type);
            return Json(new { result = data });
        }
        // Get list SaleProject
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions)
        {
            var data = _mesSaleProjectService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListAllData(DataSourceLoadOptions loadOptions)
        {
            var data = _mesSaleProjectService.GetListAllData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListProductType(string data)
        {
            var listdata = data;

            return Content(JsonConvert.SerializeObject(listdata), "application/json");
        }
        #endregion

        #region "Insert - Update - Delete
        [HttpPost]
        public IActionResult SaveSalesProject(string data)
        {
            MES_SaleProject saleProject = JsonConvert.DeserializeObject<MES_SaleProject>(data);
            var result = _mesSaleProjectService.SaveSalesProject(saleProject, CurrentUser.UserID);
            return Json(result);
        }
    
        [HttpPost]
        public IActionResult  DeleteSalesProjects(string projectCode)
        {  
            var result = _mesSaleProjectService.DeleteSalesProjects(projectCode);
            return Json(result);
        }

        #endregion
       
        #region SearchSaleProjects
        [HttpGet]
        public object SearchSaleProjects(DataSourceLoadOptions loadOptions, MES_SaleProject model)
        {
            var result = _mesSaleProjectService.SearchSaleProject(model);           
            //return DataSourceLoader.Load(result, loadOptions);
            return Json(result);
        #endregion 
        }
    }
}

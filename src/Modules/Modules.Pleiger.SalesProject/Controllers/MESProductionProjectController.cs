using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.SalesProject.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Pleiger.SalesProject.Controllers
{
    public class MESProductionProjectController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IUserService _userService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
         private readonly IMESProductionProjectService _mesProductionProject;

        public MESProductionProjectController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IUserService userService, IMESSaleProjectService mesSaleProjectService, IMESProductionProjectService mesProductionProject) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _accessMenuService = accessMenuService;
            _userService = userService;
            _mesSaleProjectService = mesSaleProjectService;
            _mesProductionProject = mesProductionProject;
        }
        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.EXPORT_EXCEL_ICUBE_YN = false;
            ViewBag.CREATE_YN = false;
            ViewBag.SEARCH_YN = false;
            ViewBag.EXCEL_YN = false;

            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
     
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.EXPORT_EXCEL_ICUBE_YN = ListButtonPermissionByUser[0].EXPORT_EXCEL_ICUBE_YN;
                ViewBag.CREATE_YN = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.SEARCH_YN = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.EXCEL_YN = ListButtonPermissionByUser[0].EXCEL_YN;
            }

            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.UserCode = CurrentUser.UserCode;

            return View();
        }

        [HttpGet]
        public IActionResult ProductionProjectCreatePopup(string projectCode, string viewbagIndex, int menuParent)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");

            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = viewbagIndex;
            var model = new MES_SaleProject();
            if (projectCode != null)
            {
                model = _mesSaleProjectService.GetDataDetail(projectCode);
            }
            return PartialView("ProductionProjectCreatePopup", model);
        }

        [HttpGet]
        public IActionResult showPopupGetSalesOrderProject(string idParent)
        {
            var model = new MES_SalesOrderProjectNew();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.idParent = idParent;
            return PartialView("PopupGetSalesOrderProject", model);
        }

        [HttpGet]
        public IActionResult showPopupProductionRequest(string viewbagIndex)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");      
            
            ViewBag.idParent = viewbagIndex;
            return PartialView("PopupGetProductionRequest");
        }

        [HttpGet]
        public IActionResult GetListSalesOrderProjectPopup(string ProjectOrderType, string OrderNumber, string SalesOrderProjectName, string SalesOrderProjectCode)
        {
            var result = _mesProductionProject.GetListSalesOrderProjectPopup(ProjectOrderType, OrderNumber, SalesOrderProjectName, SalesOrderProjectCode);
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetListCustomerCombobox()
        {
            var result = _mesProductionProject.GetAllCustomerCombobox();
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetListOrderTeamCombobox()
        {
            var result = _mesProductionProject.GetAllOrderTeamCombobox();
            return Json(result);
        }

        [HttpGet]
        public object SearchRequestProduction(string model)
        {
            MES_SaleProject modelParsed = JsonConvert.DeserializeObject<MES_SaleProject>(model);

            var result = _mesProductionProject.SearchRequestProduction(modelParsed);
            return Json(result);
        }

        [HttpPost]
        public IActionResult SaveRequestProduction(string listRequestProduction,string ProdReqRequestType)
        {
            List<MES_SaleProject> listSP = JsonConvert.DeserializeObject<List<MES_SaleProject>>(listRequestProduction);
            var result = _mesProductionProject.SaveRequestProduction(listSP, CurrentUser.UserID, ProdReqRequestType);
            return Json(result);          
        }
        [HttpPost]
        public IActionResult SaveSingleRequestProduction(string ProjectCode)
        {
            var result = _mesProductionProject.SaveSingleRequestProduction(ProjectCode, CurrentUser.UserID);
            return Json(result);
        }
        

    }
}

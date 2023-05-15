using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using InfrastructureCore.Models.Menu;

namespace Modules.Pleiger.Controllers
{
    public class MESDrawingController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IMESItemService _mESItemService;
        private readonly IGroupUserService groupUserService;
        private readonly IAccessMenuService accessMenuService;
        private readonly IPurchaseService _purchaseService;
        private readonly IUserService _userService;

        public MESDrawingController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, 
            IGroupUserService groupUserService, IMESComCodeService mesComCodeService,
            IMESSaleProjectService mesSaleProjectService, IMESItemService mESItemService, 
            IPurchaseService purchaseService, IUserService userService) : base(contextAccessor)
        {
            this._contextAccessor = contextAccessor;
            this._mesComCodeService = mesComCodeService;
            this._mesSaleProjectService = mesSaleProjectService;
            this._mESItemService = mESItemService;
            this.groupUserService = groupUserService;
            this.accessMenuService = accessMenuService;
            this._purchaseService = purchaseService;
            this._userService = userService;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.UserCode = CurrentUser.UserCode;
            ViewBag.SystemUserType = checkUserLogin.SystemUserType;

            return View();
        }

        [HttpGet]
        public IActionResult StaskDrawingCreatePopup(string projectCode, int menuid)
        {
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            var model = new MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.SystemUserType = checkUserLogin.SystemUserType;
            int menuidstrng = menuid;
            // Quan add 2020 / 08 / 18
            // Get User permission file Upload by GruopUser      
            //var ListPermission = CurrentUser.MenuAccessList.Where(m => m.MENU_ID == menuid).FirstOrDefault();
            var listSumFileUploadByMenuID = accessMenuService.SelectSumFileUploadByMenuID(menuidstrng, CurrentUser.UserID);
            if (projectCode != null)
            {
                
                model = _mesSaleProjectService.GetDataDetail(projectCode);
                model.ID = "FileID" + ViewBag.Thread;
                model.UrlPath = "";
                model.Pag_ID = projectCode;                
                model.FileMasterID = model.FileID;
                model.Pag_Name = "";
                model.Form_Name = " Drawing Upload File";
                model.Sp_Name = "UPLOAD_FILE_DRAWING";
                if (model.FileID == null || model.FileID == "")
                {
                    model.FileMasterID = Guid.NewGuid().ToString();
                    model.FileID = model.FileMasterID;
                }
                if (listSumFileUploadByMenuID.Count > 0)
                {
                    if (listSumFileUploadByMenuID[0].DELETE_FILE_SUM > 0)
                    {
                        model.Delele_File = true;
                    }
                    if (listSumFileUploadByMenuID[0].UPLOAD_FILE_SUM > 0)
                    {
                        model.Upload_File = true;
                    }
                }
            }
            return PartialView("DrawingCreatePopup", model);
        }
        [HttpGet]
        public IActionResult GetDetailByProjectCode(string projectCode)
        {
            var result = _mesSaleProjectService.GetDataDetail(projectCode);
            return Json(result);
        }
        //getItemByItemClassCode
        public object GetItemByItemClassCode(string itemClassCode, DataSourceLoadOptions loadOptions)
        {
            var data = _mESItemService.getItemsByItemClassCode(itemClassCode);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public object GetProjectStatus(DataSourceLoadOptions loadOptions)
        {
            var data = _mesSaleProjectService.GetProjectStatus();
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpGet]
        public IActionResult ShowPopupGenerateCode(string theadID)
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

        // Get list SaleProject
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions)
        {
            var data = _mesSaleProjectService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListProductType(string data)
        {
            var listdata = data;

            return Content(JsonConvert.SerializeObject(listdata), "applicatiofn/json");
        }

        [HttpGet]
        public object SearchSaleProjects(DataSourceLoadOptions loadOptions, MES_SaleProject model)
        {
            var result = _mesSaleProjectService.SearchSaleProject(model);
            //return DataSourceLoader.Load(result, loadOptions);
            return Json(result);
        }

        #endregion
        #region "Insert - Update - Delete
        [HttpPost]
        public IActionResult SaveSalesProject(string data)
        {
            MES_SaleProject a = JsonConvert.DeserializeObject<MES_SaleProject>(data);
            var result = _mesSaleProjectService.SaveSalesProject(a, CurrentUser.UserID);
            return Json(result);
        }

        [HttpPost]
        public IActionResult DeleteSalesProjects(string projectCode)
        {
            var result = _mesSaleProjectService.DeleteSalesProjects(projectCode);
            return Json(result);
        }

        #endregion


    }
}

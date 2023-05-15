using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using System;
using Modules.Pleiger.SalesProject.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.CommonModels;
using Modules.FileUpload.Services.IService;
using System.Collections.Generic;
using Modules.Common.Models;

namespace Modules.Pleiger.SalesProject.Controllers
{
    public class MESDrawingController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IMESItemService _mESItemService;
        private readonly IGroupUserService _groupUserService;
        private readonly IAccessMenuService _accessMenuService;
        private readonly ITaskDrawingService _taskDrawingService;
        private readonly IUserService _userService;
        private readonly IFileService _filesService;

        public MESDrawingController(IHttpContextAccessor contextAccessor,
            IAccessMenuService accessMenuService, 
            IGroupUserService groupUserService,
            IMESComCodeService mesComCodeService, 
            IMESSaleProjectService mesSaleProjectService,
            IMESItemService mESItemService,
            ITaskDrawingService taskDrawingService,
            IUserService userService,
            IFileService filesService
            ) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _mesComCodeService = mesComCodeService;
            _mesSaleProjectService = mesSaleProjectService;
            _mESItemService = mESItemService;
            _groupUserService = groupUserService;
            _accessMenuService = accessMenuService;
            _taskDrawingService = taskDrawingService;
            _userService = userService;
            _filesService = filesService;
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
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);////////
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
            var model = new  MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.SiteID = CurrentUser.SiteID;
            ViewBag.SystemUserType = checkUserLogin.SystemUserType;

            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuid, CurrentUser.UserCode);

            if (projectCode != null)
            {
                model = _mesSaleProjectService.GetDataDetail(projectCode);
                var files = _filesService.GetSYFileUploadMasterByID(model.FileID);
                files.Reverse();
                int count = files.Count;
                model.FileDetail = new List<SYFileUpload>();
                for (int i = 0; i < count; i++)
                {

                    var fileDetail = _filesService.GetSYFileUploadByID(files[i].FileGuid);

                    files[i].FileDetail = fileDetail;
                    fileDetail.FilePathShowBrowser = files[i].FilePath + "\\" + fileDetail.FileNameSave;
                    if (files[count - 1].FileDetail != null)
                    {
                        files[count - 1].FileDetail = fileDetail;
                        files[count - 1].FileDetail.No = 1;
                    }
                    else
                    {
                        files[i].FileDetail = fileDetail;
                        files[i].FileDetail.No = 0;
                    }
                    
                    model.FileDetail.Add(fileDetail);
                }
              
                //model.No = 1;
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
                // check user In Group Permission
                if (ListButtonPermissionByUser.Count > 0)
                {
                    model.Upload_File = ListButtonPermissionByUser[0].UPLOAD_FILE_YN;
                    model.Delele_File = ListButtonPermissionByUser[0].DELETE_FILE_YN;

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
            var result = _mesSaleProjectService.SearchSaleProject(model,null);
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

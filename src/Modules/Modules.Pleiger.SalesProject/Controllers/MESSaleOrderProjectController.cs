using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.FileUpload.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.SalesProject.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Pleiger.SalesProject.Controllers
{
    public class MESSaleOrderProjectController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUploadFileWithTemplateService _uploadFileWithTemplateService;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESItemService _mESItemService;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IUserService _userService;
        private readonly IMESSaleOrderProjectService _mESSaleOrder;
        private static string EXCEL_TEMPLATE_NAME = "TemplateSOP.xlsx";

        public MESSaleOrderProjectController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService,
            IUserService userService, IMESSaleOrderProjectService mESSaleOrder,
            IUploadFileWithTemplateService uploadFileWithTemplateService,
            IMESItemService mESItemService, IMESComCodeService mesComCodeService) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _accessMenuService = accessMenuService;
            _userService = userService;
            _mESSaleOrder = mESSaleOrder;
            _uploadFileWithTemplateService = uploadFileWithTemplateService;
            _mesComCodeService = mesComCodeService;
            _mESItemService = mESItemService;
        }

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");


            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.CREATE_YN = false;
            ViewBag.SEARCH_YN = false;
            ViewBag.EXCEL_YN  = false;
            // Phong 2021-12-09        
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.CREATE_YN             = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.SEARCH_YN             = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.EXCEL_YN              = ListButtonPermissionByUser[0].EXCEL_YN;
            }

            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.UserCode = CurrentUser.UserCode;
            return View();
        }
        [HttpGet]
        public object GetListSaleOrderProject(DataSourceLoadOptions loadOptions, MES_SalesOrderProjectNew model)
        {
            var result = _mESSaleOrder.GetListProjectOrder(model);
            return DataSourceLoader.Load(result, loadOptions);
        }
        public IActionResult GetOrderProjectType(DataSourceLoadOptions loadOptions, string GROUP_CD)
        {
            var result = _mESSaleOrder.GetProjectOrderType(GROUP_CD);
            var loadResult = DataSourceLoader.Load(result, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        public IActionResult SalesProjectCreatePopup(string projectCode, string viewbagIndex, int menuParent)
        {
            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = viewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;
            var saleProject = new MES_SalesOrderProjectNew();
            if (projectCode != null)
            {
                saleProject = _mESSaleOrder.GetDetailSaleOrderProject(projectCode);
                if (saleProject.EditDate == Convert.ToDateTime("1/1/1900 12:00:00 AM"))
                {
                    saleProject.EditDate = null;
                }
            }
            return PartialView("SalesOrderProjectCreatePopup", saleProject);
        }
        public IActionResult SalesProjectCreatePopupNew(string projectCode, string viewbagIndex, int menuParent)
        {
            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = viewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;
            var saleProject = new MES_SalesOrderProjectNew();
            if (projectCode != null)
            {
                saleProject = _mESSaleOrder.GetDetailSaleOrderProjectNew(projectCode);
                if (saleProject.EditDate == Convert.ToDateTime("1/1/1900 12:00:00 AM"))
                {
                    saleProject.EditDate = null;
                }
            }
            return PartialView("SalesOrderProjectCreatePopup", saleProject);
        }

        
        [HttpPost]
        public IActionResult SaveDataSaleProject(MES_SalesOrderProjectNew model)
        {
            string User_Id = CurrentUser.UserID;
            var result = _mESSaleOrder.SaveDataSaleProject(model, User_Id);
            return Json(result);
        }

        [HttpPost]
        public IActionResult InsertDataSaleProject(MES_SalesOrderProjectNew model)
        {
            if (model.Check == "Copy")
            {
                model.SalesOrderProjectCode = null;
            }
            string User_Id = CurrentUser.UserID;
            var result = _mESSaleOrder.SaveDataSaleProject(model, User_Id);
            return Json(result);
        }

        [HttpPost]
        public IActionResult DeleteSaleOrderProject(string SalesOrderProjectCode)
        {
            var result = _mESSaleOrder.DeleteSaleOrderProject(SalesOrderProjectCode);
            return Json(result);
        }

        [HttpGet]
        public IActionResult ShowPopupGetCustomer(string idParent)
        {
            var model = new MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.idParent = idParent;
            return PartialView("GetCustomerPopup", model);
        }
        [HttpGet]
        public IActionResult PopupExcelTemplateImport(string viewbagIndex)
        {
            var model = new MES_SalesOrderProjectNew();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = viewbagIndex;
            return PartialView("PopupImportExcel", model);
        }

        public IActionResult SaleOrderProjectDownloadFileTemplate()
        {
            string templateFilePath = Directory.GetCurrentDirectory() + "\\excelTemplate\\" + EXCEL_TEMPLATE_NAME;

            _uploadFileWithTemplateService.ImportDataToTemplateExcelFile("SALE_ORDER_PROJECT", templateFilePath);

            return Json(new { Result = true, downloadExcelPath = templateFilePath, fileName = EXCEL_TEMPLATE_NAME });
        }
        [HttpGet]
        public virtual ActionResult Download(string fileLink, string fileName)
        {
            if (fileName != null)
            {
                string Files = fileLink;
                byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
                System.IO.File.WriteAllBytes(Files, fileBytes);
                //MemoryStream ms = new MemoryStream(fileBytes);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        [HttpGet]
        public ActionResult InsertDataFromExcelSaleProject(string fileLoc)
        {

            string SPName = "SP_MES_SALE_ORDER_PROJECT_IMPORT_EXCEL";
            // dat modify 2021-1-17
            var result = _uploadFileWithTemplateService.SaveToDB_Model_Excel(fileLoc, SPName, CurrentUser.UserID, "SaleOrderProject");
            return Json(result);
        }


        ////Huy Test
        public IActionResult Test()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");


            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;

            // Phong 2021-12-09        
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

    }
}



//$("#btnImportExcel_@ViewBag.Thread").on("click", function() {
//        $.ajax({
//    url: '@Url.Action("PopupItemImportExcelTemplate", "MESItemHandMade")',
//            type: "GET",
//            data:
//        {
//        viewbagIndex: '@ViewBag.Thread',
//            },
//            dataType: "html",
//            success: function(result) {

//            parent popup
//                $("#modalContent").removeClass("modal-xl");
//                $("#modalContent").html(result);
//                $("#modalContent").addClass("modal-md");
//                $('#modalControl').modal('show');
//            LoadingPage(0);
//        }
//    });
//})
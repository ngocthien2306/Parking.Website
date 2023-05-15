using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Utils;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.FileUpload.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.SalesProject.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Modules.Pleiger.SalesProject.Controllers
{
    //[Authorize]
    public class MESSaleProjectController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IMESItemService _mESItemService;
        private readonly IUserService _userService;
        private readonly IAccessMenuService _accessMenuService;
        private readonly IUploadFileWithTemplateService _uploadFileWithTemplateService;

        private static string EXCEL_TEMPLATE_NAME = "Batch_SaleProjectTemplate.xlsx";
        public MESSaleProjectController(IHttpContextAccessor contextAccessor,
            IAccessMenuService accessMenuService,
            IMESComCodeService mesComCodeService,
            IMESSaleProjectService mesSaleProjectService,
            IMESItemService mESItemService,
            IUserService userService,
            IUploadFileWithTemplateService uploadFileWithTemplateService
            ) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _mesComCodeService = mesComCodeService;
            _mesSaleProjectService = mesSaleProjectService;
            _mESItemService = mESItemService;
            _userService = userService;
            _accessMenuService = accessMenuService;
            _uploadFileWithTemplateService = uploadFileWithTemplateService;
        }

        #region "Get Data"

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
             
            // Quan add 2021-02-25        
            var ListButtonPermissionByUser =  _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
            if(ListButtonPermissionByUser.Count>0)
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
        public IActionResult SalesProjectCreatePopup(string projectCode, string viewbagIndex)
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
        public IActionResult DeleteSalesProjects(string projectCode)
        {
            var result = _mesSaleProjectService.DeleteSalesProjects(projectCode);
            return Json(result);
        }

        #endregion

        #region SearchSaleProjects
        [HttpGet]
        public object SearchSaleProjects(DataSourceLoadOptions loadOptions, MES_SaleProject model ,string checkCode)
        {
            var result = _mesSaleProjectService.SearchSaleProject(model, checkCode);
            //return DataSourceLoader.Load(result, loadOptions);
            return Json(result);
        }
     
        [HttpGet]
        public object SearchSaleProjectsExcel(DataSourceLoadOptions loadOptions, MES_SaleProject model, string checkCode)
        {
            //var result = _mesSaleProjectService.SearchSaleProjectsExcel(model, checkCode);
            Stopwatch w2 = Stopwatch.StartNew();
            w2.Start();
            var result = _mesSaleProjectService.SearchSaleProjectsExcel1(model, checkCode);
            w2.Stop();
            return Json(new { Time = w2.ElapsedMilliseconds, Data = result});
        }
        #endregion

        // Quan add 
        [HttpPost]
        public IActionResult DownloadSalesInformation(string listSelected)
        {
            var result = _mesSaleProjectService.GetDataExportExcelICube(listSelected);

            string testJson = JsonConvert.SerializeObject(result);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(testJson);
            //dt.Columns.Remove("ProjectCode");
            //dt.Columns.Remove("VatType");
            Type myType = typeof(MES_SaleProjectExcelInfor);

            MemberInfo[] myMembers = myType.GetMembers();
            for (int i = 0; i < myMembers.Length; i++)
            {
                if (myMembers[i].CustomAttributes.Any() == true)
                {
                    foreach (var item in myMembers[i].CustomAttributes)
                    {
                        if (item.AttributeType.Name == "ColumNameAttribute")
                        {
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.ColumnName == myMembers[i].Name)
                                {
                                    col.Caption = item.ConstructorArguments[0].Value.ToString();
                                    //col.ColumnName = item.ConstructorArguments[0].Value.ToString();
                                }
                            }
                        }
                    }
                }
            }

            //string downloadExcelPath = _mesInventoryService.ExportInventoryExcelFile(dt);
            string downloadExcelPath = _mesSaleProjectService.ExportExcelICube(dt);

            var memory = new MemoryStream();
            using (var stream = new FileStream(downloadExcelPath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
            var file = File(memory, ExcelExport.GetContentType(downloadExcelPath), downloadExcelPath.Remove(0, downloadExcelPath.LastIndexOf("/") + 1));
            return Json(new { Result = true, downloadExcelPath = downloadExcelPath, fileName = file.FileDownloadName });
        }

        [HttpGet]
        public virtual ActionResult Download(string fileLink, string fileName)
        {
            if (fileName != null)
            {
                string Files = fileLink;
                byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
                System.IO.File.WriteAllBytes(Files, fileBytes);
                MemoryStream ms = new MemoryStream(fileBytes);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        [HttpGet]
        public IActionResult PopupExcelImportTemplate(string viewbagIndex)
        {
            var model = new MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.Index = viewbagIndex;
            return PartialView("PopupImportExcelTemplate", model);
        }

        public IActionResult SaleProjectDownloadFileTemplateImportExcel()
        {
            string templateFilePath = Directory.GetCurrentDirectory() + "\\excelTemplate\\" + EXCEL_TEMPLATE_NAME;

            _uploadFileWithTemplateService.ImportDataToTemplateExcelFile("SALE_PROJECT", templateFilePath);

            //templateFilePath= đường dẫn file
            //EXCEL_TEMPLATE_NAME= tên file
            return Json(new { Result = true, downloadExcelPath = templateFilePath, fileName = EXCEL_TEMPLATE_NAME });
        }

        [HttpPost]
        public ActionResult UploadFilePopup(IFormFile myFile, string chunkMetadata)
        {
            try
            {
                Type myType = Type.GetType("Modules.Pleiger.CommonModels.MES_SaleProject, Modules.Pleiger.CommonModels");

                var result = _uploadFileWithTemplateService.UploadFileDynamicForImportFromPopup(myFile, chunkMetadata, myType);

                return Json(new { result = true, data = result }); 
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("DateTime", StringComparison.Ordinal))
                {
                    Result result = new Result();
                    result.Success = false;
                    if (CurrentLanguages == "en")
                    {
                        result.Message = MessageCode.MD00014_EN;
                    }
                    else
                    {
                        result.Message = MessageCode.MD00014_KR;
                    }
                    return Json(result);
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        public ActionResult InsertDataFromExcelSaleProject(string fileLoc)
        {
            string SPName = "SP_MES_SALE_PROJECT_IMPORT_EXCEL";
            var result = _uploadFileWithTemplateService.SaveToDB_Model_Excel(fileLoc, SPName, CurrentUser.UserID, "SaleProject");
            return Json(result);
        }

        // Quan add 2021-02-23
        // showPopupGetItem
        [HttpGet]
        public IActionResult showPopupGetItem(string idParent)
        {
            var model = new MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.idParent = idParent;
            return PartialView("PopupGetItem", model);
        }

        // Quan add 2021-02-23
        // showPopupGetCustomer
        [HttpGet]
        public IActionResult showPopupGetCustomer(string idParent)
        {
            var model = new MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.idParent = idParent;
            return PartialView("PopupGetCustomer", model);
        }

        [HttpGet]
        public IActionResult Check(DataSourceLoadOptions loadOptions)
        {
            return Json(1);
        }
    }
}

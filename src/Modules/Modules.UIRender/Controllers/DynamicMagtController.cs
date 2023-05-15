using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.DataAccess;
using InfrastructureCore.Utils;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Common.Utils;
using Modules.UIRender.Services.IService;
using Modules.UIRender.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.RenderPage.Controllers
{
    public class DynamicMagtController : BaseController
    {
        #region Properties
        private IDynamicPageService _dynamicPageService;
        private readonly IHttpContextAccessor _contextAccessor;
        //duy add
        private readonly IUserService _userService;
        private readonly IAccessMenuService _accessMenuService;

        #endregion

        #region Constructor
        public DynamicMagtController(IDynamicPageService dynamicPageService, IHttpContextAccessor contextAccessor, IUserService userService, IAccessMenuService accessMenuService) : base(contextAccessor)
        {
            this._dynamicPageService = dynamicPageService;
            this._contextAccessor = contextAccessor;
            //duy add
            _userService = userService;
            this._accessMenuService = accessMenuService;

        }
        #endregion

        #region Page dynamic
        public IActionResult Index(int PageID, string MenuID)
        {
            //duy
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            //Type thisType = this.GetType();
            //MethodInfo theMethod = thisType.GetMethod(TheCommandString);
            //theMethod.Invoke(this, userParameters); 
            ViewBag.Thread = Guid.NewGuid().ToString("N");

            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
          
            #region Get permission button DynaicPgae
            ViewBag.Edit       =   false;
            ViewBag.Delete     =   false;
            ViewBag.Save       =   false;
            ViewBag.Search     =   false;
            ViewBag.Create     =   false;
            ViewBag.Excel      =   false;
            ViewBag.Print      =   false;
            ViewBag.Uploadfile =   false;
            ViewBag.Deletefile =   false;
   
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, curMenu.MenuID, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
        
                ViewBag.Edit        = ListButtonPermissionByUser[0].EDIT_YN;
                ViewBag.Delete      = ListButtonPermissionByUser[0].DELETE_YN;
                ViewBag.Save        = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.Search      = ListButtonPermissionByUser[0].SEARCH_YN;
                ViewBag.Create      = ListButtonPermissionByUser[0].CREATE_YN;
                ViewBag.Excel       = ListButtonPermissionByUser[0].EXCEL_YN;
                ViewBag.Print       = ListButtonPermissionByUser[0].PRINT_YN;
                ViewBag.Uploadfile  = ListButtonPermissionByUser[0].UPLOAD_FILE_YN;
                ViewBag.Deletefile  = ListButtonPermissionByUser[0].DELETE_FILE_YN;
            }
            
            #endregion

            // Quan add 2020/10/05        
            ViewBag.GridMenuID = MenuID;
            List<SYPageLayout> lstPage = new List<SYPageLayout>();
            ViewBag.PageID = PageID;
            int countPop = 0;
            int countPopCustomView = 0;
            List<string> pagepopupCustom = new List<string>();
            if (PageID != 0)
            {
                var page = _dynamicPageService.GetInfoPage(PageID);             
                lstPage.Add(page);

                // find page custom in page element
                var a = page.listPageElement;
                foreach (var item in page.listPageElement)
                {
                    if (item.CUSTM_VIEW != null && item.PEL_TYP == "G002C018")
                    {
                        countPopCustomView++;
                        pagepopupCustom.Add(item.CUSTM_VIEW);
                    }
                }

                List<SYPageRelationship> lstRelationship = new List<SYPageRelationship>();
                lstRelationship = _dynamicPageService.GetRelationship(PageID);
                // render popup dynamic
                if (lstRelationship != null)
                {
                    foreach (var item in lstRelationship)
                    {
                        var pagePop = _dynamicPageService.GetInfoPage(item.POP_ID);
                        if (pagePop.PAG_TYPE == "G001C002")
                        {
                            countPop++;
                        }
                        //else if (pagePop.PAG_TYPE == "G001C004")
                        //{
                        //    countPopCustomView++;
                        //}
                        lstPage.Add(pagePop);
                    }
                }
            }
            ViewBag.CountPop = countPop;
            ViewBag.CountPopCustomView = countPopCustomView;
            ViewBag.PagePopupCustom = pagepopupCustom;


            return View(lstPage);
        }


        public IActionResult JavascriptFromViewString(int PageID, string MenuID)
        {
            //Controller controller = this;
            //var jsFile = _dynamicPageService.GetJSPath(PageID);
            //const string JsPattern = @"^]*>(.*)$";
            //var content = Regex.Replace(jsFile, JsPattern, "$1", RegexOptions.Singleline);

            //return controller.Content(content, "application/javascript; charset=utf-8");
            var jsFile = _dynamicPageService.GetJSPath(PageID);

            return Content(jsFile);
        }

        #region 

        public async Task<ActionResult> MyJavaScriptAction()
        {
            return await this.GetPlainJavaScriptAsync("MyJavaScriptView");
        }

        #endregion

        public ActionResult GetPageElementsWithPELID(string pelID)
        {
            SYPageLayElements rs = _dynamicPageService.GetPageElementsWithPELID(pelID);
            return Json(rs);
        }
        #endregion

        #region Process call SP
        [HttpPost]
        public object CallSPSelect(DataSourceLoadOptions loadOptions, List<SPParameter> lstParam, string spname, string connectionType, string menuObject)
        {
            try
            {
                int pageSize = loadOptions.Take == 0 ? 100 : loadOptions.Take; // default pageSize = @PageSize = 100
                int itemSkip = loadOptions.Skip == 0 ? 0 : loadOptions.Skip;

                //duy add
                var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
                // Search from Date to Date
                if (lstParam.AsEnumerable().Where(m => m.Key == "P_DateFromTo").Select(a => a.Value).FirstOrDefault() != null)
                {
                    string date = lstParam.AsEnumerable().Where(m => m.Key == "P_DateFromTo").Select(a => a.Value).FirstOrDefault();
                    string[] DateStartEnd = date.Split(',');
                    lstParam.Add(new SPParameter { Key = "StartDate", Value = DateStartEnd[0] });
                    lstParam.Add(new SPParameter { Key = "EndDate", Value = DateStartEnd[1] });
                }
                // Auth.. Partner - Pleiger - spAdmin
                //if (CurrentUser != null && CurrentUser.UserType == "G000C003")
                //{
                //    lstParam.Add(new SPParameter { Key = "UserCode", Value = CurrentUser.UserCode });
                //}
                lstParam.Add(new SPParameter { Key = "PageNumber", Value = itemSkip.ToString() });
                lstParam.Add(new SPParameter { Key = "PageSize", Value = pageSize.ToString() });

                // Quan add permissions Partner get data
                // if (CurrentUser != null && CurrentUser.UserType != "G000C001" && CurrentUser.UserType != "G000C002" && CurrentUser.UserType != "G000C004")

                //duy close
                //if (CurrentUser != null && CurrentUser.UserType != "G000C001" && CurrentUser.UserType != "G000C002" && CurrentUser.UserType == "G000C003")
                //{
                //    lstParam.Add(new SPParameter { Key = "Userlogin", Value = CurrentUser.UserCode });
                //}
                //else
                //{
                //    lstParam.Add(new SPParameter { Key = "Userlogin", Value = null });

                //}

                //duy add
                // If UserType 
                // G000C001 SuperAdmin,
                // G000C002 Admin,
                // G000C003 Pleiger User
                // Set Userlogin = null

                if (CurrentUser != null && CurrentUser.UserType != "G000C001" && CurrentUser.UserType != "G000C002" && CurrentUser.UserType != "G000C003")
                {
                    lstParam.Add(new SPParameter { Key = "Userlogin", Value = CurrentUser.UserCode });
                }
                else
                {
                    lstParam.Add(new SPParameter { Key = "Userlogin", Value = null });
                }

                //get data
                dynamic data = _dynamicPageService.ExecuteProc2(spname, lstParam, connectionType);
                int totalCount = _dynamicPageService.ExecuteProc2Count(spname, lstParam, connectionType);

                //if (loadOptions.Sort != null)
                //{
                //    var dt = data.AsQueryable();
                //    dt = Helpers.OrderBy(dt, loadOptions.Sort[0].Selector, loadOptions.Sort[0].Desc);
                //
                //    return Json(new { data = dt, totalCount = totalCount });
                //}

                // passing totalCount to Json for handle load more data with loadOptions
                
                //return DataSourceLoader.Load(data, loadOptions);
                return Json(new { data = data, totalCount = totalCount });
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message });
            }
        }

        [HttpGet]
        public ActionResult GetEditTypeGrid(string pelID, string pagID)
        {
            try
            {
                List<SPParameter> lstParam = new List<SPParameter>();
                lstParam.Add(new SPParameter{ Key = "PEL_ID",  Value = pelID });
                lstParam.Add(new SPParameter{ Key = "PAG_ID",  Value = pagID });
                string connectionType = "G013C000";
                string spname = "GET_EDIT_TYPE_DYNAMIC";

                dynamic result = _dynamicPageService.ExecuteProc2(spname, lstParam, connectionType);
                //int totalCount = _dynamicPageService.ExecuteProc2Count(spname, lstParam, connectionType); ; // passing totalCount to Json for handle load more data with loadOptions
                return Content(JsonConvert.SerializeObject(result));
                //return Json(new { data = data, totalCount = totalCount });
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message });
            }
        }

        [HttpGet]
        public JsonResult CallSPSelectGet(DataSourceLoadOptions loadOptions, List<SPParameter> lstParam, string spname, string connectionType)
        {
            try
            {
                int pageSize = loadOptions.Take == 0 ? 100 : loadOptions.Take; // default pageSize = @PageSize = 100
                int itemSkip = loadOptions.Skip == 0 ? 0 : loadOptions.Skip;

                lstParam.Add(new SPParameter { Key = "PageNumber", Value = itemSkip.ToString() });
                lstParam.Add(new SPParameter { Key = "PageSize", Value = pageSize.ToString() });

                // get data
                dynamic data = _dynamicPageService.ExecuteProc2(spname, lstParam, connectionType);
                //int totalCount = _dynamicPageService.ExecuteProc2Count(spname, lstParam, connectionType); ; // passing totalCount to Json for handle load more data with loadOptions
                return DataSourceLoader.Load(data, loadOptions);
                //return Json(new { data = data, totalCount = totalCount });
            }
            catch (Exception e)
            {
                return Json(new { data = e.Message });
            }
        }

        //[HttpPost]
        //public JsonResult CallSPSelect(DataSourceLoadOptions loadOptions,List<SPParameter> lstParam, string spname, string connectionType)
        //{
        //    try
        //    {
        //        int pageSize = loadOptions.Take == 0 ? 100 : loadOptions.Take; // default pageSize = @PageSize = 100
        //        int itemSkip = loadOptions.Skip == 0 ? 0 : loadOptions.Skip;

        //        lstParam.Add(new SPParameter{ Key = "PageNumber", Value= itemSkip.ToString() });
        //        lstParam.Add(new SPParameter{ Key = "PageSize", Value= pageSize.ToString() });

        //        // get data
        //        dynamic data = _dynamicPageService.ExecuteProc2(spname, lstParam, connectionType);
        //        int totalCount = _dynamicPageService.ExecuteProc2Count(spname, lstParam, connectionType); ; // passing totalCount to Json for handle load more data with loadOptions
        //        //return DataSourceLoader.Load(data, loadOptions, totalCount);
        //        return Json(new { data = data, totalCount = totalCount});
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { data = e.Message });
        //    }
        //}


        [HttpPost]
        public IActionResult CreateDataDynamic(List<SPParameter> lstParam, string spname, Dictionary<string, string> temp)
        {
            try
            {
                List<SPParameter> parameters = new List<SPParameter>();
                dynamic data = _dynamicPageService.ExecuteProCRUD(spname, lstParam);
                return Json(new { data = data });
            }
            catch (Exception)
            {
                return Json(new { data = new List<string>() });
            }
        }

        [HttpPost]
        public IActionResult BatchPost(string objPostData)
        {
            dynamic stuff = JsonConvert.DeserializeObject(objPostData);

            //var result = FXLayoutManager.SavePostData(objPostData);
            var result = _dynamicPageService.ExecuteSave(objPostData, CurrentUser);
            return Json(result);
        }
        #endregion

        #region Init Grid view
        [HttpPost]
        public IActionResult InitGridDynamic(string pelID, int pagID)
        {
            var result = _dynamicPageService.InitModelGridDynamic(pelID, pagID);
            return Json(result);
        }

        [HttpPost]
        public IActionResult InitModelGridDynamic(List<SPParameter> lstParam, string spname, Dictionary<string, string> temp)
        {
            try
            {
                List<SPParameter> parameters = new List<SPParameter>();
                // parameters = parms.Select(x => new SPParameter { Key = $"{x.Key}", Value = x.Value }).ToList();
                dynamic data = _dynamicPageService.ExecuteProc2(spname, lstParam, "");
                return Json(new { data = data });
            }
            catch (Exception)
            {
                return Json(new { data = new List<string>() });
            }
        }

        public IActionResult UpdateGrid(int key, string values)
        {
            return Ok();
        }
        #endregion

        #region Page Template
        public IActionResult PageViewTemplate(int PageID)
        {
            List<SYPageLayout> lstPage = new List<SYPageLayout>();
            ViewBag.PageID = PageID;
            if (PageID != 0)
            {
                var page = _dynamicPageService.GetInfoPage(PageID);
                lstPage.Add(page);
                List<SYPageRelationship> lstRelationship = new List<SYPageRelationship>();
                lstRelationship = _dynamicPageService.GetRelationship(PageID);
                foreach (var item in lstRelationship)
                {
                    var pagePop = _dynamicPageService.GetInfoPage(item.POP_ID);

                    lstPage.Add(pagePop);
                }
            }
            return View(lstPage);
        }
        #endregion

        #region "Generate Popup"

        // Generate Popup in Layout
        public IActionResult GeneratePopup(int pageID)
        {
            var model = _dynamicPageService.GetInfoPage(pageID);
            // render popup dynamic mapping
            if (model != null && model.PAG_TYPE == "G001C001")
            {
                return View("_RenderPopup", model);
            }
            //// render popup custom
            //else if (model != null && model.PAG_TYPE == "G001C004")
            //{
            //    return View("_RenderPopupCustom", model);
            //}

            return null;
        }
        // Generate Popup in Layout
        public IActionResult GeneratePopupCustom(string viewPage)
        {
            // render popup dynamic mapping
            if (viewPage != null)
            {
                return View("_RenderPopupCustom", viewPage);
            }

            return null;
        }

        public IActionResult PopupTest()
        {
            return View();
        }
        #endregion

        #region Export Excel PVN

        public ActionResult ExportExcel(List<SPParameter> lstParam, string spname, string connectionType, string fileName)
        {
            DownloadFileUtils _downloadFileUtils = new DownloadFileUtils();

            DataTable dt = new DataTable();

            // Quan add 2021-03-29
            if (lstParam.AsEnumerable().Where(m => m.Key == "P_DateFromTo").Select(a => a.Value).FirstOrDefault() != null)
            {
                string date = lstParam.AsEnumerable().Where(m => m.Key == "P_DateFromTo").Select(a => a.Value).FirstOrDefault();
                string[] DateStartEnd = date.Split(',');
                lstParam.Add(new SPParameter { Key = "StartDate", Value = DateStartEnd[0] });
                lstParam.Add(new SPParameter { Key = "EndDate", Value = DateStartEnd[1] });
            }                  

            var getData = _dynamicPageService.ExecuteProc2(spname, lstParam, connectionType);
            string test = JsonConvert.SerializeObject(getData);
            dt = JsonConvert.DeserializeObject<DataTable>(test);
            
            var exportExcelPath = _dynamicPageService.ExportExcelDynamic(dt, fileName, spname);

            var fileStream = _downloadFileUtils.GetFileStreamResult(exportExcelPath);

            //var downloadFile = _downloadFileUtils.DownloadFile(exportExcelPath, fileStream.FileDownloadName);
            // Quan add 2021-03-29
            // Check if data is null
            if(fileStream!=null)
            {
                return Json(new { Result = true, downloadExcelPath = exportExcelPath, fileName = fileStream.FileDownloadName });
            }
            else
            {
                return Json(new { Result = false, downloadExcelPath = "", fileName = "" });

            }
        }

        [HttpGet]
        public virtual ActionResult Download(string fileLink, string fileName)
        {
            DownloadFileUtils _downloadFileUtils = new DownloadFileUtils();
            if (fileName != null)
            {
               return _downloadFileUtils.DownloadFile(fileLink, fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        #endregion

        #region Import Data Through Excel File Dynamic 
        [HttpGet]
        public IActionResult PopupUploadFile(int PageID, int PageAct, string PageKey, string MenuID)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");

            //ViewBag.MenuID = CurrentMenu.MenuID;
            ViewBag.MenuID = MenuID;

            var page = _dynamicPageService.GetMenuImportAction(PageID, PageAct, PageKey);
           
            return PartialView("_RenderPopupUploadFile", page);
        }

        [HttpPost]
        public ActionResult UploadFilePopup(IFormFile myFile, string chunkMetadata, int PageID, int ActID, string PageKey)
        {
            try
            {
                var dynamicJson = JsonUtil.ReadJsonOfDynamicPage();
                string SPName = "";
                Type myType = null;

                foreach (var dynaJson in dynamicJson)
                {
                    foreach (var item in dynaJson.DynamicTable)
                    {
                        if (PageKey == item.PageName)
                        {
                            //myType = Type.GetType("Modules.Pleiger.CommonModels.MES_BOM, Modules.Pleiger.CommonModels");
                            //Console.WriteLine(typeof(MES_BOM).AssemblyQualifiedName);
                            myType = Type.GetType(item.AssemblyName);
                            SPName = item.SPName;
                        }
                    }
                }

                var result = _dynamicPageService.UploadFileDynamicForImportFromPopup(myFile, chunkMetadata, myType);

                var page = _dynamicPageService.GetMenuImportAction(PageID, ActID, PageKey);

                page.FileLoc = result;

                return Json(new { result = true, data = page, fileID = "", massage = "" });
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

        public ActionResult InsertDataFromExcelDynamic(string fileLoc, int PageID, int ActID, string PageKey)
        {
            var dynamicJson = JsonUtil.ReadJsonOfDynamicPage();
            string SPName = "";

            foreach (var dynaJson in dynamicJson)
            {
                foreach (var item in dynaJson.DynamicTable)
                {
                    if (PageKey == item.PageName)
                    {
                        SPName = item.SPName;
                    }
                }
            }

            var lstParams = _dynamicPageService.GetListParam(ActID, PageID);
            var result = _dynamicPageService.SaveToDB_DynamicData_Excel(fileLoc, SPName, lstParams, CurrentUser.UserID);
            return Json(result);
        }

        
        #endregion

        #region EXCEL TEMPLATE DOWNLOAD
        [HttpGet]
        public ActionResult DownloadFileTemplateImportExcel(int PageID, int ActID, string PageKey)
        {
            try
            {
                var dynamicJson = JsonUtil.ReadJsonOfDynamicPage();
                string PageName = "";
                string templateName = "";
                foreach (var dynaJson in dynamicJson)
                {
                    foreach (var dynaTable in dynaJson.DynamicTable)
                    {
                        if (dynaTable.PageName == PageKey)
                        {
                            templateName = dynaTable.TemplateName;
                            PageName = PageKey;
                        }
                    }
                }
                string templateFilePath = Directory.GetCurrentDirectory() + "\\excelTemplate\\" + templateName;

                _dynamicPageService.ImportDataToTemplateExcelFile(PageName, templateFilePath);

                return Json(new { Result = true, downloadExcelPath = templateFilePath, fileName = templateName });
            }
            catch(Exception ex)
            {
                LogWriter logWriter = new LogWriter(ex.Message);

                return Json(new { Result = false, downloadExcelPath = "", fileName = "" });
            }
         
        }
        #endregion



    }
}

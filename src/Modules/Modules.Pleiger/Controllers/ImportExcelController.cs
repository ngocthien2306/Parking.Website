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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;
using InfrastructureCore.Helpers;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Internal;
using DocumentFormat.OpenXml.EMMA;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using ClosedXML.Excel;
using System.Data;
using InfrastructureCore.Extensions;

namespace Modules.Pleiger.Controllers
{
    public class ImportExcelController : BaseController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IMESItemService _mESItemService;
        private readonly IGroupUserService groupUserService;
        private readonly IAccessMenuService accessMenuService;
        private readonly IEmployeeService _employeeService;
        //private readonly IImportExcelService _mportExcelService;
        public ImportExcelController(IHttpContextAccessor contextAccessor,  IAccessMenuService accessMenuService, IEmployeeService employeeService, IGroupUserService groupUserService, IMESComCodeService mesComCodeService, IMESSaleProjectService mesSaleProjectService, IMESItemService mESItemService) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _mesComCodeService = mesComCodeService;
            _mesSaleProjectService = mesSaleProjectService;
            _mESItemService = mESItemService;
            this.groupUserService = groupUserService;
            this.accessMenuService = accessMenuService;
            this._employeeService = employeeService;
            //_mportExcelService = mportExcelService;
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
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;

            return View();
        }

        [HttpGet]
        public IActionResult StaskDrawingCreatePopup(string projectCode, int menuid)
        {
            var model = new MES_SaleProject();
            ViewBag.Thread = Guid.NewGuid().ToString("N");       
            // Quan add 2020 / 08 / 18
            // Get User permission file Upload by GruopUser      
            //var ListPermission = CurrentUser.MenuAccessList.Where(m => m.MENU_ID == menuid).FirstOrDefault();
            var listSumFileUploadByMenuID = accessMenuService.SelectSumFileUploadByMenuID(menuid, CurrentUser.UserID);
            //var ListPermissionbyGroupMenuID = CurrentUser.listSumDelUploadByMenuID.Where(m => m.MENU_ID == menuid).FirstOrDefault();
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
        public IActionResult ReadExceclFile(string UrlFile)
        {
            try
            {
                //Lets open the existing excel file and read through its content . Open the excel using openxml sdk
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(@"C:\Users\Tuan\Desktop\Raw_Data_20200803\User_Info.XLSX", false))
                {
                    //create the object for workbook part  
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();
                    StringBuilder excelResult = new StringBuilder();

                    //using for each loop to get the sheet from the sheetcollection  
                    foreach (Sheet thesheet in thesheetcollection)
                    {
                        excelResult.AppendLine("Excel Sheet Name : " + thesheet.Name);
                        excelResult.AppendLine("----------------------------------------------- ");
                        //statement to get the worksheet object by using the sheet id  
                        Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;

                        SheetData thesheetdata = (SheetData)theWorksheet.GetFirstChild<SheetData>();
                        foreach (Row thecurrentrow in thesheetdata)
                        {
                            foreach (Cell thecurrentcell in thecurrentrow)
                            {
                                //statement to take the integer value  
                                string currentcellvalue = string.Empty;
                                if (thecurrentcell.DataType != null)
                                {
                                    if (thecurrentcell.DataType == CellValues.SharedString)
                                    {
                                        int id;
                                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                                        {
                                            SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                                            if (item.Text != null)
                                            {
                                                //code to take the string value  
                                                excelResult.Append(item.Text.Text + " ");
                                            }
                                            else if (item.InnerText != null)
                                            {
                                                currentcellvalue = item.InnerText;
                                            }
                                            else if (item.InnerXml != null)
                                            {
                                                currentcellvalue = item.InnerXml;
                                            }
                                            return new EmptyResult();
                                        }
                                    }
                                }
                                else
                                {
                                    excelResult.Append(Convert.ToInt16(thecurrentcell.InnerText) + " ");
                                }
                            }
                            excelResult.AppendLine();
                        }
                        excelResult.Append("");
                        Console.WriteLine(excelResult.ToString());
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception)
            {

            }
            return new EmptyResult();

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
        [HttpGet]
        public IActionResult ReadlFile(string UrlFile, bool header)
        {

            string path = @"C:\Users\Tuan\Desktop\Raw_Data_20200803\User_Info.XLSX";
            //string path = UrlFile;
            ExcelHelperTest excelHelperTest = new ExcelHelperTest();
            // Get data from excecl to DataTable
            var dt = excelHelperTest.ReadFromExcelfile(path, "");
            // Add DataTable to Dataset
            DataSet dts = new DataSet();
            dts.Tables.Add(dt);

            // Convert DataSetType to ModelType
            var results = dts.parseCellDataByTypefromExcel<MESEmployees>();
            return Json(results);       


        }        
    }
}

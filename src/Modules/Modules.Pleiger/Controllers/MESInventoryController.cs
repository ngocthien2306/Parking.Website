using ClosedXML;
using DevExpress.Data.Extensions;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Extensions;
using InfrastructureCore.Utils;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Modules.Pleiger.Controllers
{
    public class MESInventoryController : BaseController
    {
        #region variables
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESInventoryService _mesInventoryService;
        private readonly IAccessMenuService accessMenuService;

        #endregion

        public MESInventoryController(IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IMESInventoryService mesInventoryService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._mesInventoryService = mesInventoryService;
            this.accessMenuService = accessMenuService;

        }

        public IActionResult Index()
        {
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            var SelectUserPermissionAccessMenu = accessMenuService.SelectUserPermissionAccessMenu(menuID, CurrentUser.UserID);
            if(SelectUserPermissionAccessMenu.Count > 0)
            {
                ViewBag.InventoryYN = SelectUserPermissionAccessMenu[0].INVENTORY_YN;
            }
            else
            {
                ViewBag.InventoryYN = false;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.UserType = CurrentUser.UserType;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            return View();
        }
        //Inventory History
        public IActionResult InventoryHistory()
        {
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.UserType = CurrentUser.UserType;
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            return View("InventoryHistory");
        }

        public IActionResult GetWareHouseByCategory(DataSourceLoadOptions loadOptions, string CategoryCode)
        {
            var result = _mesInventoryService.GetWareHouseByCategory(CategoryCode);
            return Json(result);
        }

        #region Get data trans closing master
        [HttpGet]
        public object GetTransClosingMst(DataSourceLoadOptions loadOptions, string startDate, string endDate)
        {
            var data = _mesInventoryService.GetTransClosingMst(startDate, endDate);
            return DataSourceLoader.Load(data, loadOptions);
        }
        #endregion

        #region Get data trans closing detail
        [HttpGet]
        public object GetTransClosingDtl(DataSourceLoadOptions loadOptions, string TransMonth)
        {
            var data = _mesInventoryService.GetTransClosingDtl(TransMonth);
            return DataSourceLoader.Load(data, loadOptions);
        }

        #endregion
        #region Get data trans closing items
        [HttpGet]
        public object GetTransClosingItems(DataSourceLoadOptions loadOptions, string TransCloseNo)
        {
            var data = _mesInventoryService.GetTransClosingItems(TransCloseNo);
            return DataSourceLoader.Load(data, loadOptions);
        }
        public object GetTransClosingMstSearch(DataSourceLoadOptions loadOptions, 
        string startDate , string endDate  , string Category, string  ItemClass, string ItemCode, string ItemName)
        {
            var data = _mesInventoryService.GetTransClosingMstSearch(startDate, endDate, Category, ItemClass, ItemCode, ItemName);
            return DataSourceLoader.Load(data, loadOptions);
        }

        #endregion

        #region Duy Get ClosingDetail from ClosingItem 
        //duy 
        public object GetTransClosingDtlItemsGetDetail(DataSourceLoadOptions loadOptions, string TransCloseNo)
        {
            var data = _mesInventoryService.GetTransClosingDtlTransCloseNo(TransCloseNo);
            return DataSourceLoader.Load(data, loadOptions);
        }
        #endregion

        #region Duy Get ClosingDetail from ClosingDetail 
        //duy 
        public object GetTransClosingMstFrDetail(DataSourceLoadOptions loadOptions, string TransMonth)
        {
            var data = _mesInventoryService.GetTransClosingMstTransMonth(TransMonth);
            return DataSourceLoader.Load(data, loadOptions);
        }
        #endregion
        #region Popup Download/Upload file Inventory check

        [HttpGet]
        public IActionResult InventoryCheckPopupDownloadExcel()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            return PartialView("InventoryQuantityDownload");
        }
        
        [HttpGet]
        public IActionResult IsTransmonthHaveInventoryClosed(string transMonth)
        {
            var result = _mesInventoryService.IsTransmonthHaveInventoryClosed(transMonth);
            return Json(result);
        }

        [HttpGet]
        public IActionResult InventoryCheckPopupImportExcel(string pageParentThread)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.pageThread = pageParentThread;
            
            return PartialView("InventoryQuantityUpload");
        }

        [HttpGet]
        public object GetInventoryCurrentStock(DataSourceLoadOptions loadOptions, string warehouseCode, string category,
            string itemCode, string itemName, DateTime? closeMonth)
        {
            var data = _mesInventoryService.GetInventoryCurrentStock(warehouseCode, category, itemCode, itemName, closeMonth);
            return DataSourceLoader.Load(data, loadOptions);
        }

        //PVN Add 2020-11-01
        //TODO: Export Excel Need discuss again
        [HttpGet]
        public object GetInventoryCurrentStockNew(DataSourceLoadOptions loadOptions, string warehouseCode/*warehouseType*/, string category,
            string itemCode, string itemName, DateTime? closeMonth, string Lang)
        {
            var data = _mesInventoryService.GetInventoryCurrentStockNew(warehouseCode, category, itemCode, itemName, closeMonth, Lang);
            return DataSourceLoader.Load(data, loadOptions);
        }

        [HttpPost]
        public IActionResult DownloadFileInventoryCurrentStock(string listSelected, string transMonth)
        {
            var log = new LogWriter("DownloadFileInventoryCurrentStock");
            log.LogWrite("DownloadFileInventoryCurrentStock Start");
            //List<MES_InventoryCheckVO> data = JsonConvert.DeserializeObject<List<MES_InventoryCheckVO>>(listSelected);
           
            var haveInventoryClosed = _mesInventoryService.IsTransmonthHaveInventoryClosed(transMonth);
            if (haveInventoryClosed)//have data
            {
                return Json(new { Result = false, Message = "Fail" });
            }
            else
            {
                var result = _mesInventoryService.GetItemInventoryByWHCodeAndItemCode(listSelected);
                //List<MES_InventoryCheckExcelVO> ExcelData = new List<MES_InventoryCheckExcelVO>();
                //int no = 1;
                //foreach (var d in data)
                //{
                //    ExcelData.Add(new MES_InventoryCheckExcelVO { 
                //        No = no,
                //        WHCode = d.WHCode,
                //        WHName = d.WHName,
                //        CategoryCode = d.CategoryCode,
                //        CategoryName = d.CategoryName,
                //        ItemCode = d.ItemCode,
                //        ItemName = d.ItemName,
                //        MESSystemQty = d.CurrentStockQty,
                //        OfflineCheckQty = d.CheckQty,
                //        Remark = d.Remark,
                //        StockDate = DateTime.Now,
                //        CheckDate = d.CheckDate
                //    });
                //    no++;
                //}
                
                string testJson = JsonConvert.SerializeObject(result);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(testJson);

                dt.Columns.Remove("CheckDate");

                Type myType = typeof(MES_InventoryCheckExcelVO);
                MemberInfo[] myMembers = myType.GetMembers();
                for (int i = 0; i < myMembers.Length; i++)
                {
                    if(myMembers[i].CustomAttributes.Any() == true)
                    {
                        foreach (var item in myMembers[i].CustomAttributes)
                        {
                            if(item.AttributeType.Name == "ColumNameAttribute")
                            {
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if(col.ColumnName == myMembers[i].Name)
                                    {
                                        col.Caption = item.ConstructorArguments[0].Value.ToString();
                                    }    
                                }
                            }    
                        }
                    }    
                }

                string downloadExcelPath = _mesInventoryService.ExportInventoryExcelFile(dt);

                var memory = new MemoryStream();
                using (var stream = new FileStream(downloadExcelPath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                var file = File(memory, ExcelExport.GetContentType(downloadExcelPath), downloadExcelPath.Remove(0, downloadExcelPath.LastIndexOf("/") + 1));
                return Json(new { Result = true, downloadExcelPath = downloadExcelPath, fileName = file.FileDownloadName });
            }
        }

        [HttpGet]
        public virtual ActionResult Download(string fileLink, string fileName)
        {
            if (fileName != null)
            {
                //byte[] data = TempData[fileGuid] as byte[];
                //return File(fileName, "application/vnd.ms-excel", fileName);
                string Files = fileLink;
                byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
                System.IO.File.WriteAllBytes(Files, fileBytes);
                MemoryStream ms = new MemoryStream(fileBytes);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }


        [HttpPost]
        public ActionResult UploadFileInventoryCurrentStock(IFormFile myFile, string chunkMetadata)
        {
            var rs = -1;
            // Removes temporary files
            // RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 5, 0));
            try
            {
                Type myType = typeof(MES_InventoryCheckExcelVO);
                var data = _mesInventoryService.UploadFileInventoryCurrentStock(myFile, chunkMetadata, myType, CurrentLanguages);
                return Json(data);
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("DateTime", StringComparison.Ordinal))
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

        [HttpPost]
        public IActionResult SaveInventoryCheck(string data, string transMonth, string detailRemark)
        {
            List<MES_InventoryCheckVO> dataUpload = JsonConvert.DeserializeObject<List<MES_InventoryCheckVO>>(data);
            var haveInventoryClosed = _mesInventoryService.IsTransmonthHaveInventoryClosed(transMonth);
            if (haveInventoryClosed)//have data
            {
                string messageFail = "This month " + transMonth.Substring(0,7) + " have inventory closed data. Excel cannot be exported, and cannot be uploaded.";
                return Json(new { Result = false, Message = messageFail });
            }
            else
            {
                var result = _mesInventoryService.SaveInventoryCheck(dataUpload, CurrentUser.UserID, detailRemark);
                return Json(result);
            }
        }
        #endregion

        #region Close/Unclose Month
        // Save Data Close
        [HttpPost]
        public IActionResult CloseMonth(MES_TransClosingMst selectedRowsData)
        {
            //MES_TransClosingMst item = JsonConvert.DeserializeObject<MES_TransClosingMst>(selectedRowsData);
            var result = _mesInventoryService.CloseMonth(selectedRowsData, CurrentUser.UserID);
            return Json(result);
        }

        // Save Data UnClose
        [HttpPost]
        public IActionResult UnCloseMonth(List<MES_TransClosingMst> data)
        {
            //MES_TransClosingMst item = JsonConvert.DeserializeObject<MES_TransClosingMst>(selectedRowsData);
            var result = _mesInventoryService.UnCloseMonth(data, CurrentUser.UserID);
            return Json(result) ;
        }
        #endregion
    }
}

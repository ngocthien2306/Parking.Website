using ClosedXML.Excel;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.FileUpload.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using LazZiya.ExpressLocalization;
using InfrastructureCore.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Linq.Expressions;
using Modules.Common.Utils;
/// <summary>
/// Create User: Minh Vu
/// Create Day: 2020-09-10
/// Wharehouse Inventory Status (kor: 창고별자재현황)
/// </summary>
namespace Modules.Pleiger.Controllers
{
    [Authorize]
    public class MESWHInventoryStatusController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private IMESPartnerService _partnerService;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IMESWHInventoryStatusService _mesWHInventoryStatusService;

        public MESWHInventoryStatusController(IHttpContextAccessor contextAccessor, IMESPartnerService partnerService
            , IMESComCodeService mesComCodeService, IMESWHInventoryStatusService mesWHInventoryStatusService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._partnerService = partnerService;
            this._mesComCodeService = mesComCodeService;
            this._mesWHInventoryStatusService = mesWHInventoryStatusService;
        }
        
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

        #region Get Data

        [HttpGet]
        public object GetAllWHInventory(DataSourceLoadOptions loadOptions, MES_WHInventoryStatus model)
        {
            int pageSize = loadOptions.Take == 0 ? 100 : loadOptions.Take; // default pageSize = @PageSize = 100
            int itemSkip = loadOptions.Skip == 0 ? 0 : loadOptions.Skip;

            //var data = _mesWHInventoryStatusService.GetAllWHInventory(model, pageSize, itemSkip);
            var data = _mesWHInventoryStatusService.GetAllWHInventory(model, pageSize, itemSkip);
            var count = _mesWHInventoryStatusService.CountAllWHInventory(model);

            if (loadOptions.Sort != null)
            {
                var dt = data.AsQueryable();
                dt = Helpers.OrderBy(dt, loadOptions.Sort[0].Selector, loadOptions.Sort[0].Desc);

                return Json(new { data = dt, totalCount = count });
            }    

            //var dsLoader = DataSourceLoader.Load(data, loadOptions);
            //dsLoader.totalCount = data.Count == 0 ? 0 : 60000;

            //var result = new JsonResult(dsLoader);
             //return result;
             //return DataSourceLoader.Load(data, dsLoader);
            //return DataSourceLoader.Load(data, loadOptions);
            return Json(new { data = data, totalCount = count });
        }

        [HttpGet]
        public object GetAllWHInventoryTest(DataSourceLoadOptions loadOptions)
        {
            int pageSize = loadOptions.Take == 0 ? 100 : loadOptions.Take; // default pageSize = @PageSize = 100
            int itemSkip = loadOptions.Skip == 0 ? 0 : loadOptions.Skip;

            var data = _mesWHInventoryStatusService.GetAllWHInventory(new MES_WHInventoryStatus(), 0, 0);
            return DataSourceLoader.Load(data, loadOptions);
        }
        #endregion
        #region ExportExcel-Server
        public DataTable GetAllDataWHItemStock(string WarehouseCode,string Category, string ItemName)
        {
            DataTable dt = new DataTable();
            dt.TableName = "WHInventoryData";
            //Add Columns  
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("WarehouseCode", typeof(string));
            dt.Columns.Add("WarehouseName", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("StockQty", typeof(int));
            //Add Rows in DataTable  
            var listData = _mesWHInventoryStatusService.GetAllWHInventoryToExport(WarehouseCode, Category, ItemName);
            foreach (var item in listData)
            {
                dt.Rows.Add(item.NO,item.WarehouseCode,item.WarehouseName
                    ,item.ItemCode,item.ItemName,item.Category,item.StockQty);
            }
            dt.AcceptChanges();
            return dt;
        }


        [HttpGet]
        [Authorize]
        public IActionResult ExportExcelServer(string WarehouseCode ,string Category,string ItemName)
        {
            DataTable dt = GetAllDataWHItemStock(WarehouseCode, Category, ItemName);
            string fileName = "WareHouseInventory.xlsx";
            //using (XLWorkbook wb = new XLWorkbook())
            //{
            //    var ws = wb.Worksheets.Add(dt);
            //    ws.Columns("A","E").AdjustToContents();
            //    using (var  stream = new MemoryStream())
            //    {
            //        wb.SaveAs(stream);
            //        stream.Position = 0;
            //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            //    }
            //}


            string downloadExcelPath = _mesWHInventoryStatusService.ExportWHInventoryExcelFile(dt);

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
        #endregion
    }
}

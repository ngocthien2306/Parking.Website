using InfrastructureCore.Utils;
using InfrastructureCore.Web.Controllers;
using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Pleiger.Production.Controllers
{
    public class MESProductionResultController : BaseController
    {
        private readonly IMESProductionResultService _mESProductionResultService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ISharedCultureLocalizer _loc;

        public MESProductionResultController(IMESProductionResultService mESProductionResultService,
            IHttpContextAccessor contextAccessor, ISharedCultureLocalizer loc) : base(contextAccessor)
        {
            this._mESProductionResultService = mESProductionResultService;
            this._contextAccessor = contextAccessor;
            this._loc = loc;
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

            return View();
        }

        [HttpGet]
        public IActionResult SearchProjectProductionResult(string UserProjectCode, string ProductionCode, string ItemCode, string ItemName, string ProjectStatus, string SalesClasification, string ProjectOrderType, string SalesOrderProjectName, string SalesOrderProjectCode)
        {
            var result = _mESProductionResultService.searchProductionResult(UserProjectCode, ProductionCode, ItemCode, ItemName, ProjectStatus, SalesClasification,  ProjectOrderType,  SalesOrderProjectName, SalesOrderProjectCode);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpGet]
        public IActionResult GetProductionLineResult(string ProjectCode, string ProductionCode)
        {
            var result = _mESProductionResultService.getProductionLineResult(ProjectCode, ProductionCode);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }
        [HttpGet]
        public IActionResult GetProductionLineResultDetail(string ProjectCode, string GroupLine)
        {
            var result = _mESProductionResultService.getProductionLineResultDetail(ProjectCode, GroupLine);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }
        [HttpGet]
        public IActionResult GetWorkHistory(string ProjectCode, string ProductionCode, string ProdcnLineCode)
        {
            var result = _mESProductionResultService.getWorkHistory(ProjectCode, ProductionCode, ProdcnLineCode);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        [HttpGet]
        public IActionResult GetDeliveryInformation(string ProjectCode)
        {
            var result = _mESProductionResultService.getDeliveryInformation(ProjectCode);
            var data = JsonConvert.SerializeObject(result);
            return Content(data, "application/json");
        }

        #region ExportExcel-Server
        private DataTable GetExportExcelData(string UserProjectCode, string ProductionCode, string ItemCode, string ItemName, string ProjectStatus, string SalesClasification, string ProjectOrderType, string SalesOrderProjectName,string SalesOrderProjectCode)
        {
            DataTable dt = new DataTable();
            dt.TableName = "ProductionResult";
            //Add Columns  
            dt.Columns.Add("No", typeof(int));
            dt.Columns.Add("User Project Code", typeof(string));
            dt.Columns.Add("Project Name", typeof(string));
            dt.Columns.Add("Sales Classification", typeof(string));
            dt.Columns.Add("Project Order Type", typeof(string));
            dt.Columns.Add("Project Status", typeof(string));
            dt.Columns.Add("Sales Order Project Name", typeof(string));
            dt.Columns.Add("Customer Name", typeof(string));
            dt.Columns.Add("Product Type", typeof(string));
            dt.Columns.Add("Item Code", typeof(string));
            dt.Columns.Add("Item Name", typeof(string));
            dt.Columns.Add("Order Quantity", typeof(int));
            dt.Columns.Add("Total Production Qty", typeof(int));
            dt.Columns.Add("Total Deliveried Qty", typeof(int));
            dt.Columns.Add("Production Line Code", typeof(string));
            dt.Columns.Add("Production Line Name", typeof(string));
            dt.Columns.Add("Manager", typeof(string));
            dt.Columns.Add("Production Done Qty", typeof(int));
            dt.Columns.Add("Production State", typeof(string));
            dt.Columns.Add("Work Done Qty", typeof(int));
            dt.Columns.Add("Work Done Time", typeof(DateTime));
            dt.Columns.Add("Customer Warehouse", typeof(string));
            dt.Columns.Add("Delivery Date", typeof(DateTime));
            dt.Columns.Add("Deliveried Qty", typeof(string));

            //Add Rows in DataTable  
            var listData = _mESProductionResultService.getExportExcelData(UserProjectCode, ProductionCode, ItemCode, ItemName, ProjectStatus, SalesClasification, ProjectOrderType, SalesOrderProjectName, SalesOrderProjectCode);
            foreach (var item in listData)
            {
                dt.Rows.Add(item.No,
                            item.UserProjectCode,
                            item.ProjectName,
                            item.SalesClassificationName,
                            item.ProjectOrderTypeName,
                            item.ProjectStatusName,
                            item.SalesOrderProjectName,
                            item.CustomerName,
                            item.ProductType,
                            item.ItemCode,
                            item.ItemName,
                            item.OrderQuantity,
                            item.ProdcnDoneQty,
                            item.DeliveryTotalQty,
                            item.ProdcnLineCode,
                            item.ProductLineName,
                            item.Manager,
                            item.ProdDoneQty,
                            item.ProdcnLineStateName,
                            item.WorkDoneQty,
                            item.WorkDoneTime,
                            item.CustWH,
                            item.DeliveryDate,
                            item.DeliveryTotalQty);
            }
            dt.AcceptChanges();
            return dt;
        }

        [HttpGet]
        public IActionResult ExportExcelServer(string UserProjectCode, string ProductionCode, string ItemCode, string ItemName, string ProjectStatus, string SalesClasification, string ProjectOrderType, string SalesOrderProjectName,string SalesOrderProjectCode)
        {
            DataTable dt = GetExportExcelData(UserProjectCode, ProductionCode, ItemCode, ItemName, ProjectStatus, SalesClasification, ProjectOrderType, SalesOrderProjectName, SalesOrderProjectCode);

            string downloadExcelPath = _mESProductionResultService.ExportItemPartnerExcelFile(dt);

            var memory = new MemoryStream();
            using (var stream = new FileStream(downloadExcelPath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;
            var file = File(memory, ExcelExport.GetContentType(downloadExcelPath), downloadExcelPath.Remove(0, downloadExcelPath.LastIndexOf("/") + 1));
            memory.Dispose();
            dt.Dispose();
            return Json(new { Result = true, downloadExcelPath = downloadExcelPath, fileName = file.FileDownloadName });
        }

        [HttpGet]
        public ActionResult Download(string fileLink, string fileName)
        {
            if (fileName != null)
            {
                string Files = fileLink;
                byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
                System.IO.File.WriteAllBytes(Files, fileBytes);
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

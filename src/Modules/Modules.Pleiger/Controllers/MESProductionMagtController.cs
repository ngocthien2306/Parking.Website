using System;
using System.Collections.Generic;
using System.Linq;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using Modules.Admin.Services.ServiceImp;

namespace Modules.Pleiger.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
    public class MESProductionMagtController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESSaleProjectService saleProjectService;
        private readonly IMESComCodeService mesComCodeService;
        private readonly IMESProductionService productionService;
        private readonly IEmployeeService employeeService;
        private readonly IMESWarehouseService warehouseService;
        private readonly IAccessMenuService accessMenuService;

        private const string PROJECT_STATUS_PLAN = "PJST03";
        private const string PROJECT_STATUS_WORK = "PJST04";
        private const string PROJECT_STATUS_COMPLETED = "PJST05";
        private const string PRODUCTION_LINE_STATUS_DOING = "PJLN02";
        private const string PRODUCTION_LINE_STATUS_DONE = "PJLN03";

        public MESProductionMagtController(IMESProductionService productionService, IAccessMenuService accessMenuService, IMESSaleProjectService saleProjectService, IMESComCodeService mesComCodeService, IEmployeeService employeeService, IMESWarehouseService warehouseService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.productionService = productionService;
            this.saleProjectService = saleProjectService;
            this.contextAccessor = contextAccessor;
            this.employeeService = employeeService;
            this.mesComCodeService = mesComCodeService;
            this.warehouseService = warehouseService;
            this.accessMenuService = accessMenuService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult ProductionPlanning()
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
        public IActionResult WorkManagement()
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
        public IActionResult ProductionPlanningDetail(string projectCode,int menuid)
        {
           
            var listSumFileUploadByMenuID = accessMenuService.SelectSumFileUploadByMenuID(menuid, CurrentUser.UserID);         

            ViewBag.lstProdLines = productionService.GetListProdLinesMaster();
            ViewBag.lstStatusProdLines = mesComCodeService.GetListComCodeDTL("PJLN00");           
            var data = productionService.GetDetailData(projectCode, PROJECT_STATUS_PLAN);           
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            data.ID = "FileID" + ViewBag.Thread;
            if (listSumFileUploadByMenuID.Count > 0)
            {
                if (listSumFileUploadByMenuID[0].DELETE_FILE_SUM > 0)
                {
                    data.Delele_File = true;
                }
                if (listSumFileUploadByMenuID[0].UPLOAD_FILE_SUM > 0)
                {
                    data.Upload_File = true;
                }
            }
            return View(data);
        }
        public IActionResult WorkDetailManagement(string projectCode,int menuid)
        {
            var listSumFileUploadByMenuID = accessMenuService.SelectSumFileUploadByMenuID(menuid, CurrentUser.UserID);

            ViewBag.lstProdLines = productionService.GetListProdLinesMaster();
            ViewBag.lstStatusProdLines = mesComCodeService.GetListComCodeDTL("PJLN00");
            var data = productionService.GetDetailData(projectCode, PROJECT_STATUS_WORK);
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.CurentUser = CurrentUser.UserName;
            data.ID = "FileID" + ViewBag.Thread;    
            if (listSumFileUploadByMenuID.Count > 0)
            {
                if (listSumFileUploadByMenuID[0].DELETE_FILE_SUM > 0)
                {
                    data.Delele_File = true;
                }
                if (listSumFileUploadByMenuID[0].UPLOAD_FILE_SUM > 0)
                {
                    data.Upload_File = true;
                }
            }
            return View(data);
        }
        [HttpGet]
        public object GetListWareHouse(DataSourceLoadOptions loadOptions)
        {
            var data = warehouseService.GetListWareHouseByType("WHTP01");
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return loadResult;
        }
        [HttpGet]
        public object GetListFinishWareHouse(DataSourceLoadOptions loadOptions)
        {
            var data = warehouseService.GetListWareHouseByType("WHTP02");
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return loadResult;
        }
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions, string projectCode, string itemCode, string itemName, string customer)
        {
            var data = productionService.GetListData(PROJECT_STATUS_PLAN, projectCode, null, itemCode, itemName, customer);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListDataWorkMagt(DataSourceLoadOptions loadOptions, string projectCode, string productionCode, string itemCode, string itemName)
        {
            var data = productionService.GetListData(PROJECT_STATUS_WORK, projectCode, productionCode, itemCode, itemName, null);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListSalesProject(DataSourceLoadOptions loadOptions)
        {
            var data = saleProjectService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public object GetListEmployees(DataSourceLoadOptions loadOptions)
        {
            var data = employeeService.GetListEmployees();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            // return Content(JsonConvert.SerializeObject(loadResult), "application/json");

            return loadResult;
        }
        [HttpGet]
        public IActionResult GetListStatusProdLine(DataSourceLoadOptions loadOptions)
        {
            var data = mesComCodeService.GetListComCodeDTL("PJLN00");
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListDataProdLines(DataSourceLoadOptions loadOptions, string projectCode)
        {
            var data = productionService.GetListProjectProdcnLines(projectCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetDataProdLinesMaster(string prodlineCode)
        {
            var data = productionService.GetListProdLinesMaster().Where(m => m.ProductLineCode == prodlineCode).FirstOrDefault();

            return Json(data);
        }
        [HttpPost]
        public IActionResult onSave(string ProjectCode, string ProdcnCode, List<MES_ProjectProdcnLines> lstAdd, 
            List<MES_ProjectProdcnLines> lstEdit, List<MES_ProjectProdcnLines> lstDelete, 
            string MaterWHCode, string ProdcnMessage,
            DateTime PlanDoneDate)
        {
            //  string ProdcnCode = "ProdC1";
            var result = productionService.SaveProductPlainLines(ProjectCode, ProdcnCode, lstAdd, lstEdit, lstDelete, MaterWHCode, ProdcnMessage, PlanDoneDate);
            return Json(new { result.Success, result.Message });
            //return Json(new { Success= true });
        }
        [HttpPost]
        public IActionResult OnUpdateWorkPlan(string ProjectCode, string ProdcnCode, string RequestCode, string MaterialWarehouse, int OrderQty)
        {
            var result = productionService.OnUpdateWorkPlan(ProjectCode, ProdcnCode, PROJECT_STATUS_WORK, RequestCode,
                MaterialWarehouse, OrderQty, CurrentUser.UserID);
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnUpdateWorkCompleted(string ProjectCode, string ProdcnCode)
        {
            //var result = productionService.OnUpdateWorkPlan(ProjectCode, ProdcnCode, PROJECT_STATUS_COMPLETED, "", null, CurrentUser.UserID);
            var result = productionService.OnUpdateWorkCompleted(ProjectCode, ProdcnCode, PROJECT_STATUS_COMPLETED);
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnUpdateProdLineStatus(string ProjectCode, string ProdcnCode, string ProdcnLineCode, string ItemCode,
            string FNWarehouse, string RequestCode, string FinishWarehouseCodeSlt)
        {
            var result = productionService.OnUpdateProdLineStatus(ProjectCode, ProdcnCode, ProdcnLineCode, PRODUCTION_LINE_STATUS_DOING,
                ItemCode, FNWarehouse, RequestCode, FinishWarehouseCodeSlt, CurrentUser.UserID);
            return Json(new { result.Success, result.Message });
            // return Json(new { Success= true });
        }
        [HttpPost]
        public IActionResult OnUpdateProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity,
            string ItemCode, string FinishWHCode, string MasterWHCode)
        {
            string UserID = CurrentUser.UserID;
            string UserName = CurrentUser.UserName;
            var result = productionService.OnUpdateProdLineDoneQty(ProjectCode, ProdcnCode, ProdcnLineCode, ProductionQuantity, UserID,
                UserName, ItemCode, FinishWHCode, MasterWHCode);
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnUpdateProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity,
            string ItemCode, string FinishWHCode, string MasterWHCode)
        {
            string UserID = CurrentUser.UserID;
            string UserName = CurrentUser.UserName;

            var result = productionService.OnUpdateProdLineDoneQtyAndState(ProjectCode, ProdcnCode, ProdcnLineCode, ProductionQuantity,
                PRODUCTION_LINE_STATUS_DONE, UserID, UserName, ItemCode, FinishWHCode, MasterWHCode);
            return Json(new { result.Success, result.Message });
        }


        //Add By PVN
        [HttpGet]
        public IActionResult GetProductLine(DataSourceLoadOptions loadOptions)
        {
            var data = productionService.GetProductLine();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetProjectName(DataSourceLoadOptions loadOptions, string ProjectStatus)
        {
            var data = productionService.GetProjectName(ProjectStatus);
            //MES_SaleProject mES_SaleProject = new MES_SaleProject();
            //mES_SaleProject.ProjectName = "All";
            //mES_SaleProject.ProjectCode = "All";
            //data.Add(mES_SaleProject);
            //data.Reverse();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetItemName(DataSourceLoadOptions loadOptions, string ProjectStatus)
        {
            var data = productionService.GetItemName(ProjectStatus);
            //MES_Item mES_Item = new MES_Item();
            //mES_Item.ItemName = "All";
            //mES_Item.ItemCode = "All";
            //data.Add(mES_Item);
            //data.Reverse();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public IActionResult GetListDataWorkMagtNew(DataSourceLoadOptions loadOptions, string ProductLineCode, string ProjectCode, string itemCode)
        {
            var data = productionService.GetListDataNew(ProductLineCode, ProjectCode, itemCode);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }


        [HttpPost]
        public IActionResult CheckQtyOfEachItemIsEnoughInWarehouse(string ProjectCode, string ProdcnCode, string RequestCode,
            string MaterialWarehouse, int OrderQty)
        {
            var result = productionService.CheckQtyOfEachItemIsEnoughInWarehouse(ProjectCode, ProdcnCode, RequestCode,
                MaterialWarehouse, OrderQty, CurrentUser.UserID);
            return Json(new { result.Success, result.Message, result.Data });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using Modules.Pleiger.SalesProject.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;
using Modules.Pleiger.Inventory.Services.IService;
using Modules.Pleiger.Production.Services.IService;
using Modules.Pleiger.CommonModels;
using InfrastructureCore.Web.Extensions;
using InfrastructureCore;
using Modules.Common.Models;
using Modules.FileUpload.Services.IService;

namespace Modules.Pleiger.Production.Controllers
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
        private readonly IMESSaleProjectService _mesSaleProjectService;
        private readonly IUserService _userService;
        private readonly IFileService _filesService;

        private const string PROJECT_STATUS_PLAN = "PJST03";
        private const string PROJECT_STATUS_WORK = "PJST04";
        private const string PROJECT_STATUS_COMPLETED = "PJST05";
        private const string PRODUCTION_LINE_STATUS_DOING = "PJLN02";
        private const string PRODUCTION_LINE_STATUS_DONE = "PJLN03";
        private const string PROJECT_STATUS_PROJECT_RETURN = "PJST08";// Return

        private const string PROJECT_STATUS_PROJECT_CLOSE = "PJST11";// Close

        private const string PROJECT_STATUS_PROJECT_CANCEL = "PJST01"; // Cancel



        public MESProductionMagtController(IFileService filesService, IMESSaleProjectService mesSaleProjectService, IUserService userService, 
            IMESProductionService productionService ,IAccessMenuService accessMenuService, IMESSaleProjectService saleProjectService, 
            IMESComCodeService mesComCodeService, IEmployeeService employeeService, IMESWarehouseService warehouseService, 
            IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.productionService = productionService;
            this.saleProjectService = saleProjectService;
            this.contextAccessor = contextAccessor;
            this.employeeService = employeeService;
            this.mesComCodeService = mesComCodeService;
            this.warehouseService = warehouseService;
            this.accessMenuService = accessMenuService;
            this._userService = userService;
            this._mesSaleProjectService = mesSaleProjectService;
            this._filesService = filesService;
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
        public IActionResult ProductionPlanningDetail(string projectCode, int menuid, string vbParent)
        {
            

            ViewBag.lstProdLines = productionService.GetListProdLinesMaster();
            ViewBag.lstStatusProdLines = mesComCodeService.GetListComCodeDTL("PJLN00");           
            var model = productionService.GetDetailData(projectCode, PROJECT_STATUS_PLAN);           
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            ViewBag.lstInternalExternal = mesComCodeService.GetListComCodeDTL("INEX00");



            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            var listUserPermissionInGroup = accessMenuService.SelectUserPermissionInGroup(menuid, CurrentUser.UserID);
            var listUserPermissionAccessMenu = accessMenuService.SelectUserPermissionAccessMenu(menuid, CurrentUser.UserID);
            var ListButtonPermissionByUser = accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuid, CurrentUser.UserCode);

            if (listUserPermissionInGroup.Count > 0) // check user Permission in Group
            {
                if (listUserPermissionInGroup[0].SAVE_YN_SUM > 0)
                {
                    model.Btn_Save = true;
                }

            }
            if (listUserPermissionAccessMenu.Count > 0) //check user Permission
            {
                if (listUserPermissionAccessMenu[0].SAVE_YN == true)
                {
                    model.Btn_Save = true;
                }
            }
            string check = "";
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            if (model.InitialCode == true)
            {
                check = "Yes";
            }
            else
            {
                check = "No";
            }

            if (projectCode != null)
            {
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
                ViewBag.SiteID = CurrentUser.SiteID;
                ViewBag.SystemUserType = checkUserLogin.SystemUserType;
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

            ViewBag.code = check;
            ViewBag.ProjectName = model.ProjectName;
            ViewBag.ProjectCode = model.ProjectCode;
            ViewBag.UserProjectCode = model.UserProjectCode;
            ViewBag.requestCode = model.RequestCode;
            ViewBag.PartnerName = model.PartnerName;
            ViewBag.ProjectStatus = model.ProjectStatus;
            ViewBag.ProjectStatusName = model.ProjectStatusName;
            ViewBag.RequestDate = model.RequestDate;
            ViewBag.RequestDate = model.RequestDate;
            ViewBag.OrderQuantity = model.OrderQuantity;
            ViewBag.RequestMessage = model.RequestMessage;
            ViewBag.ItemName = model.ItemName;
            ViewBag.ItemCode = model.ItemCode;
            ViewBag.RequestType = model.RequestType;
            ViewBag.UserRequest = CurrentUser.UserName;
            ViewBag.MenuID = menuid;
            ViewBag.Parent = vbParent;
            return View(model);
        }
        public IActionResult WorkDetailManagement(string projectCode,int menuid)
        {
            // Quan add 2021-02-25        
            var ListButtonPermissionByUser = accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuid, CurrentUser.UserCode);
            
            ViewBag.lstProdLines = productionService.GetListProdLinesMaster();
            ViewBag.lstStatusProdLines = mesComCodeService.GetListComCodeDTL("PJLN00");
            
            var data = productionService.GetDetailData(projectCode, PROJECT_STATUS_WORK);
            if(data!=null)
            {
                ViewBag.Thread = Guid.NewGuid().ToString("N");
                ViewBag.CurentUser = CurrentUser.UserName;
                data.ID = "FileID" + ViewBag.Thread;
                if (ListButtonPermissionByUser.Count > 0)
                {
                    data.Delele_File = ListButtonPermissionByUser[0].DELETE_FILE_YN;
                    data.Upload_File = ListButtonPermissionByUser[0].UPLOAD_FILE_YN;                 
                    data.Btn_Save    = ListButtonPermissionByUser[0].SAVE_YN;
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
        // Quan add 
        [HttpGet]
        public object GetListWareHouseInternal(DataSourceLoadOptions loadOptions)
        {
            var data = warehouseService.GetListWareHouseInternal();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return loadResult;
        }
        [HttpGet]
        public IActionResult GetListDataWorkPlan(DataSourceLoadOptions loadOptions, string userProjectCode, string projectName, string itemCode, string itemName, string customer, string SalesClassification, string projectOrderType, string saleOrderProjectName)
        {
            
            var data = productionService.GetListData(PROJECT_STATUS_PLAN, userProjectCode, projectName, null, itemCode, itemName, customer, 
                SalesClassification, projectOrderType, saleOrderProjectName, null, null);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public IActionResult GetListDataWorkMagt(DataSourceLoadOptions loadOptions, string userProjectCode, string projectName, string productionCode, string itemCode, string itemName, string SalesClassification, string projectOrderType, string saleOrderProjectName)
        {
            var data = productionService.GetListData(PROJECT_STATUS_WORK, userProjectCode, projectName, productionCode, itemCode, itemName, null, 
                SalesClassification, projectOrderType, saleOrderProjectName, null, null);
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
            var result = productionService.OnUpdateWorkCompleted(ProjectCode, ProdcnCode, PROJECT_STATUS_COMPLETED);
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnUpdateProjectReturn(string ProjectCode, string ProdcnCode, bool Flag, string ProjectStatusNow, string Planning)
        {
            // PROJECT_STATUS_PROJECT_RETURN = PJST08
            // Project return
            string remark = "Project Return(Close Work running)";
            string status = PROJECT_STATUS_PROJECT_CLOSE;

            if (Planning == "Y") status = PROJECT_STATUS_PROJECT_CANCEL;

            if (!Flag) remark = "Project Close(Pending Work running)";

            var result = productionService.OnUpdateProjectReturn(ProjectCode, ProdcnCode, status, remark, ProjectStatusNow);
          
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnUpdateProdLineStatus(string ProjectCode, string ProdcnCode, string ProdcnLineCode, string ItemCode,
            string FNWarehouse, string RequestCode, string FinishWarehouseCodeSlt , string ProductionMessage)
        {
            var result = productionService.OnUpdateProdLineStatus(ProjectCode, ProdcnCode, ProdcnLineCode, PRODUCTION_LINE_STATUS_DOING,
                ItemCode, FNWarehouse, RequestCode, FinishWarehouseCodeSlt, CurrentUser.UserID, ProductionMessage);
            return Json(new { result.Success, result.Message });
            // return Json(new { Success= true });
        }
        [HttpPost]
        public IActionResult OnUpdateProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity,string ItemCode, string FinishWHCode,
            string MasterWHCode, DateTime? EndDate, DateTime? PlanStartDate, DateTime? PlanEndDate, int CumulativeProductionQuantity, string ProductionMessage,string GroupLine)
        {
            string UserID = CurrentUser.UserID;
            string UserName = CurrentUser.UserName;
            var result = productionService.OnUpdateProdLineDoneQty(ProjectCode, ProdcnCode, ProdcnLineCode, ProductionQuantity, UserID,
                UserName, ItemCode, FinishWHCode, MasterWHCode, EndDate, PlanStartDate, PlanEndDate, CumulativeProductionQuantity, ProductionMessage, GroupLine);
            return Json(new { result.Success, result.Message });
        }
        [HttpPost]
        public IActionResult OnUpdateProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string ItemCode, string FinishWHCode,
            string MasterWHCode, DateTime? EndDate, DateTime? PlanStartDate, DateTime? PlanEndDate, int CumulativeProductionQuantity, string ProductionMessage,string GroupLine)
        {
            string UserID = CurrentUser.UserID;
            string UserName = CurrentUser.UserName;

            var result = productionService.OnUpdateProdLineDoneQtyAndState(ProjectCode, ProdcnCode, ProdcnLineCode, ProductionQuantity, PRODUCTION_LINE_STATUS_DONE, UserID, UserName,
                ItemCode, FinishWHCode, MasterWHCode, EndDate, PlanStartDate, PlanEndDate, CumulativeProductionQuantity, ProductionMessage, GroupLine);
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

        [HttpGet]
        public IActionResult CheckDataDataWorkMagt(DataSourceLoadOptions loadOptions,string ProjectCode)
        {
            var data = productionService.CheckDataDataWorkMagt(ProjectCode);          

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
        
    }
}

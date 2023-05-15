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
using AutoMapper;

namespace Modules.Pleiger.Production.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "<Pending>")]
    public class MESProductionStatusController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESSaleProjectService saleProjectService;
        private readonly IMESComCodeService mesComCodeService;
        private readonly IMESProductionService productionService;
        private readonly IEmployeeService employeeService;
        private readonly IMESWarehouseService warehouseService;
        private readonly IAccessMenuService accessMenuService;
        private readonly IMapper mapper;

        private const string PROJECT_STATUS_PLAN = "PJST03";
        private const string PROJECT_STATUS_WORK = "PJST04";
        private const string PROJECT_STATUS_COMPLETED = "PJST05";
        private const string PRODUCTION_LINE_STATUS_DOING = "PJLN02";
        private const string PRODUCTION_LINE_STATUS_DONE = "PJLN03";

        public MESProductionStatusController(IMESProductionService productionService, IMapper mapper, IAccessMenuService accessMenuService, IMESSaleProjectService saleProjectService, IMESComCodeService mesComCodeService, IEmployeeService employeeService, IMESWarehouseService warehouseService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.productionService = productionService;
            this.saleProjectService = saleProjectService;
            this.contextAccessor = contextAccessor;
            this.employeeService = employeeService;
            this.mesComCodeService = mesComCodeService;
            this.warehouseService = warehouseService;
            this.accessMenuService = accessMenuService;
            this.mapper = mapper;

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
        [HttpGet]
        public IActionResult GetChartData(DataSourceLoadOptions loadOptions)
        {
            //get all MES_SalesProject
            var ListProject = productionService.GetAllSalesProject();
            //get all MES_ProjectProdcnLines
            var ListProjectProdcnLines = productionService.GetAllProjectProdcnLines();
            //get all GetAllProjectProdcnWorkResults 
            var ListProjectProdcnWorkResults = productionService.GetAllProjectProdcnWorkResults();

            for (int i = 0; i < ListProjectProdcnLines.Count; i++)
            {
                var ProdcnLinesByProject = ListProjectProdcnWorkResults.Where(n => n.ProjectCode == ListProjectProdcnLines[i].ProjectCode 
                                                                               &&  n.ProdcnLineCode== ListProjectProdcnLines[i].ProdcnLineCode).ToList();
                ListProjectProdcnLines[i].MES_ProjectProdcnWorkResultsChart = ProdcnLinesByProject;
            }

            for (int i = 0; i< ListProject.Count;i++)
            {
                var ProdcnLinesByProject = ListProjectProdcnLines.Where(n => n.ProjectCode == ListProject[i].ProjectCode ).ToList();
                ListProject[i].MES_ProjectProdcnLinesChart = ProdcnLinesByProject;
                
            }
           // var loadResult = DataSourceLoader.Load(ListProjectProdcnWorkResults, loadOptions);
            return Content(JsonConvert.SerializeObject(ListProject), "application/json");
        }
    }
}

using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Pleiger.Production.Controllers
{
    public class MESProjectStatusController : BaseController
    {
        private readonly IMESProjectStatusService _MESProjectStatusService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserService _userService;

        public MESProjectStatusController(IMESProjectStatusService mESProjectStatusService, IUserService userService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._MESProjectStatusService = mESProjectStatusService;
            this._contextAccessor = contextAccessor;
            this._userService = userService;
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
        public object LoadProductTypeCombobox(DataSourceLoadOptions loadOptions)
        {
            var result = _MESProjectStatusService.getProductTypeCombobox("", "en");
            return DataSourceLoader.Load(result, loadOptions);
        }

        [HttpPost]
        public IActionResult SearchProjectStatus(string ProjectOrderType, string SaleOrderProjectCode, string ProjectStatus, string ProductType, string ProjectName, string UserProjectCode, string SalesClassification,string checkCode)
        {
            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            string Userlogin = null;
            if (CurrentUser != null && CurrentUser.UserType != "G000C001" && CurrentUser.UserType != "G000C002" && CurrentUser.UserType != "G000C003")
            {
                //lstParam.Add(new SPParameter { Key = "Userlogin", Value = CurrentUser.UserCode });
                Userlogin = CurrentUser.UserCode;
            }
            else
            {
                //lstParam.Add(new SPParameter { Key = "Userlogin", Value = null });
                Userlogin = null;
            }


            var result = _MESProjectStatusService.searchMESSaleProject(
                ProjectOrderType,
                SaleOrderProjectCode,
                ProjectStatus,
                ProductType,
                ProjectName,
                UserProjectCode,
                SalesClassification,
                Userlogin,checkCode);
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        public IActionResult SalesStatusByCustomer()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuID = menuID;
            return View();
        }

        public IActionResult SearchSalesStatusByCustomer(string ProjectOrderType, string SaleOrderProjectName, string ProductionProjectCode, string ProductProjectName, string ProductProjectStatus,  string Customer)
        {
            var result = _MESProjectStatusService.SearchSalesStatusByCustomer(
                ProjectOrderType,
                SaleOrderProjectName,
                ProductionProjectCode,
                ProductProjectName,
                ProductProjectStatus,
                Customer, CurrentUser.UserCode);
            return Content(JsonConvert.SerializeObject(result), "application/json");

            
        }
        public IActionResult GetCustomer()           
        {
            var result = _MESProjectStatusService.GetCustomer(CurrentLanguages.Substring(1));
            return Content(JsonConvert.SerializeObject(result), "application/json");
            
        }
    }
}

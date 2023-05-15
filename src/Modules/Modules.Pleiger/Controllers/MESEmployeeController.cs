using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Modules.Pleiger.Controllers
{
    public class MESEmployeeController : BaseController
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMESComCodeService _mesComCodeService;
        private readonly IEmployeeService _employeeService;
        private const string superAdminRole = "G000C001";
        public MESEmployeeController(IHttpContextAccessor contextAccessor, IUserService userService, IMESComCodeService mesComCodeService,IEmployeeService employeeService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._mesComCodeService = mesComCodeService;
            this._employeeService = employeeService;
            this.userService = userService;

        }
        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            //CurrentUser.UserID
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;

            var model = new MESEmployees();      
            return View(model);
        }

        [HttpGet]
        public IActionResult GetListEmployess(DataSourceLoadOptions loadOptions)
        {
            var data = _employeeService.GetListEmployess();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        [HttpGet]
        public object ListSearchEmployee(DataSourceLoadOptions loadOptions, string PartnerCode, string EmployeeNumber, string EmployeeNameKr, string EmployeeNameEng ,string UseYN)
        {
            var data = _employeeService.ListSearchEmployee(PartnerCode, EmployeeNumber, EmployeeNameKr, EmployeeNameEng, UseYN);        
            return DataSourceLoader.Load(data, loadOptions);
            
        }
        [HttpGet]
        public IActionResult EmployeeDetailPopup(string EmployeeCode)
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var model = new MESEmployees();
            if (EmployeeCode != null)
            {
                model = _employeeService.GetEmployess(EmployeeCode);
            }
            return PartialView("EmployeeDetailPopup", model);
        }
        [HttpGet]
        public IActionResult CheckEmployeeDuplicate(string EmployeeNumber)
        {
            var result = _employeeService.GetEmployess(EmployeeNumber);
            return Json(result);
        }

        #region "Insert - Update - Delete
        [HttpPost]
        public IActionResult SaveMESEmployee(MESEmployees model)
        {            
            var result = _employeeService.SaveMESEmployee(model, CurrentUser.UserID);         
            return Json(result);
        }
        [HttpPost]
        public IActionResult DeleteSalesProject(string EmpCode)
        {
            var result = _employeeService.DeleteMESEmployee(EmpCode);
            return Json(result);
        }
        [HttpPost]
        public IActionResult DeleteEmployeeInfo(List<MESEmployees> listEmployeeInfo)
        {
            var result = _employeeService.DeleteEmployeeInfo(listEmployeeInfo);
            return Json(new { result.Success, result.Message });
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using InfrastructureCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using InfrastructureCore;
using Newtonsoft.Json;
using Modules.Common.Models;
using InfrastructureCore.DAL;
using System.IO;
using Modules.Pleiger.Services.ServiceImp;

namespace Modules.Pleiger.Controllers
{
    public class MESPurchaseOrderController : BaseController
    { 
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPurchaseService _purchaseService;
        private readonly IEmployeeService _employeeService;
        private readonly IUserService _userService;
        private readonly IAccessMenuService _accessMenuService;

        #region constructor     
        public MESPurchaseOrderController(IHttpContextAccessor httpContextAccessor,
            IAccessMenuService accessMenuService,
            IPurchaseService purchaseService,
            IEmployeeService employeeService,
            IUserService userService) : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _purchaseService = purchaseService;
            _employeeService = employeeService;
            _userService = userService;
            _accessMenuService = accessMenuService;

        }
        #endregion
        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.CurrentUser = CurrentUser;

            //check user code -> partner or admin or customer
            //var checkResult = _purchaseService.CheckUserRole_Partner(CurrentUser.UserCode);
            var checkUserLogin = _userService.CheckUserType(CurrentUser?.UserID  );
            ViewBag.CheckUserType = checkUserLogin.SystemUserType;
            //if (checkUserLogin.SystemUserType.Equals("G000C004") || checkUserLogin.SystemUserType.Equals("G000C002")) //Admin of pleiger

            //Quan add          
            var PurchaseYN = false;
            var SelectUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(CurrentMenu.MenuID, CurrentUser.UserID);
            if (SelectUserPermissionAccessMenu.Count > 0)
            {
                PurchaseYN = SelectUserPermissionAccessMenu[0].PURCHASE_ORDER_YN;
            }
            else
            {
                PurchaseYN = false;
            }

            if (checkUserLogin.SystemUserType.Equals("G000C003") || checkUserLogin.SystemUserType.Equals("G000C002")) //Admin of pleiger
            {
                if (PurchaseYN == true)
                {
                    ViewBag.URole = "Admin";
                    ViewBag.PartnerCode = null; //usercode
                    ViewBag.VisibledPrintingStatusCol = true;
                    return View();
                }
            }
            if (checkUserLogin.SystemUserType.Equals("G000C005") || checkUserLogin.SystemUserType.Equals("G000C007") || checkUserLogin.SystemUserType.Equals("G000C006"))//Partner and PartnerCustomer
            {
                ViewBag.URole = "Partner";
                ViewBag.VisibledPrintingStatusCol = false;
                string partnerCode = "";
                var checkUserEmployee = new CHECKRESULT();
                if (checkUserLogin != null)
                {
                    checkUserEmployee = _userService.CheckUserEmployee(checkUserLogin.UserCode);
                    if (checkUserEmployee != null) /// có trong table employ
                    {
                        partnerCode = checkUserEmployee.PartnerCode;
                    }
                    else
                    {
                        partnerCode = checkUserLogin.UserCode;
                    }
                }
                ViewBag.PartnerCode = partnerCode;

                return View();
            }
            else if(checkUserLogin.SystemUserType.Equals("G000C006"))//Customer
            {
                ViewBag.URole = "Customer";
                ViewBag.PartnerCode = CurrentUser.UserCode;
                return View();
            }
            return View();
        }

         [HttpGet]
         public object GetDataAndSearch(DataSourceLoadOptions loadOptions,
              string StartPurchaseDate,
              string EndPurchaseDate,
              string    ItemCode,
              string    ItemName,
              string    ProjectCode,
              string    PONumber,
              string PartnerCode,string UserProjectCode ,string UserPONumber)
        {       
            var data = _purchaseService.SearchAll(StartPurchaseDate, EndPurchaseDate,
                ItemCode,ItemName,ProjectCode,PONumber,PartnerCode,UserProjectCode,UserPONumber);   
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpPut]
        public IActionResult SaveDataPurchaseOrder(string flag, string itemPurchaseOrder, string userModify)
        {
             List<MES_Purchase> listPurchaseOrder = JsonConvert.DeserializeObject<List<MES_Purchase>>(itemPurchaseOrder);
            var result = _purchaseService.Update_Data_MES_PurchaseOrderList(flag, listPurchaseOrder);
            return Json(result);
        }
    }
    //Excel Utils  
    
    
}

using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Utils;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Modules.Pleiger.PurchaseOrder.Controllers
{
    public class MESPurchaseOrderController : BaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPurchaseService _purchaseService;
        private readonly IUserService _userService;
        private readonly IAccessMenuService _accessMenuService;

        #region constructor     
        public MESPurchaseOrderController(IHttpContextAccessor httpContextAccessor,
            IAccessMenuService accessMenuService,
            IPurchaseService purchaseService,
            IUserService userService) : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _purchaseService = purchaseService;
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
            ViewBag.VisibledPrintingStatusCol = false;
            ViewBag.URole = "";
            ViewBag.PartnerCode = null; //usercode

            var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            ViewBag.CheckUserType = checkUserLogin.SystemUserType;
            ViewBag.SystemUserType = checkUserLogin.SystemUserType;
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
                else
                {
                    ViewBag.URole = "Pleiger User";
                    ViewBag.PartnerCode = null; //usercode
                    ViewBag.VisibledPrintingStatusCol = false;
                }
            }
            if (checkUserLogin.SystemUserType.Equals("G000C005") || checkUserLogin.SystemUserType.Equals("G000C007"))//Partner and PartnerCustomer
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
            else if (checkUserLogin.SystemUserType.Equals("G000C006"))//Customer
            {
                ViewBag.URole = "Customer";
                ViewBag.PartnerCode = CurrentUser.UserCode;
                return View();
            }
            return View();
        }

        [HttpGet]
        public object GetDataAndSearch(DataSourceLoadOptions loadOptions,
             string startDate,
             string endDate,
             string itemCode,
             string itemName,
             string projectCode,
             string poNumber,
             string partnerCode, string projectName, string userPONumber, string poStatus,string SalesClassification)
        {
            var data = _purchaseService.SearchAll(startDate, endDate,
                itemCode, itemName, projectCode, poNumber, partnerCode, projectName, userPONumber, poStatus, SalesClassification);
            return DataSourceLoader.Load(data, loadOptions);
        }
        [HttpGet]
        public object GetDataPurchaseOrderList(DataSourceLoadOptions loadOptions,
           string startDate,
           string endDate,
           string userPONumber,
           string projectName,
           string poStatus,
           string itemCode,
           string itemName,
           string Remark1,
           string partnerCode)

        {
            var data = _purchaseService.GetDataPurchaseOrderList(startDate, endDate,
                userPONumber, projectName, poStatus, itemCode, itemName, Remark1, partnerCode);
            return DataSourceLoader.Load(data, loadOptions);
        }        

        [HttpPut]
        public IActionResult SaveDataPurchaseOrder(string flag, string itemPurchaseOrder, string userModify)
        {
            List<MES_Purchase> listPurchaseOrder = JsonConvert.DeserializeObject<List<MES_Purchase>>(itemPurchaseOrder);
            var result = _purchaseService.Update_Data_MES_PurchaseOrderList(flag, listPurchaseOrder);
            return Json(result);
        }
    }


}

using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Authorization;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IAdminLayoutService adminLayoutService;
        private readonly IAccessMenuService _accessMenuService;

        private const string superAdminRole = "G000C001";

        public UserController(IUserService userService, IAdminLayoutService adminLayoutService, IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService) : base(contextAccessor)
        {
            this.userService = userService;
            this.adminLayoutService = adminLayoutService;
            this.contextAccessor = contextAccessor;
            this._accessMenuService = accessMenuService;
        }

        #region "Get Data"

        //[CustomAuthorization]
        public IActionResult UserManagement()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            #region set permission button
            ViewBag.Excel = false;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;

                // Quan add set permission button in toolbar
                var listSumFileUploadByMenuID = _accessMenuService.SelectSumFileUploadByMenuID(CurrentMenu.MenuID, CurrentUser.UserID);
                var listUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(CurrentMenu.MenuID, CurrentUser.UserID);

                // check user In Group Permission
               
                if (listSumFileUploadByMenuID.Count > 0)
                {
                    if (listSumFileUploadByMenuID[0].EXCEL_YN_SUM > 0)
                    {
                        ViewBag.Excel = true;
                    }
                }
                // check user Permission
                if (listUserPermissionAccessMenu.Count > 0)
                {
                    if (listUserPermissionAccessMenu[0].EXCEL_YN == true)
                    {
                        ViewBag.Excel = true;
                    }
                }
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;

            // Quan add set permission button in grid
            if (CurrentUser!=null)
            {
                ViewBag.SysTemUserType = CurrentUser.SystemUserType;
            }
            else
            {
                ViewBag.SysTemUserType = "";
            }
            #endregion

            return View();
        }

        // Get list User
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions)
        {
            List<SYUser> data = new List<SYUser>();
            if (CurrentUser != null)
            {
                if (CurrentUser.UserType == superAdminRole)
                {
                    //data = userService.GetListDataAll(CurrentUser.SiteID);
                    data = userService.GetListUserAll();
                    int no = 1;
                    //data.ForEach(x =>
                    //{
                    //    x.No = no++;
                    //});
                }
                else
                {
                    data = userService.GetListDataUserBySite(CurrentUser.SiteID);
                    int no = 1;
                    //data.ForEach(x =>
                    //{
                    //    x.No = no++;
                    //});
                }
            }
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get list User by UserCode
        public IActionResult GetListDataByUserCode(DataSourceLoadOptions loadOptions, string UserCode)
        {
            List<SYUser> data = new List<SYUser>();

            data = userService.GetListDataAll(CurrentUser.SiteID);

            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        // Get common code
        [HttpGet]
        public IActionResult GetListRole(DataSourceLoadOptions loadOptions)
        {
            List<SYComCode> lstType = new List<SYComCode>();
            lstType = adminLayoutService.SelectComCodeAndGrpByGRP("G000");
            if (CurrentUser != null)
            {
                if (CurrentUser.UserType != superAdminRole)
                {
                    lstType = lstType.Where(x => x.DTL_CD != superAdminRole).ToList();
                }
            }

            var loadResult = DataSourceLoader.Load(lstType, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
      
        // Show popup change password
        [HttpGet]
        public IActionResult ShowPopupChangPassword(SYUser data)
        {
            bool adminUpdate = false;
            if (CurrentUser != null && CurrentUser.UserID != data.UserID)
            {
                adminUpdate = true;
            }
            ViewBag.AdminUpdate = adminUpdate;

            return PartialView("PopupChangePassword", data);
        }

        [HttpGet]
        public IActionResult ShowPopupCreateUser(string viewbagIndex, int menuParent)
        {
            ViewBag.SAVE_YN = false;
            ViewBag.DELETE_YN = false;
            var ListButtonPermissionByUser = _accessMenuService.GetButtonPermissionByUser(CurrentUser.SiteID, menuParent, CurrentUser.UserCode);
            if (ListButtonPermissionByUser.Count > 0)
            {
                ViewBag.SAVE_YN = ListButtonPermissionByUser[0].SAVE_YN;
                ViewBag.DELETE_YN = ListButtonPermissionByUser[0].DELETE_YN;
            }

            ViewBag.Thread = Guid.NewGuid().ToString("N");
            //var checkUserLogin = _userService.CheckUserType(CurrentUser.UserID);
            //ViewBag.UserType = checkUserLogin.SystemUserType;
            ViewBag.Index = viewbagIndex;
            ViewBag.UserName = CurrentUser.UserName;
            ViewBag.UserId = CurrentUser.UserID;


            return PartialView("ShowCreateUser");

        }

        // Show popup User Information
        [HttpGet]
        public IActionResult ShowPopupUserInformation()
        {
            string name = userService.GetSystemUserTypeByUserId(CurrentUser.UserID);
            string groupNames = string.Empty;
            //get group
            List<SYUsersInGroup> userInGroup = userService.GetUserInGroupsByUserId(CurrentUser?.UserID);
            if(userInGroup!= null && userInGroup.Count > 0)
            {
                foreach(var group in userInGroup)
                {
                   if(!string.IsNullOrEmpty(groupNames))
                   {
                        groupNames += ",";
                        groupNames += group.GROUP_NAME;
                   }
                   else
                   {
                        groupNames += group.GROUP_NAME;
                   }
                }
            }

            ViewBag.CurrentUser = CurrentUser;
            ViewBag.UserTypeName = name;
            ViewBag.GroupNames = groupNames;
            return PartialView("PopupUserInformation");
        }
        // Get list User Search
        [HttpGet]
        public IActionResult GetListDataSearch(DataSourceLoadOptions loadOptions,string UserCode, string UserName, string FirstName,
            string UserType, string IsBlock, string UseYN)
         {
            List<SYUser> data = new List<SYUser>();
            SYUser model = new SYUser();
            model.UserCode = UserCode;
            model.UserName = UserName;
            model.FirstName = FirstName;
            model.UserType = UserType;
            if(!string.IsNullOrEmpty(IsBlock))
            {
                model.IsBlock = Convert.ToBoolean(IsBlock);
            }    
            model.UseYN = UseYN;
            if (CurrentUser != null)
            {
                if (CurrentUser.UserType == superAdminRole)
                {
                    data = userService.GetListDataAll(CurrentUser.SiteID);
                }
                else
                {
                    data = userService.GetListDataUserBySiteSearch(CurrentUser.SiteID, model);
                }
            }
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }
        //public object SearchSaleProjects(DataSourceLoadOptions loadOptions, MES_SaleProject model)
        //{
        //    var result = _mesSaleProjectService.SearchSaleProject(model);
        //    //return DataSourceLoader.Load(result, loadOptions);
        //    return Json(result);
        //}
        #endregion

        #region "Insert - Update - Delete"

        // Insert  - Update User

        [HttpPost]
        public IActionResult SaveData(SYUser data)
        {
            if (CurrentUser.UserType != superAdminRole)
            {
                data.SiteID = CurrentUser.SiteID;
            }
            var result = userService.InsertUpdateData(data, CurrentUser.UserID);

            return Json(result);
        }
        [HttpPost]
        public IActionResult SaveUser(SYUser data)
        {
            if (CurrentUser.UserType != superAdminRole)
            {
                data.SiteID = CurrentUser.SiteID;
            }
            var result = userService.InsertUpdateData(data, CurrentUser.UserID);

            return Json(result);
        }

        // Delete User
        [HttpPost]
        public IActionResult DeleteData(string userId,string userCode)
        {
            var result = userService.DeleteData(userId,userCode);

            return Json(result);
        }

        // Update User Information
        [HttpPost]
        public IActionResult UpdateUserInformation(string firstName, string lastName, string email, string oldPassword, string newPassword,string userTypeName)
        {
            var result = userService.UpdateUserInformation(CurrentUser.UserID, firstName, lastName, email, oldPassword, newPassword, userTypeName, CurrentUser.SiteID);
            return Json(result);
        }

        // Change password
        [HttpPost]
        public IActionResult ChangePassword(bool adminUpdate, string userID, string oldPassword, string newPassword)
        {
            var result = userService.ChangePassword(adminUpdate, userID, oldPassword, newPassword, CurrentUser.SiteID);

            return Json(result);
        }

        #endregion
    }
}

using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Modules.Admin.Controllers
{
    public class UserGroupController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IGroupUserService groupUserService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAccessMenuService _accessMenuService;
            
        public UserGroupController(IUserService userService, IGroupUserService groupUserService, IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService) : base(contextAccessor)
        {
            this.groupUserService = groupUserService;
            this._userService = userService;
            this._contextAccessor = contextAccessor;
            this._accessMenuService = accessMenuService;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            #region set permission button
            ViewBag.Save = false;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
                // Quan add set permission button in toolbar
                var listSumFileUploadByMenuID = _accessMenuService.SelectSumFileUploadByMenuID(CurrentMenu.MenuID, CurrentUser.UserID);
                var listUserPermissionAccessMenu = _accessMenuService.SelectUserPermissionAccessMenu(CurrentMenu.MenuID, CurrentUser.UserID);

                // check user In Group Permission
               
                if (listSumFileUploadByMenuID.Count > 0)
                {
                    if (listSumFileUploadByMenuID[0].SAVE_YN_SUM > 0)
                    {
                        ViewBag.Save = true;
                    }
                }
                // check user Permission
                if (listUserPermissionAccessMenu.Count > 0)
                {
                    if (listUserPermissionAccessMenu[0].SAVE_YN == true)
                    {
                        ViewBag.Save = true;
                    }
                }
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            // Quan add set permission button in grid
            if (CurrentUser != null)
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

        // Get list SYUser Group
        [HttpGet]
        public IActionResult GetListUserGroup(DataSourceLoadOptions loadOptions, int siteID)
        {
            var data = groupUserService.GetListUserGroup(CurrentUser, siteID);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        // Get list SYUser in Site
        [HttpGet]
        public IActionResult GetListUserInSite(DataSourceLoadOptions loadOptions, int siteID)
        {
            var data = _userService.GetListDataUserBySite(siteID);
            var loadResult = DataSourceLoader.Load(data, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        // Get SYUser in SYUser Group
        [HttpGet]
        public IActionResult GetListUserInUserGroup(string groupID, int siteID)
        {
            var data = groupUserService.GetListUserGroupSeletedByGroupID(groupID, siteID);
            return Json(data);
        }
        public IActionResult CheckUserInGroup(string userID, int siteID)
        {
            var data = groupUserService.CheckUserInGroup(userID, siteID);
            return Json(data);
        }    
        #endregion

        #region "Insert - Update - Delete"

        #region "SYUser Group"

        // Save SYUser Group
        [HttpPost]
        public IActionResult SaveDataGroupMaster(SYUserGroups data, int siteId)
        {
            Result result = groupUserService.SaveDataGroupMaster(data, CurrentUser, siteId);
            return Json(new { result.Success, result.Message });
        }

        // Delete SYUser Group
        [HttpPost]
        public IActionResult DeleteGroupMaster(SYUserGroups data, int siteId)
        {
            Result result = groupUserService.DeleteGroupMaster(data, CurrentUser, siteId);
            return Json(new { result.Success, result.Message });
        }

        #endregion

        #region "SYUser in Group"

        // Set SYUser in SYUser Group
        [HttpPost]
        public IActionResult SetUserIntoUserGroup(string groupId, List<SYUser> data, int siteId)
        {
            var rs = groupUserService.SetUserIntoUserGroup(groupId, data, CurrentUser, siteId);
            return Json(new { Success = rs.Success });
        }

        #endregion

        #endregion
    }
}

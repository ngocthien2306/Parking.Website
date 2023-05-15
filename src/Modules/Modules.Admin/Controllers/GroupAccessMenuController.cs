using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
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
    public class GroupAccessMenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IUserService userService;
        private readonly IAccessMenuService accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;

        public GroupAccessMenuController(IUserService userService, IMenuService menuService, IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService) : base(contextAccessor)
        {
            this.menuService = menuService;
            this.userService = userService;
            this.contextAccessor = contextAccessor;
            this.accessMenuService = accessMenuService;
        }

        #region "Get Data"

        // Group Access Menu
        [HttpGet]
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

        // Get list Menu of Site
        [HttpGet]
        public IActionResult GetListMenuData(DataSourceLoadOptions loadOptions, int siteID)
        {
            var data = menuService.GetListDataByGroup(siteID);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        // Get list SYUser Group of Site
        [HttpGet]
        public IActionResult GetListUserGroupBySite(DataSourceLoadOptions loadOptions, int siteID)
        {
            var data = userService.GetListGroupBySiteID(siteID);
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        // Get list SYUser Group access Menu
        [HttpGet]
        public IActionResult GetListUserGroupSeleted(string menuId)
        {
            var data = accessMenuService.GetListAccessMenuGroupByMenuID(menuId, CurrentUser.SiteID);
            return Json(new { data = data });
        }

        // Quan add 2020/08/18
        // Get list SYUser Group access Menu By MenuID
        // Sum col DELETE_FILE_YN 
        // Sum UPLOAD_FILE_SUM
        [HttpGet]
        public IActionResult SelectSumDelUploadByMenuID(int menuId,string UserID)
        {
            var data = accessMenuService.SelectSumFileUploadByMenuID(menuId, UserID);
            return Json(new { data = data });
        }
        #endregion

        #region "Insert - Update - Delete"

        // Set Group Access Menu
        [HttpPost]
        public IActionResult SetUserGroupAccessMenu(int menuId, List<SYUserGroups> data)
        {
            var rs = accessMenuService.SaveGroupAccessMenu(menuId, data, CurrentUser);
            return Json(new { Success = rs.Success });
        }

        #endregion
    }
}

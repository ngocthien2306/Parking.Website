using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Authorization;
using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;

using Newtonsoft.Json;

namespace Modules.Admin.Controllers
{
    public class UserAccessMenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IUserService userService;
        private readonly IAccessMenuService accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMapper mapper;

        public UserAccessMenuController(IUserService userService, IMenuService menuService, IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IMapper mapper) : base(contextAccessor)
        {
            this.menuService = menuService;
            this.userService = userService;
            this.contextAccessor = contextAccessor;
            this.accessMenuService = accessMenuService;
            this.mapper = mapper;
        }

        #region "Get Data"

        // SYUser Access Menu
        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            var userInfo = contextAccessor.HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            var curUrl = (contextAccessor.HttpContext.Request.Path.Value + Request.QueryString);
            //var count = contextAccessor.HttpContext.Request.Path.Value.Split("/")[1].Length + 1;
            var curUrlTemp = URLRequest.URLSubstring(curUrl);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            ViewBag.MenuId = curMenu != null ? curMenu.MenuID : 0;
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

        // Get list SYUser of Site
        [HttpGet]
        public IActionResult GetListDataUserBySite(DataSourceLoadOptions loadOptions, int siteID)
        {
            var data = userService.GetListDataUserBySite(siteID);
            List<SYUserAccessMenus> lstUser = new List<SYUserAccessMenus>();
            lstUser = mapper.Map<List<SYUser>, List<SYUserAccessMenus>>(data);
            var loadResult = DataSourceLoader.Load(lstUser, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        // Get list SYUser access Menu
        [HttpGet]
        public IActionResult GetListUserSeleted(string menuId)
        {
            var data = accessMenuService.GetListAccessMenuUserByMenuID(menuId);
            return Json(new { data = data });
        }

        #endregion

        #region "Insert - Update - Delete"

        // Set SYUser Access Menu
        [HttpPost]
        public IActionResult SetUserAccessMenu(int menuId, List<SYUserAccessMenus> data)
        {
            var rs = accessMenuService.SaveUserAccessMenu(menuId, data, CurrentUser);
            return Json(new { Success = rs.Success });
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using Modules.Admin.Models;
using InfrastructureCore;

namespace Modules.Admin.Controllers
{
    public class UserAccessMenuNewController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IUserService userService;
        private readonly IAccessMenuService accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IMapper mapper;

        public UserAccessMenuNewController(IUserService userService, IMenuService menuService, IHttpContextAccessor contextAccessor, IAccessMenuService accessMenuService, IMapper mapper) : base(contextAccessor)
        {
            this.menuService = menuService;
            this.userService = userService;
            this.contextAccessor = contextAccessor;
            this.accessMenuService = accessMenuService;
            this.mapper = mapper;
        }


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

            // 
            ViewBag.CurrentLanguage = CurrentLanguages.Substring(1) != null ? CurrentLanguages.Substring(1) : "en";
            return View();
        }

        public IActionResult GetAllUser()
        {
            var result = userService.GetAllUser(CurrentUser.SiteID);
            int no = 1;
            result.ForEach(x =>
            {
                x.No = no++;
            });

            return Content(JsonConvert.SerializeObject(result));
        }

        public IActionResult GetAllSite()
        {
            var result = userService.GetAllSite(CurrentUser.SiteID);
            return Content(JsonConvert.SerializeObject(result));
        }

        [HttpGet]
        public IActionResult GetMenuByUserId(string UserCode)
        {
            var result = userService.GetMenuByUserId(UserCode,CurrentUser.SiteID);
            ///ViewBag.CurrentLanguage = CurrentLanguages.Substring(1) != null ? CurrentLanguages.Substring(1) : "en";
            return Content(JsonConvert.SerializeObject(result));
        }


        [HttpGet]
        public IActionResult GetCheckedMenu(string UserCode)
        {
            var result = userService.GetCheckedMenu(UserCode,CurrentUser.SiteID);
           // ViewBag.CurrentLanguage = CurrentLanguages.Substring(1) != null ? CurrentLanguages.Substring(1) : "en";
            return Content(JsonConvert.SerializeObject(result));
        }
        [HttpGet]
        public IActionResult GetPermission(string UserCode, int MenuId)
        {
            var result = userService.GetPermission(UserCode, MenuId, CurrentUser.SiteID);
            return Content(JsonConvert.SerializeObject(result));
        }
        [HttpGet]
        public IActionResult GetPermissionByGroup(string UserCode, int MenuId,int GroupID)
        {
            var result = userService.GetPermissionByGroup(UserCode, MenuId, CurrentUser.SiteID, GroupID);
            return Content(JsonConvert.SerializeObject(result));
        }
        //Quan add GetGroupByUser
        [HttpGet]
        public IActionResult GetGroupByUser(string UserCode, int MenuId)
        {
            var result = userService.GetGroupByUser(UserCode, MenuId, CurrentUser.SiteID);
            return Content(JsonConvert.SerializeObject(result));
        }
        
        #region Update - Insert Permission

        [HttpPost]
        public IActionResult UpdateUserPermission(string listUpdatePermission/*, string listDelete*/)
        {
            //var listDeleteObj = JsonConvert.DeserializeObject<List<UserPermissionUpdate>>(listDelete);
            var listObj = JsonConvert.DeserializeObject<List<UserPermissionUpdate>>(listUpdatePermission);
            var result = userService.UpdateUserPermission(listObj, CurrentUser);
            return Content(JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// Quan add 2021-03-04
        /// Get list Permission By Userid And MenuID
        /// Use view UserAccessMenuNew function MoveToSelectedGrid 
        /// SP SP_NEW_USER_PERMISSON @Method = GetPermissionByUserAndMenuID
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="MenuID"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetPermissionByUserAndMenuID(string UserID, int MenuID)
        {
            var result = userService.GetPermissionByUserAndMenuID(UserID, MenuID, CurrentUser.SiteID);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion
    }
}
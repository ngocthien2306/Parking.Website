using InfrastructureCore;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Services.IService;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Modules.Admin.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IHttpContextAccessor contextAccessor;

        public MenuController(IMenuService menuService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.menuService = menuService;
            this.contextAccessor = contextAccessor;
        }

        #region "Get Data"

        public IActionResult MenuManagement()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            int menuID = 0;
            if (CurrentMenu != null)
            {
                menuID = CurrentMenu.MenuID;
            }
            ViewBag.MenuId = menuID;
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentLanguage = CurrentLanguages.Substring(1) != null ? CurrentLanguages.Substring(1) : "en";
            return View();
        }

        // Get list Menu by siteID
        [HttpGet]
        public IActionResult GetListDataByGroup(int siteID)
        {
            var data = menuService.GetListDataByGroup(siteID);
            var model = JsonConvert.SerializeObject(data);

            return Json(model);
        }

        // Get list Menu Level
        [HttpGet]
        public IActionResult GetListMenuLevel()
        {
            var data = menuService.GetListMenuLevel();

            return Content(JsonConvert.SerializeObject(data), "application/json");
        }

        // Get list Menu Parent
        [HttpGet]
        public IActionResult GetListMenuParent(int siteID, int menuLevel)
        {
            var result = menuService.GetListMenuParent(siteID, menuLevel);

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        // Get list Page Layout
        [HttpGet]
        public IActionResult GetListPageLayout()
        {
            var result = menuService.GetListPageLayout();

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
        // Get list Page Layout Board
        [HttpGet]
        public IActionResult GetListPageBoard()
        {
            //var result = menuService.GetListPageBoard();
            // Quan add site
            var result = menuService.GetListPageBoard(CurrentUser.SiteID);

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
        
        // Get list Menu Icon
        [HttpGet]
        public IActionResult GetListMenuIcon()
        {
            var result = menuService.GetListMenuIcon();

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        #endregion

        #region "Insert - Update - Delete"

        // Save Menu
        [HttpPost]
        public IActionResult SaveData(int siteID, int menuID, string menuName,string menuNameEng, string menuPath,
            int menuLevel, int? menuParentID, int? menuSeq, string adminLevel, string menuType,
            int? programID, string mobileUse, string intraUse, string menuDesc, string startupPageUse, string isCanClose, int? menuIcon,int? programIsBoard)
        {
            var result = menuService.InsertUpdateData(siteID, menuID, menuName, menuNameEng, menuPath, menuLevel, menuParentID,
                menuSeq, adminLevel, menuType, programID, mobileUse, intraUse, menuDesc, startupPageUse, isCanClose, menuIcon);

            return Json(result);
        }

        // Delete Menu
        [HttpPost]
        public IActionResult DeleteData(int menuID)
        {
            var result = menuService.DeleteData(menuID);

            return Json(result);
        }

        #endregion

        #region GetMenuIDByBoadID
        [HttpPost]
        public IActionResult GetMenuIDByBoadID(string strUrl)
        {
            var data = menuService.GetListDataByGroup(CurrentUser.SiteID);
            var infoMenu = data.Where(m => m.MenuPath == strUrl).FirstOrDefault();
            int menuID = 0;
            if (infoMenu != null)
            {
                menuID = infoMenu.MenuID;
            }
            else
            {
                infoMenu = data.Where(m => m.MenuPath == "/BoardManagement").FirstOrDefault();
            }           
            return Json(infoMenu);
        }
        #endregion

        #region Get Path By MenuID
        [HttpPost]
        public IActionResult GetPathIDByMenuID(int MenuID)
        {
            var data = menuService.GetListDataByGroup(CurrentUser.SiteID);
            var infoMenu = data.Where(m => m.MenuID == MenuID).FirstOrDefault();
            int menuID = 0;
            if (infoMenu != null)
            {
                menuID = infoMenu.MenuID;
            }
            else
            {
                infoMenu = data.Where(m => m.MenuPath == "/BoardManagement").FirstOrDefault();
            }
            return Json(infoMenu);
        }
        #endregion
    }
}

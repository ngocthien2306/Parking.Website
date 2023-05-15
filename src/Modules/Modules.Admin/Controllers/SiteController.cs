using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Site;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Modules.Admin.Services.IService;

using Modules.FileUpload.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Modules.Admin.Controllers
{
    public class SiteController : BaseController
    {
        private readonly ISiteService siteService;
        private readonly IFileService fileService;
        private readonly IHttpContextAccessor contextAccessor;

        public SiteController(ISiteService siteService, IFileService fileService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.siteService = siteService;
            this.fileService = fileService;
            this.contextAccessor = contextAccessor;
        }

        #region "Get Data"

        public IActionResult SiteManagement()
        {
            var curUrlTemp = (contextAccessor.HttpContext.Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var curMenu = CurrentUser != null ? CurrentUser.AuthorizedMenus.Where(m => m.MenuPath == curUrl).FirstOrDefault() : null;
            int menuID = curMenu != null ? curMenu.MenuID : 0;
            ViewBag.MenuId = menuID;

            if (CurrentUser != null && CurrentUser.UserType != "G000C001")
            {
                string url = Url.Action("SiteDetail", "Site", new { siteID = CurrentUser.SiteID, menuID = menuID });
                return Redirect(url);
            }


            return View();
        }

        // Get list Site
        [HttpGet]
        public IActionResult GetListData(DataSourceLoadOptions loadOptions)
        {
            var data = siteService.GetListData();
            var loadResult = DataSourceLoader.Load(data, loadOptions);

            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        // Get Site detail
        [HttpGet]
        public IActionResult SiteDetail(int siteID, int menuID)
        {
            ViewBag.MenuId = menuID;

            var model = siteService.GetDetail(siteID);

            var listMenuType = new List<SelectListItem>();
            listMenuType.Add(new SelectListItem() { Text = "Top Left", Value = "TopLeft" });
            listMenuType.Add(new SelectListItem() { Text = "TreeView", Value = "TreeView" });
            ViewBag.ListMenuType = listMenuType;

            var listSideBarType = new List<SelectListItem>();
            listSideBarType.Add(new SelectListItem() { Text = "Dark", Value = "Dark" });
            listSideBarType.Add(new SelectListItem() { Text = "Light", Value = "Light" });
            ViewBag.ListSideBarType = listSideBarType;

            var listVisibleFooter = new List<SelectListItem>();
            listVisibleFooter.Add(new SelectListItem() { Text = "True", Value = "True" });
            listVisibleFooter.Add(new SelectListItem() { Text = "False", Value = "False" });
            ViewBag.ListVisibleFooter = listVisibleFooter;

            return View(model);
        }

        #endregion

        #region "Insert - Update - Delete"

        // Insert - Update Site
        [HttpPost]
        public IActionResult SaveData(int siteID, string siteCode, string siteName, string siteDescription)
        {
            var userInfo = HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            var result = siteService.SaveData(siteID, siteCode, siteName, siteDescription, userInfo.UserID);

            return Json(result);
        }

        // Update Site detail
        [HttpPost]
        public IActionResult UpdateData(SYSite model, IFormFile loginBackgroundImage, IFormFile icon, IFormFile file)
        {
            if (icon != null)
            {
                string fileName = Guid.NewGuid() + "_" + icon.FileName;
                string directory = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\IconPage");
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\IconPage", fileName);

                var uploadResult = fileService.SaveFile(icon, path, directory);
                if (!uploadResult.Success)
                {
                    return Json(uploadResult);
                }

                model.IconPath = fileName;
            }

            if (loginBackgroundImage != null)
            {
                string fileName = Guid.NewGuid() + "_" + loginBackgroundImage.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\BackgroundImage", fileName);
                string directory = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\BackgroundImage");

                var uploadResult = fileService.SaveFile(loginBackgroundImage, path, directory);
                if (!uploadResult.Success)
                {
                    return Json(uploadResult);
                }

                model.LoginBackgroundImage = fileName;
            }

            if (file != null)
            {
                string fileName = Guid.NewGuid() + "_" + file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\LogoImage", fileName);
                string directory = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\LogoImage");

                var uploadResult = fileService.SaveFile(file, path, directory);
                if (!uploadResult.Success)
                {
                    return Json(uploadResult);
                }

                model.LogoPath = fileName;
            }

            var result = siteService.UpdateData(model, CurrentUser?.UserID);

            return Json(result);
        }

        // Delete Site
        [HttpPost]
        public IActionResult DeleteData(int siteID)
        {
            var result = siteService.DeleteData(siteID);

            return Json(result);
        }

        #endregion
    }
}

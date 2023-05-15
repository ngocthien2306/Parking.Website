using AutoMapper;
using InfrastructureCore.DataAccess;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Site;
using InfrastructureCore.Web.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Controllers;
using Modules.Admin.Services.IService;
using System.Linq;


namespace Modules.Pleiger.SystemMgt.Controllers
{
    public class MESAccountController : LoginController
    {
        private readonly IUserManager userManager;
        private readonly ISiteService siteService;
        private readonly AppConfig _appSettings;


        private readonly IMapper mapper;
        private readonly IAccessMenuService accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserService userService;
        string defaultSiteCode = "";

        public MESAccountController(IUserManager userManager, ISiteService siteService, IMapper mapper,
            IAccessMenuService accessMenuService, IHttpContextAccessor contextAccessor,
            IUserService userService) : base(contextAccessor, userManager, siteService, mapper, accessMenuService)
        {
            this.userManager = userManager;
            this.siteService = siteService;
            this.mapper = mapper;
            this.accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            this.userService = userService;
            SiteConfig site = new SiteConfig();
            defaultSiteCode = site.SiteCode;
            _appSettings = new AppConfig();



        }

        #region
        public IActionResult MESLogin()
        {
            var siteSetting = siteService.GetDetailByCode(defaultSiteCode);
            ViewBag.SiteSetting = siteSetting;

            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region "Insert - Update - Delete"

        // On Login site MES
        [HttpPost]
        //[Authorize]
        public IActionResult OnMESLogin(string UserName, string Password)
        {
            // Response.Cookies.Append("langname", "en",
            //new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
            ValidateResult result = new ValidateResult();
            string MESHome = CurrentLanguages + "/MESHome";
            // get UserName from EmployeeNo
            // var temp = employeeService.GetEmployess(UserName);
            int SiteID = int.Parse(_appSettings.SiteID);
            var user = userService.GetListDataAll(SiteID).Where(m => m.UserCode == UserName).FirstOrDefault();
            if (user != null)
            {
                result = this.AuthenticateUser(defaultSiteCode, user.UserName, Password, MESHome);
                //if (result.Success)
                //{
                //    this.SiteSettings.LeftMenuComponentUrl = "LeftMenu";
                //}
                // var temp = User.Identity;
            }
            else
            {
                result.Success = false;
                result.Error = ValidateResultError.CredentialNotFound;
            }
            // Redirecto to Home of MES

            return Json(result);
        }

        #endregion

        #region WorkLogin

        [HttpPost]
        public IActionResult OnWorkLogin(string id, string pw)
        {
            int SiteID = int.Parse(_appSettings.SiteID);
            var result = this.OnWorkLogin(defaultSiteCode, id, pw);
            var user = userService.GetListDataAll(SiteID).Where(m => m.UserCode == id).FirstOrDefault();
            if (user != null)
            {
                result = this.OnWorkLogin(defaultSiteCode, user.UserName, pw);
            }
            else
            {
                result.Success = false;
                result.Error = ValidateResultError.CredentialNotFound;
            }
            return Json(result);
        }

        #endregion
    }
}

using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.DataAccess;
using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Models.Site;
using InfrastructureCore.Web.Controllers;
using InfrastructureCore.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Modules.Admin.Controllers
{
    //[Authorize]
    public class LoginController : BaseController
    {
        //private readonly IWebAuthentication _authentication;//Quan add
        private readonly IUserManager userManager;
        private readonly ISiteService siteService;
        private readonly IMapper mapper;
        private readonly IAccessMenuService accessMenuService;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly AppConfig _appSettings;

        private const string superAdminRole = "G000C001";
        private const string adminRole = "G000C002";
        public const string SiteCode = "";
        public const string UserName = "";
        public const string PassWord = "";
        public const string ReturnUrl = "";
        //public LoginController(IWebAuthentication authentication, ISiteService siteService)
        public LoginController(IHttpContextAccessor contextAccessor, IUserManager userManager, ISiteService siteService,
            IMapper mapper, IAccessMenuService accessMenuService)
        {
            //this.authentication = authentication;
            this.userManager = userManager;
            this.siteService = siteService;
            this.mapper = mapper;
            this.accessMenuService = accessMenuService;
            this.contextAccessor = contextAccessor;
            // _authentication = authentication;
            _appSettings = new AppConfig();
            // this.menuRepository = menuRepository;            
        }

        [AllowAnonymous]
        public IActionResult Index(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        // Login
        [HttpPost]
        [Authorize]
        public IActionResult OnLogin(string siteCode, string userName, string password, string returnUrl)
        {
            //var result = authentication.CheckLogInTL(siteCode, userName, password);
            var result = AuthenticateUser(siteCode, userName, password, returnUrl);

            return Json(result);
        }
        // Logout
        public IActionResult Logout()
        {
            var UserNameAuditLog = HttpContext.Session.GetString("UserNameAuditLog");
            var PasswordAuditLog = HttpContext.Session.GetString("PasswordAuditLog");
            var SiteIDAuditLog   = HttpContext.Session.GetInt32("SiteIDAuditLog");

            userManager.SignOut(contextAccessor.HttpContext);
            
            var language = Request.Cookies["langname"];
            SaveAuditLog(UserNameAuditLog, PasswordAuditLog, HttpContext.Request.Host.Value, "LOGOUT", "User Logout", SiteIDAuditLog);
            //SaveAuditLog(CurrentUser.UserName,CurrentUser.Password,"LOGOUT")
            //string url = Url.Action("MESLogin", "MESAccount");
            //return Redirect("/"+language);
            // Quan change multilang from login
            return Redirect("/" + language);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        protected ValidateResult AuthenticateUser(string siteCode, string userName, string password, string returnUrl)
        {
            var result = userManager.Validate(siteCode, userName, password);

            if (result.Success)
            {
                // Quan add AuditLog         
                HttpContext.Session.SetString("UserNameAuditLog", result.SYUser.UserCode);
                HttpContext.Session.SetString("PasswordAuditLog", result.SYUser.Password);              
                HttpContext.Session.SetInt32("SiteIDAuditLog", result.SYUser.SiteID);
                SaveAuditLog(result.SYUser.UserCode, result.SYUser.Password, HttpContext.Request.Host.Value, "LOGIN", "User login successful", result.SYUser.SiteID);
                // End
                userManager.SignIn(contextAccessor.HttpContext, result.SYUser, false);
                SYLoggedUser userLogin = mapper.Map<SYLoggedUser>(result.SYUser);
                List<SYMenu> lstMenu = null;

                // authentication successful so generate jwt token
                var token = generateJwtToken(userLogin);

                if (userLogin.UserType == superAdminRole)
                {
                    lstMenu = accessMenuService.GetAccessMenuWithSuperAdmin();
                }
                else if (userLogin.UserType == adminRole)
                {
                    // some thing wrong?
                    // admin also has add menus from permissions
                    //lstMenu = accessMenuService.GetAccessMenuWithAdmin(userLogin.SiteID);
                    // Quan change 2021-03-07
                    // Hiện tại set user admin giống như user khác                    
                    lstMenu = accessMenuService.GetAccessMenuWithUserCode(userLogin.UserCode, userLogin.SiteID, userLogin.UserID);
                }
                else
                {
                    lstMenu = accessMenuService.GetAccessMenuWithUserCode(userLogin.UserCode, userLogin.SiteID, userLogin.UserID);
                }

                userLogin.AuthorizedMenus = lstMenu;

                // get toolbar
                userLogin.MenuAccessList = accessMenuService.GetListAccessToobarWithUser(userLogin);

                // store logged in information
                HttpContext.Session.Set<SYLoggedUser>("UserInfo", userLogin);

                // Get Site Setting
                var siteSettingExist = HttpContext.Session.Get<SYSite>("SiteInfo");
                if (siteSettingExist == null)
                {
                    var siteSetting = siteService.GetDetail(userLogin.SiteID);
                    if (siteSetting != null)
                    {
                        HttpContext.Session.Set<SYSite>("SiteInfo", siteSetting);
                    }
                }
                //
                result.ReturnURL = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index", "Home") : returnUrl;
            }

            return result;
        }
        protected ValidateResult GetAllMenu(string siteCode, string userName, string password, string returnUrl, string Lang)
        {
            var result = userManager.Validate(siteCode, userName, password);

            if (result.Success)
            {
                userManager.SignIn(contextAccessor.HttpContext, result.SYUser, false);
                SYLoggedUser userLogin = mapper.Map<SYLoggedUser>(result.SYUser);
                List<SYMenu> lstMenu = null;

                // authentication successful so generate jwt token
                var token = generateJwtToken(userLogin);

                if (userLogin.UserType == superAdminRole)
                {
                    lstMenu = accessMenuService.GetAccessMenuWithSuperAdmin();
                }
                else if (userLogin.UserType == adminRole)
                {
                    // some thing wrong?
                    // admin also has add menus from permissions
                    lstMenu = accessMenuService.GetAccessMenuWithAdmin(userLogin.SiteID);
                }
                else
                {
                    lstMenu = accessMenuService.GetAccessMenuWithUserCode(userLogin.UserCode, userLogin.SiteID, userLogin.UserID);
                }

                userLogin.AuthorizedMenus = lstMenu;

                // get toolbar
                userLogin.MenuAccessList = accessMenuService.GetListAccessToobarWithUser(userLogin);

                // store logged in information
                HttpContext.Session.Set<SYLoggedUser>("UserInfo", userLogin);

                // Get Site Setting
                var siteSettingExist = HttpContext.Session.Get<SYSite>("SiteInfo");
                if (siteSettingExist == null)
                {
                    var siteSetting = siteService.GetDetail(userLogin.SiteID);
                    if (siteSetting != null)
                    {
                        HttpContext.Session.Set<SYSite>("SiteInfo", siteSetting);
                    }
                }

                result.ReturnURL = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index", "Home") : returnUrl;
            }

            return result;
        }
        public ValidateResult OnWorkLogin(string siteCode, string id, string pw)
        {
            var result = userManager.Validate(siteCode, id, pw);
            return result;
        }
        [HttpGet]
        public IActionResult CheckSessionUser()
        {
            var sessionUser = HttpContext.Session.Get<SYLoggedUser>("UserInfo");

            if (sessionUser == null)
            {
                return Json(false);
            }
            return Json(true);
        }
        // helper methods
        private string generateJwtToken(SYLoggedUser user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserID.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public IActionResult AuditLog()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            return View();
        }
        // Quan add Auditlog
        [HttpGet]
        public object GetAuditLogTracking(DataSourceLoadOptions loadOptions, string UserName, string startDate, string endDate)
        {
            var SiteID = HttpContext.Session.GetInt32("SiteIDAuditLog");
            var result = userManager.GetAuditLogTracking(UserName, SiteID, startDate, endDate);
            //return DataSourceLoader.Load(result, loadOptions);
            return Json(result);

        }
        [HttpPost]
        public IActionResult SaveAuditLog(string userName, string password, string url, string action, string message, int? siteid)
        {

            // var a = HttpContext.Request.Path;
            // var b = HttpContext.Request.Headers;
            SYAuditLogTracking model = new SYAuditLogTracking();
            model.USER_INFO = "UserName: " + userName + " ; " + "PassWord: " + password;
            model.USERNAME = userName;
            model.PASSWORD = password;
            model.ACTION_TYPE = action;
            model.SOURCE_IP = HttpContext.Connection.RemoteIpAddress.ToString();
            model.URL = url;
            model.DATE_LOG = DateTime.Now;
            model.MESSAGE = message;
            model.SITE_ID = siteid;
            model.HEADER_MAP = JsonConvert.SerializeObject(HttpContext.Request.Headers);
            string[] HEADER_MAP = model.HEADER_MAP.Split(",\"");
            string header = "";                 
            for(int i =0; i< HEADER_MAP.Length; i++)
            {
                header = header + "<li><p>" + HEADER_MAP[i] + "</p></li>";
            }    
            model.HEADER_MAP = header;
            header = "";
            var result = userManager.SaveAuditLogTracking(model);
            return Json("");
        }

    }
}
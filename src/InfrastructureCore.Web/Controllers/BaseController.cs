using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Models.Site;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace InfrastructureCore.Web.Controllers
{

    public abstract class BaseController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public BaseController()
        {
        }

        public BaseController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected SYLoggedUser CurrentUser
        {
            get
            {
                return _contextAccessor.HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            }
        }

        protected SYSiteSettings SiteSettings
        {
            get
            {
                return _contextAccessor.HttpContext.Session.Get<SYSiteSettings>("SiteInfo");
            }
        }

        protected string CurrentLanguages
        {            
            get
            {
                var lang = "/ko";
                if (Request.Cookies["langname"] != null && Request.Cookies["langname"] != "ko")
                {
                    lang = "/" + Request.Cookies["langname"];
                }
                return lang;
            }
        }

        protected SYMenu CurrentMenu
        {
            get
            {
                var userInfo = _contextAccessor.HttpContext.Session.Get<SYLoggedUser>("UserInfo");
                var curUrl = (_contextAccessor.HttpContext.Request.Path.Value + Request.QueryString);
                var curUrlTemp = URLRequest.URLSubstring(curUrl);
                var curMenu = userInfo != null ? userInfo.AuthorizedMenus.Where(m => m.MenuPath == curUrlTemp).FirstOrDefault() : null;
                return curMenu;
            }
        }

        // Check session User is exists or not
        protected bool CheckSessionIsExists()
        {
            var result = CurrentUser == null ? false : true;
            return result;
        }
    }
}

using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.Models;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLFXFramework.WebHost.ViewComponents
{
    public class TopMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userInfo = HttpContext.Session.Get<SYLoggedUser>("UserInfo");

            var menuLogin = new List<SYMenu>();
            if (userInfo != null)
            {
                menuLogin = userInfo.AuthorizedMenus;
            }

            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            SetSessionMenu(curUrl, menuLogin);

            return View(menuLogin);
        }

        private void SetSessionMenu(string url, List<SYMenu> menus)
        {
            var menu = menus.FirstOrDefault(m => m.MenuPath == url);
            if (menu != null)
            {
                HttpContext.Session.Set<SYMenu>("session_menu", menu);
            }
        }
    }
}

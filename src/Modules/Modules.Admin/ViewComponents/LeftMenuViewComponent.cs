using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLFXFramework.WebHost.ViewComponents
{
    public class LeftMenuViewComponent : ViewComponent
    {
        public LeftMenuViewComponent()
        {
        }

        public virtual async Task<IViewComponentResult> InvokeAsync(List<SYMenu> model)
        {
            var curUrlTemp = (Request.Path.Value + Request.QueryString);
            var curUrl = URLRequest.URLSubstring(curUrlTemp);
            var menuExist = HttpContext.Session.Get<List<SYMenu>>("MenuLeft");
            if (menuExist != null)
            {
                var checkExist = menuExist.Find(x => x.MenuPath == curUrl);
                if (checkExist != null)
                {
                    SetSessionMenu(curUrl, menuExist);
                    return View(menuExist);
                }
            }
            if (model != null)
            {
                SetSessionMenu(curUrl, model);
            }

            return View(model);
        }

        private void SetSessionMenu(string url, List<SYMenu> menus)
        {
            var menu = menus.FirstOrDefault(m => m.MenuPath == url);
            if (menu != null)
            {
                HttpContext.Session.Set<SYMenu>("session_menu", menu);
            }
        }


        //public async Task<IViewComponentResult> InvokeAsync()
        //{
        //    // var logged = HttpContext.Session.Get<LoggedInfo>("logged");
        //    //string empcode = logged != null ? logged.empcode : "not login";
        //    List<SYPageLayout> menus = new List<SYPageLayout>();
        //    menus = await _dynamicPageService.GetPageLayoutWithType("G001C001").ConfigureAwait(true);

        //    return View(menus);
        //}

        //private void SetSessionMenu(string url, List<SYMenuViewModel> menus)
        //{
        //    var menu = menus.FirstOrDefault(m => m.MenuPath == url);
        //    if (menu != null)
        //    {
        //        HttpContext.Session.Set<SYMenuViewModel>("session_menu", menu);
        //    }
        //}
    }
}

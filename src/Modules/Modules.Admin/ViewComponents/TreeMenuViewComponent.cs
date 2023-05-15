using InfrastructureCore.Extensions;
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
    public class TreeMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userInfo = HttpContext.Session.Get<SYLoggedUser>("UserInfo");

            var menuLogin = new List<SYMenu>();
            if (userInfo != null)
            {
                menuLogin = userInfo.AuthorizedMenus;
            }

            var menuMobile = new List<SYMenu>();
            if (HttpContext.Request.IsMobileBrowser())
            {
                menuMobile = menuLogin.Where(x => x.MobileUse == "Y").ToList();
                if (menuMobile.Count > 0)
                {
                    for (int i = 0; i < menuMobile.Count; i++)
                    {
                        bool changeData = false;

                        if (menuMobile[i].MenuLevel != 1)
                        {
                            var menu = new SYMenu();
                            menu = SecursiveMenuMobile(menuMobile[i], menuLogin);
                            var checkData = menuMobile.FirstOrDefault(x => menu.MenuID == x.MenuID);
                            if (checkData == null)
                            {
                                menuMobile.Add(menu);
                                changeData = true;
                            }

                            if (menu.MenuLevel != 1)
                            {
                                menu = SecursiveMenuMobile(menu, menuLogin);
                            }

                            checkData = menuMobile.FirstOrDefault(x => menu.MenuID == x.MenuID);
                            if (checkData == null)
                            {
                                menuMobile.Add(menu);
                                changeData = true;
                            }
                        }
                        i = changeData ? i - 1 : i;
                    }
                }

                var curUrlTemp = (Request.Path.Value + Request.QueryString);
                var curUrl = URLRequest.URLSubstring(curUrlTemp);
                SetSessionMenu(curUrl, menuMobile);

                return View(menuMobile);
            }
            else
            {
                var curUrlTemp = (Request.Path.Value + Request.QueryString);
                var curUrl = URLRequest.URLSubstring(curUrlTemp);
                SetSessionMenu(curUrl, menuLogin);

                return View(menuLogin);
            }
        }

        private void SetSessionMenu(string url, List<SYMenu> menus)
        {
            var menu = menus.FirstOrDefault(m => m.MenuPath == url);
            if (menu != null)
            {
                HttpContext.Session.Set<SYMenu>("session_menu", menu);
            }
        }

        private SYMenu SecursiveMenuMobile(SYMenu menu, List<SYMenu> data)
        {
            return data.FirstOrDefault(x => x.MenuID == menu.MenuParentID);
        }
    }
}

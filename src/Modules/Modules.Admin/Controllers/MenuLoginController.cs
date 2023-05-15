using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Models.Site;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace Modules.Admin.Controllers
{
    public class MenuLoginController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;

        public MenuLoginController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        #region "Get Data"

        [HttpGet]
        public IActionResult LoadLeftMenuComponent(int menuID)
        {
            var userInfo = HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            //bao add
            if (userInfo == null)
            {
                return NotFound();
            }
            /////
            var siteSetting = HttpContext.Session.Get<SYSite>("SiteInfo");

            var menuLeft = new List<SYMenu>();
            var menuLeftsearchname = new List<SYMenu>();
            if (userInfo != null)
            {
                menuLeft = userInfo.AuthorizedMenus.Where(x => x.MenuID == menuID).ToList();
                menuLeft.AddRange(GetListMenuLeft(userInfo.AuthorizedMenus, menuID));

            }

            SetSessionMenu(menuLeft);
            //viewbag
            ViewBag.CurrentLanguage = CurrentLanguages.Substring(1) != null ? CurrentLanguages.Substring(1) : "en";

            return ViewComponent(siteSetting.SideMenuComponentName == null ? "LeftMenu" : siteSetting.SideMenuComponentName, menuLeft);
        }

        // Quan add 2020-12-04       
        // GetTabActiveDefault
        [HttpGet]
        public IActionResult GetTabActiveDefault()
        {       
             var userInfo = HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            if (userInfo == null)
            {
                return NotFound();
            }
            var MenuActivefirst = new List<SYMenu>();

            if (CurrentUser.AuthorizedMenus.Count > 0)
            {
                var Menulv1 = new List<SYMenu>();          
                var Menulv2 = new List<SYMenu>();
                var Menulv3 = new List<SYMenu>();
                var menuLeft = new List<SYMenu>();

                Menulv1 = CurrentUser.AuthorizedMenus.Where(x => x.MenuLevel == 1).ToList();          
                menuLeft = userInfo.AuthorizedMenus.Where(x => x.MenuID == Menulv1[0].MenuID).ToList();
                menuLeft.AddRange(GetListMenuLeft(userInfo.AuthorizedMenus, Menulv1[0].MenuID));              
              
                Menulv2 = menuLeft.Where(x => x.MenuLevel == 2).ToList();
                Menulv3 = menuLeft.Where(x => x.MenuLevel == 3).ToList();
                MenuActivefirst = menuLeft.Where(x => x.MenuPath.Contains("/CB?bid=")).ToList();
                // Check xem list menuleft có menu notice không
                // Nếu có thì menuActivefirst = menu notice
                if (MenuActivefirst.Count > 0)
                {
                    MenuActivefirst.ForEach(x => x.MenuIDActivefirst = MenuActivefirst[0].MenuID);
                    MenuActivefirst.ForEach(x => x.MenuPathActivefirst = MenuActivefirst[0].MenuPath);
                    MenuActivefirst.ForEach(x => x.MenuNameActivefirst = MenuActivefirst[0].MenuNameEng);
                    return Json(MenuActivefirst);
                }
                // Nếu không có menu notice
                // Xét tiếp menu đầu tiên chắc chắn là menu lv 2                           
                else
                {
                    // Kiểm tra xem Menulv2 hiện tại có con không
                    // Nếu có thì focus đến menu con hiện tại (là menulv3)
                    MenuActivefirst = Menulv3.Where(x => x.MenuParentID == Menulv2[0].MenuID).ToList();
                    if (MenuActivefirst.Count > 0)
                    {
                        MenuActivefirst.ForEach(x => x.MenuIDActivefirst = MenuActivefirst[0].MenuID);
                        MenuActivefirst.ForEach(x => x.MenuPathActivefirst = MenuActivefirst[0].MenuPath);
                        MenuActivefirst.ForEach(x => x.MenuNameActivefirst = MenuActivefirst[0].MenuNameEng);
                        return Json(MenuActivefirst);
                    }
                    // Ngược lại nếu menu lv2 hiện tại không có con
                    // Thì mở menulv2 hiện tại
                    else
                    {
                        // Nếu menu lv 2 không có con
                        MenuActivefirst = menuLeft.Where(x => x.MenuLevel == 2).ToList();
                        MenuActivefirst.ForEach(x => x.MenuIDActivefirst = Menulv2[0].MenuID);
                        MenuActivefirst.ForEach(x => x.MenuPathActivefirst = Menulv2[0].MenuPath);
                        MenuActivefirst.ForEach(x => x.MenuNameActivefirst = Menulv2[0].MenuNameEng);
                        return Json(MenuActivefirst);
                    }
                }             
            }
            else
            {

                MenuActivefirst.ForEach(x => x.MenuIDActivefirst = default);
                MenuActivefirst.ForEach(x => x.MenuPathActivefirst = default);
                MenuActivefirst.ForEach(x => x.MenuNameActivefirst = default);
                return Json(MenuActivefirst);
            }

        }
        public List<SYMenu> GetListMenuLeft(List<SYMenu> data, int menuParentID)
        {
            var result = data.Where(x => x.MenuParentID == menuParentID).ToList();

            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result.AddRange(GetListMenuLeft(data, result[i].MenuID));
                }
            }

            return result;
        }

        #endregion
        #region "search menu left"
        [HttpGet]
        public IActionResult LoadLeftMenuComponentSearch(string menuName, int menuID)
        {
            var userInfo = HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            //bao add
            if (userInfo == null)
            {
                return NotFound();
            }
            /////
            var siteSetting = HttpContext.Session.Get<SYSite>("SiteInfo");

            var menuLeft = new List<SYMenu>();
            var menuLeftsearchname = new List<SYMenu>();
            if (userInfo != null)
            {
                if (menuName != null)
                {
                    menuLeft = userInfo.AuthorizedMenus.Where(x => x.MenuDesc.ToLower().Contains(menuName.ToLower())).ToList();
                    //menuLeft.AddRange(GetListMenuLeftSearch(userInfo.AuthorizedMenus, menuName));
                }
                else
                {
                    if (userInfo != null)
                    {
                        menuLeft = userInfo.AuthorizedMenus.Where(x => x.MenuID == menuID).ToList();
                        menuLeft.AddRange(GetListMenuLeft(userInfo.AuthorizedMenus, menuID));


                    }
                }



            }

            SetSessionMenu(menuLeft);


            return ViewComponent(siteSetting.SideMenuComponentName == null ? "LeftMenu" : siteSetting.SideMenuComponentName, menuLeft);
        }
        public List<SYMenu> GetListMenuLeftSearch(List<SYMenu> data, string menuName)
        {
            var result = data.Where(x => x.MenuDesc.ToLower().Contains(menuName.ToLower())).ToList();

            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result.AddRange(GetListMenuLeft(data, result[i].MenuID));
                }
            }

            return result;
        }
        #endregion

        #region "Insert - Update - Delete"

        private void SetSessionMenu(List<SYMenu> listMenu)
        {
            HttpContext.Session.Set<List<SYMenu>>("MenuLeft", listMenu);
        }

        #endregion
    }
}

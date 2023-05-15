using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace InfrastructureCore.Authorization
{
    /// <summary>
    /// Authorize Attribute to check authorization
    /// </summary>
    public class CustomAuthorization : AuthorizeAttribute, IAuthorizationFilter
    {
 
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            SYLoggedUser info = filterContext.HttpContext.Session.Get<SYLoggedUser>("UserInfo");
            var currentUrl = filterContext.HttpContext.Request.Path + filterContext.HttpContext.Request.QueryString;
            if (info != null)
            {
                if (currentUrl.Contains("/Site/SiteDetail?siteID="))
                {
                    var existMenu = info.AuthorizedMenus.Where(m => m.MenuPath == "/Site/SiteManagement").ToList();
                    if (existMenu.Count == 0)
                    {
                        filterContext.Result = new RedirectToActionResult("Error", "Home", new { ReturnUrl = "" });
                    }
                }
                else
                {
                    var listMenu = info.AuthorizedMenus.Where(m => m.MenuPath == currentUrl).ToList();
                    if (listMenu.Count == 0)
                    {
                        filterContext.Result = new RedirectToActionResult("Error", "Home", new { ReturnUrl = "" });
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectToActionResult("Error", "Home", new { ReturnUrl = "" });
            }
        }
    }
}

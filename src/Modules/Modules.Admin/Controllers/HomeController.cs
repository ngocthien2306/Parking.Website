using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Modules.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        
        public HomeController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult OnGetSetCultureCookie(string cltr, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cltr)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
                );
            Response.Cookies.Append("langname", cltr,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });
           // var culture = System.Globalization.CultureInfo.CurrentCulture.Name;
            returnUrl = String.Format("/{0}/MESHome", cltr);
            return LocalRedirect(returnUrl);
        }
    }
}

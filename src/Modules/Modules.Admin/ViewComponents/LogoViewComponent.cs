using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Site;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Modules.Admin.ViewComponents
{
    public class LogoViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var siteSetting = HttpContext.Session.Get<SYSite>("SiteInfo");

            return View(siteSetting);
        }
    }
}

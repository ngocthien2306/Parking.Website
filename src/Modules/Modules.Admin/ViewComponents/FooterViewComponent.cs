using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Site;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Modules.Admin.ViewComponents
{
    public class FooterViewComponent: ViewComponent
    {
        public FooterViewComponent()
        {
        }

        public virtual async Task<IViewComponentResult> InvokeAsync(string aaa,string bbb)
        {
            var siteSetting = HttpContext.Session.Get<SYSite>("SiteInfo");
            return View(siteSetting);
        }
    }
}

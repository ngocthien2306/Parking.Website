using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Modules.Admin.ViewComponents
{
    public class UserLoginViewComponent: ViewComponent
    {
        public UserLoginViewComponent()
        {

        }

        public virtual async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = HttpContext.Session.Get<SYLoggedUser>("UserInfo");

            return View(currentUser);
        }
    }
}

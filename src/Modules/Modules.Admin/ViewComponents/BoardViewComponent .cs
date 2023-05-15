using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using InfrastructureCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HLFXFramework.WebHost.ViewComponents
{
    public class BoardViewComponent : ViewComponent
    {
        public BoardViewComponent()
        {
         
        }

        public async Task<IViewComponentResult> InvokeAsync() { 
            return null;              
        }

    }
}

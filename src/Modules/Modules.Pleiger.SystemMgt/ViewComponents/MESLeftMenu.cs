using HLFXFramework.WebHost.ViewComponents;
using InfrastructureCore.Models.Menu;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Pleiger.SystemMgt.ViewComponents
{
    public class MESLeftMenu : LeftMenuViewComponent
    {
        public override async Task<IViewComponentResult> InvokeAsync(List<SYMenu> model)
        {
            return View(model);
        }
    }
}

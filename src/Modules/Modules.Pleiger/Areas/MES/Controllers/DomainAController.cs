using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Modules.Pleiger.Areas.MES.Controllers
{
    [Area("MES")]
    public class DomainAController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

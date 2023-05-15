using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Modules.Pleiger.Controllers
{
    public class DesignPageController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Thread = Guid.NewGuid().ToString("N");
            return View();
        }
    }
}

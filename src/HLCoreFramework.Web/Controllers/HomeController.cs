using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HLCoreFramework.Web.Models;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web.Services.IService;

namespace HLCoreFramework.Web.Controllers
{
    public class HomeController : Controller
    {
        private IUserService userService;

        public HomeController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Login()
        {
            ValidateResult validateResult = this.userService.Validate("S0001", "minh", "minh");

            if (validateResult.Success)
                this.userService.SignIn(this.HttpContext, validateResult.User, false);

            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            this.userService.SignOut(this.HttpContext);
            return this.RedirectToAction("Index");
        }


    }
}

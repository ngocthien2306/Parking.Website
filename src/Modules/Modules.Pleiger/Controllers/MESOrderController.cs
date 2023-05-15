using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Modules.Pleiger.Controllers
{
    public class MESOrderController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;

        public MESOrderController(IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        #region "Get Data"

        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region "Insert - Update - Delete"

        #endregion
    }
}

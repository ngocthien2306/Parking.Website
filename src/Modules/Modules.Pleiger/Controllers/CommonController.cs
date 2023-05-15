using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;

namespace Modules.Pleiger.Controllers
{
    public class CommonController : BaseController
    {
        private readonly ICommonService _commonService;
        private readonly IHttpContextAccessor _contextAccessor;

        #region Constructor
        public CommonController(ICommonService commonService, IHttpContextAccessor contextAccessor) : base(contextAccessor)
        {
            this._commonService = commonService;
            this._contextAccessor = contextAccessor;
        }
        #endregion

        public IActionResult GetAllComCodeDTL(string Lang)
        {
            var result = _commonService.GetAllComCodeDTL(Lang);
            return Content(JsonConvert.SerializeObject(result));
        }

        public IActionResult GetAllItem(string Lang)
        {
            var result = _commonService.GetAllItem(Lang);
            return Content(JsonConvert.SerializeObject(result));
        }

        public IActionResult GetItemRaw(string Lang)
        {
            var result = _commonService.GetItemRaw(Lang);
            return Content(JsonConvert.SerializeObject(result));
        }
    }
}

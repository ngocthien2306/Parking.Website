using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfrastructureCore;
using InfrastructureCore.DataAccess;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Services.IService;

namespace Modules.Pleiger.Controllers
{
    public class PleigerValidateController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private IMESWarehouseService _mesWarehouseService;

        public PleigerValidateController(IHttpContextAccessor contextAccessor, IMESWarehouseService mesWarehouseService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._mesWarehouseService = mesWarehouseService;
        }

        [HttpPost]
        public JsonResult CheckWarehouseCode(string Key, string Value)
        {
            var input = !string.IsNullOrEmpty(Value) ? Value : "";
            bool isValid = _mesWarehouseService.WarehouseValidateDuplicate(input);
            //string message = isValid ? "Warehouse code is existed." : "";
            Result result = new Result();
            result.Success = isValid;
            result.Message = isValid ? "" : "창고코드가 중복 값입니다. 확인 후 다시 등록 해 주십시오.";
            //return Json(new { result.Success, result.Message });
            return Json(isValid);
        }
    }
}

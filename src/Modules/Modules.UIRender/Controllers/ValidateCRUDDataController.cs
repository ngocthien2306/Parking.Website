using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.CommonModels;
using Modules.UIRender.Services.IService;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Modules.UIRender.Controllers
{
    public class ValidateCRUDDataController : BaseController
    {
        #region Properties

        private IValidateDataService _validateDataService;
        private readonly IHttpContextAccessor _contextAccessor;

        #endregion

        #region Constructor
        public ValidateCRUDDataController(IHttpContextAccessor contextAccessor, IValidateDataService validateDataService) : base(contextAccessor)
        {
            this._validateDataService = validateDataService;
            this._contextAccessor = contextAccessor;
        }

        #endregion

        public IActionResult WarehouseValidateCRUD(string validateData)
        {
            var model = JsonConvert.DeserializeObject<List<MES_Warehouse>>(validateData);

            var result = _validateDataService.ValidateWarehouseData(model);

            return Json(new { result = result });
        }

        public IActionResult ItemPartnerValidateCRUD(string validateData)
        {
            var model = JsonConvert.DeserializeObject<List<MES_ItemPartner>>(validateData);

            var result = _validateDataService.ValidateItemPartnerData(model);

            return Json(new { result = result });
        }
    }
}

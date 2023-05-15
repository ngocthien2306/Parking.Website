using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.FileUpload.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Create User: Minh Vu
/// Create Day: 2020-08-05
/// </summary>
namespace Modules.Pleiger.Controllers
{
    public class MESWarehouseController : BaseController
    {
        private readonly IHttpContextAccessor contextAccessor;
        private IMESWarehouseService _mesWarehouseService;
        private readonly IMESComCodeService _mesComCodeService;

        public MESWarehouseController(IHttpContextAccessor contextAccessor, IMESWarehouseService mesWarehouseService, IMESComCodeService mesComCodeService) : base(contextAccessor)
        {
            this.contextAccessor = contextAccessor;
            this._mesWarehouseService = mesWarehouseService;
            this._mesComCodeService = mesComCodeService;
        }

        #region Get Data
        
        [HttpGet]
        public object GetAllPleigerWarehouse(DataSourceLoadOptions loadOptions)
        {
            var lstType = _mesWarehouseService.GetAllPleigerWarehouse();
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetAllPleigerMaterialWarehouse(DataSourceLoadOptions loadOptions)
        {
            var lstType = _mesWarehouseService.GetAllPleigerMaterialWarehouse();
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        
        [HttpGet]
        public object GetAllPleigerFinishProductWarehouse(DataSourceLoadOptions loadOptions)
        {
            var lstType = _mesWarehouseService.GetAllPleigerFinishProductWarehouse();
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        
        [HttpGet]
        public object GetAllPartnerWarehouse(DataSourceLoadOptions loadOptions, string PartnerCode)
        {
            var lstType = _mesWarehouseService.GetAllPartnerWarehouse(PartnerCode);
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        #endregion

    }
}

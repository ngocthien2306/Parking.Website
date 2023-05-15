using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Inventory.Services.IService;
using Modules.Pleiger.MasterData.Services.IService;

/// <summary>
/// Create User: Minh Vu
/// Create Day: 2020-08-05
/// </summary>
namespace Modules.Pleiger.Inventory.Controllers
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
        public object GetAllPleigerWH(DataSourceLoadOptions loadOptions)
        {
            var lstType = _mesWarehouseService.GetAllPleigerWH();
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        [HttpGet]
        public object GetAllPleigerFinishProductWarehouse(DataSourceLoadOptions loadOptions)
        {
            var lstType = _mesWarehouseService.GetAllPleigerFinishProductWarehouse();
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        /// <summary>
        /// Defective warehouse: Kho hàng bị lỗi
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetAllPleigerDefectiveWarehouse(DataSourceLoadOptions loadOptions)
        {
            var lstType = _mesWarehouseService.GetAllPleigerDefectiveWarehouse();
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        
        [HttpGet]
        public object GetAllPartnerWarehouse(DataSourceLoadOptions loadOptions, string PartnerCode)
        {
            var lstType = _mesWarehouseService.GetAllPartnerWarehouse(PartnerCode);
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetAllPartnerMaterialWarehouse(DataSourceLoadOptions loadOptions, string PartnerCode)
        {
            var lstType = _mesWarehouseService.GetAllPartnerMaterialWarehouse(PartnerCode);
            return DataSourceLoader.Load(lstType, loadOptions);
        }

        [HttpGet]
        public object GetAllPartnerFinishWarehouse(DataSourceLoadOptions loadOptions, string PartnerCode)
        {
            var lstType = _mesWarehouseService.GetAllPartnerFinishWarehouse(PartnerCode);
            return DataSourceLoader.Load(lstType, loadOptions);
        }
        #endregion

    }
}

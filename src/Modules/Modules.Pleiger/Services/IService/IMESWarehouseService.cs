using Modules.Admin.Models;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESWarehouseService
    {
        #region Warehouse
        bool WarehouseValidateDuplicate(string WarehouseCode);
        List<DynamicCombobox> GetAllPleigerWarehouse();
        List<DynamicCombobox> GetAllPleigerMaterialWarehouse();
        List<DynamicCombobox> GetAllPleigerFinishProductWarehouse();
        List<DynamicCombobox> GetAllPartnerWarehouse(string PartnerCode);
        List<MES_Warehouse> GetListWareHouseByType(string WareHouseType);
        #endregion
    }
}

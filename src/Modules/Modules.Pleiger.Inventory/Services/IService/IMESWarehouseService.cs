using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System.Collections.Generic;

namespace Modules.Pleiger.Inventory.Services.IService
{
    public interface IMESWarehouseService
    {
        #region Warehouse
        bool WarehouseValidateDuplicate(string WarehouseCode);
        List<DynamicCombobox> GetAllPleigerWarehouse();
        List<DynamicCombobox> GetAllPleigerMaterialWarehouse();
        List<DynamicCombobox> GetAllPleigerWH();
        
        List<DynamicCombobox> GetAllPleigerFinishProductWarehouse();
        List<DynamicCombobox> GetAllPleigerDefectiveWarehouse();
        List<DynamicCombobox> GetAllPartnerWarehouse(string PartnerCode);
        List<DynamicCombobox> GetAllPartnerMaterialWarehouse(string PartnerCode);
        List<DynamicCombobox> GetAllPartnerFinishWarehouse(string PartnerCode);
        List<MES_Warehouse> GetListWareHouseByType(string WareHouseType);
        List<MES_Warehouse> GetListWareHouseInternal();//Get GetListWareHouseInternal not Defective warehouse

        #endregion
    }
}

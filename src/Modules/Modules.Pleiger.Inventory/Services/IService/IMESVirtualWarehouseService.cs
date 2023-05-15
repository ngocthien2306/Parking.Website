using InfrastructureCore;
using Modules.Pleiger.CommonModels;
using System.Collections.Generic;

namespace Modules.Pleiger.Inventory.Services.IService
{
    public interface IMESVirtualWarehouseService
    {
        List<MES_VirtualWarehouse> GetListAllData();

        List<MES_VirtualWarehouse> SearchVirtualWarehouse(MES_VirtualWarehouse model);

        Result SaveVirtualWarehouse(MES_VirtualWarehouse model, string userModify, string type);
        MES_VirtualWarehouse GetDataDetail(string Id);
        Result DeleteVirtualWarehouse(string[] VirtualWareHouseId);
        MES_VirtualWarehouse GetVirtualWareHousesDetailByID(string Id); 
    }
}

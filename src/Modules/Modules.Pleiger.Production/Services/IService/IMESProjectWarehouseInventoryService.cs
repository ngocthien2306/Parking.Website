using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESProjectWarehouseInventoryService
    {
        List<MESProjectWarehouseInventory> SearchProjectWarehouseInventory(string warehouseName, string productionProjectCode, string category);

        List<MESProjectWarehouseInventory> SearchProjectWarehouseInventoryNew(string warehouseName, string productionProjectCode, string category,string SalesOrderProjectCode);

        List<MES_ComWareHouseInventory> GetComBoBoxWareHouseInventory();
        List<MESProjectWarehouseInventory> GetProjectWarehouseInventoryDetail(string ProjectCode, string WarehouseCode);

        List<MESProjectWarehouseInventory> GetProjectWarehouseInventoryListDetail(string listWarehouseInventory);

        

    }
}

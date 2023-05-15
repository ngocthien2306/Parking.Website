using Modules.Pleiger.CommonModels;
using System.Collections.Generic;
using System.Data;

namespace Modules.Pleiger.Inventory.Services.IService
{
    public interface IMESWHInventoryStatusService
    {
        #region "Master"
        List<MES_WHInventoryStatus> GetAllWHInventory(MES_WHInventoryStatus model, int pageSize, int itemSkip);
        List<MES_WHInventoryStatus> GetAllWHInventoryToExport(string WarehouseCode, string Category, string ItemName);
        int CountAllWHInventory(MES_WHInventoryStatus model);
        string ExportWHInventoryExcelFile(DataTable dt);

        #endregion

    }
}

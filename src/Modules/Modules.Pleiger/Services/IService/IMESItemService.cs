using Modules.Admin.Models;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESItemService
    {
        MES_Item GetDetail(string itemCode);
        List<MES_Item> GetListData(); 
        List<MES_Item> GetListMaterialByItemClassCode(string ItemClassCode); 
        List<MES_Item> GetListFinishItem();

        List<MES_Item> GetListData(int pageSize, int itemSkip);

        List<MES_Item> GetItemCodeClassByCategory();
        bool IsItemCodeMstExisted(string itemCode);
        List<MES_Item> getItemsByItemClassCode(string itemClassCode);
        List<MES_Item> GetItemsByCategoryCode(string categoryCode);
        List<MES_Item> GetListItemMaterialNotexist(string projectCode);
        List<MES_Item> GetItemInWareHouse(string WareHouseCode);
        List<MES_Item> GetItemInWareHousebyItemCode(string WareHouseCode, string ItemCode);
        List<MES_Item> SearchItemInWareHouse(string WareHouseCode, string ItemCode, string ItemName);








    }
}

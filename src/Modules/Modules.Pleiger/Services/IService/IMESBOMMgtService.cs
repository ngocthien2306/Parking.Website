using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESBOMMgtService
    {
        List<MES_BOM> GetItemFinish();
        List<MES_BOM> GetDataSearch(string ItemCode, string ParentItemLevel);
        List<MES_BOM> SearchItemPopup(string categoryCode, string itemClassCode, string itemName, string itemCode);
        List<MES_BOM> InsertParentItem(MES_BOM mesBOM);
        List<MES_Item> GetListItem(string ItemClassCode, string categoryCode);
        Result InsertBOMItems(List<MES_BOM> InsertArr, List<MES_BOM> UpdateArr, List<MES_BOM> DeleteArr);
        List<MES_ItemClass> LoadItemClass();
        List<MES_BOM> SearchItemPopupGetOther(string categoryCode, string itemClassCode, string itemName, string itemCode);
        Result InsertBOMTreeJSON(string InsertArrFromGetOther);
        List<ItemRequest> GetBOMItemBySalePJCode(string PJCode);
    }
}

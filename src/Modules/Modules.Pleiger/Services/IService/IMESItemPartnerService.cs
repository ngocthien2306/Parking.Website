using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESItemPartnerService
    {
        List<MES_ItemPartner> GetListPartnerByItem(string itemCode);
        MES_ItemPartner GetItemDetail(string itemCode, string projectCode);
        List<MES_ItemPartner> GetListItemSupply(string partnerCode);
        List<MES_ItemPartner> SearchItemByPartner(string partnerCode,string itemName ,string itemCode);

        MES_ItemPartner GetItemPartnerByParams(string partnerCode, string itemCode);
        object getListMonetaryUnit();
    }
}

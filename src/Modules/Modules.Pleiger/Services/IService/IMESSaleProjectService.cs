using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IMESSaleProjectService
    {
        List<MES_SaleProject> GetListData();
        List<MES_SaleProject> GetListAllData(); 
        List<DynamicCombobox> GetProjectStatus();
        List<MES_SaleProject> GetUserProjectCode();
        MES_SaleProject GetDataDetail(string projectCode);
        List<ItemRequest> GetListItemRequest(string projectCode);
        List<MES_ItemPO> GetListItemPO(string projectCode);
        Result SaveDataProductionRequest(MES_SaleProject model, string listItemRequest, string listItemPO);
        Result SaveSalesProject(MES_SaleProject model, string modifiedUser);
        Result DeleteSalesProject(string projectCode); 
        Result DeleteSalesProjects(string projectCode);
        List<MES_SaleProject> SearchSaleProject(MES_SaleProject model);
        MES_SaleProject CheckDuplicate(string DuplicateCode,string Type);
        //bao add
        List<MES_SaleProject> GetListProjectCodeByStatus( );

    }
}

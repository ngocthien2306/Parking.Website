using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using System.Collections.Generic;
using System.Data;

namespace Modules.Pleiger.SalesProject.Services.IService
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
        object SearchSaleProject(MES_SaleProject model, string check);
        List<MES_SaleProject> SearchSaleProjectsExcel(MES_SaleProject model, string check);
        object SearchSaleProjectsExcel1(MES_SaleProject model, string check);
        MES_SaleProject CheckDuplicate(string DuplicateCode,string Type);
        //bao add
        List<MES_SaleProject> GetListProjectCodeByStatus();
        string ExportExcelICube(DataTable dt);
        List<MES_SaleProjectExcelInfor> GetDataExportExcelICube(string jsonObj);

        List<SYFileUpload> GetSYFileUploadByID(string fileGuid);

        List<MES_UrlByUser> GetListFileUrlSaleProject(string code);

        Result SaveListFile(List<MES_UrlByUser> file, string code);


        Result SaveUrlFile(int Id, string saleProjectID, string fileUrl, int flag);
    }
}

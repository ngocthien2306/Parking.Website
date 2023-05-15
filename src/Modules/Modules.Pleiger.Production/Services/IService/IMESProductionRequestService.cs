using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System.Collections.Generic;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESProductionRequestService
    {
        List<MES_SaleProject> GetListData();
        List<MES_SaleProject> SearchListProductionRequest(MES_SaleProject saleProject ,string UserCode, string checkCode, string StartDate, string EndDate);
        MES_SaleProject GetDataDetail(string projectCode);
        List<ItemRequest> GetListItemRequest(string projectCode);
        List<MES_ItemPO> GetListItemPO(string projectCode);
        List<DynamicCombobox> GetComboboxProjectName();
        bool CheckEnoughItemQty(string projectCode);
        Result SaveDataProductionRequest(MES_SaleProject model, string listItemRequest, string listItemPO);
        Result SendRequestProduction(string projectCode);
        ItemRequest GetListItemRequestDetail(string projectCode);

        // Quan add 2020/09/11
        // Save data roductionRequest
        Result SaveDataProductionRequestChange(
            string projectCode,
            string requestCode,
            string requestType,
            string userIDRequest,
            string requestDate,
            string requestMessage,
            List<ItemRequest> listDataSave);
        Result UpdateDataProductionRequestChange(
           string projectCode,
           string requestCode,
           string requestType,
           string userIDRequest,
           string requestDate,
           string requestMessage, 
           string ProductType, 
           string ItemCode,
           bool InitialCode);
        Result RequestProduction(string projectCode, string UserID, string requestDate, string requestType);
        Result DeleteGridItemPartList(string ItemCode, string RequestCode);
        Result DeleteBatchRowItem(string ListItem, string RequestCode);

        
        Result RecallProductionRequest(string projectCode, string UserID);
        #region Functions For New PO UI

        List<MES_ComCodeDtls> GetListCategories();
        List<MES_ItemClass> GetListItemClass(string categoryCode);
        //List<MES_Item> GetListItemOfProject(string ProjectCode, string Category, string ItemClassCode, string ItemCode);
        List<ItemRequest> GetListItemOfProject(string ProjectCode, string ItemCode);

        //duy add
        List<ItemRequest> GetListItemOfItemClassCode(string Category, string ItemClassCode, string ItemCode,string ItemName);
        List<ItemRequest> GetListItemFromQrScanning(string ItemClassCode, string ItemCode);

        List<MES_SaleProject> GetItemByProjectCodeInStatus();
        List<MES_SaleProject> GetProjectGoodsDelivery();

        #endregion
        List<MES_ProjectProdcnLines> GetListProductionLine(string projectCode);

        int CheckPlanRequestQty(string projectCode);

    }

}

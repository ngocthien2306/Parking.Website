using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;

namespace Modules.Pleiger.Transaction.Services.IService
{
    public interface IMESItemSlipService
    {
        #region "MES ItemSlip Master"

        List<MES_ItemSlipMaster> GetListMESItemSlipMaster(string startDate, string endDate, string status, string userProjectNo, string userPoNumber, string goodReceipt);
        
        #endregion

        #region "MES ItemSlip Detail"
        List<MES_ItemSlipDetail> GetListMESItemSlipDetail(string slipNumber, string poNumber);
        List<MES_ItemSlipDetail> GetListMESItemSlipDetailForReleasePartner(string slipNumber, string poNumber);
        
        List<MES_ItemSlipDetail> GetItemSlipDetailInPopup(string poNumber);    
        
        List<Combobox> GetPONumberForRelease();
        List<MES_ItemSlipMaster> GetPOAllNumberForRelease();
        List<MES_ItemSlipMaster> GetPOAllNumberSearch(string UserPONumber,string PartnerName);
        MES_PORequest GetDataReferByPONumberForRelease(string poNumber);
        List<MES_ItemPO> GetItemSlipDetailInPopupSearchPO(string poNumber);
        #endregion
        string GetItemSlipMasterKey();

        List<MES_ItemSlipDetail> CreateGridItemSlipDtlByPONumber(string poNumber);
        List<MES_ItemSlipDetail> CreateGridItemSlipDtlByPONumberPartner(string poNumber);
        

        #region "Insert - Update - Delete release item from Partner to Pleiger"
        Result SaveDataMaterialInStock(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify);
        Result DeleteItemSlip(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl, string userModify);
        Result DeleteItemSlipDetail(MES_ItemSlipDetail dataDtl);
        Result UpdateSlipDate(string SlipNumber, DateTime? SlipDate);
        #endregion


        #region  Get Data ItemSlip Project have production complete  - prepare item deliver from Pleiger to Partner
        List<MES_SaleProject> GetListProjectPrepareDelivery(string startDate, string endDate, string status, string projectNo, string itemCodeSearch, string itemNameSearch, string prodcnCodeSearch, string projectOrderType, string saleOrderProjectName, string userProjectCode);
        List<MES_SaleProject> ProjectPrepareDeliveryDataGridMasterDetailView(string projectNo);

        List<Combobox> GetCustomerPartnerCode(string projectNo);
        List<Combobox> GetWarehousePGItem(string ItemCode);
        List<MES_ItemSlipDetail> GetQtyWarehousePGItem(string ItemCode);
        List<MES_Warehouse> GetWarehouseOfPartner();

        List<MES_ItemSlipDelivery> GetListSlipDelivery(string projectCode);

        #endregion
        #region "Insert - Update - Delete - item deliver from Pleiger to Partner"
        Result SaveDataDeliveryOutStock(string flag, List<MES_ItemSlipDelivery> slipDetails, string userModify);
        Result DeleteItemSlipDelivery(List<MES_ItemSlipDelivery> data, string userModify);
        #endregion

        #region "MES ItemSlip Moving Stock"

        List<MES_ItemSlipMaster> GetListMovingStockItem(string startDate, string endDate, string status, string fromWH, string toWH);
        List<MES_ItemSlipDetail> GetListMovingStockItemDetail(string slipNumber);
        Result SaveMovingStockItem(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify);
        Result DeleteMovingStockItem(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl, string userModify);
        #endregion

        #region Form search 
        List<MES_SaleProject> GetProjectNameInProduction();
        #endregion

        #region Goods Return PO
        List<MES_ItemSlipMaster> GetItemSlipMasterGoodsReturnPO(string startDate, string endDate, string status, string partnerCode,string userProjectCode, string userPoNumber);
        List<Combobox> GetPONumberHaveReceipt();
        List<MES_ItemSlipDetail> CreateGridItemSlipDtlByPONumberInGoodsReturn(string poNumber);
        Result SaveDataGoodsReturn(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify);
        Result DeleteDataGoodsReturn(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl, string userModify);
        List<MES_ItemSlipDetail> GetListMESItemSlipDetailGoodsReturn(string slipNumber, string poNumber);
        List<MES_ItemSlipDetail> GetListMESItemSlipDetailGoodsReturnDelivery(string slipNumber, string poNumber,string slipNumberGoodReceipt);
        
        List<MES_ItemSlipMaster> GetPONumberHaveReceiptSearch(string userPONumber, string partnerName);

        #endregion

        #region Goods Return Delivery
        List<MES_ItemSlipMaster> GetItemSlipMasterGoodsReturnDelivery(string startDate, string endDate, string status, string partnerCode,string userProjectCode);
        List<MES_ItemSlipMaster> GetProjectHaveGoodsIssuesSearch(string userProjectCode, string partnerName);
        List<MES_ItemSlipMaster> GetListGoodsDeliveries(string userProjectCode, string partnerName, string ProjectCode);
        List<MES_ItemSlipDetail> CreateGridItemSlipDtlByProjectInGoodsReturnDelivery(string projectCode,string slipNumber);
        Result SaveDataGoodsReturnDelivery(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify, string transSlipNumber);
        #endregion
        List<MES_ItemSlipDetail> getStockQtyByItemCode(string ItemCode);

        // Slide 40    New menu, This menu is for partner use only.
        // Purchase Order Delivery
        List<MES_ItemSlipMaster> GetListPODeliverybyPartner(string startDate, string endDate, string status, string userProjectNo, string userPoNumber, string UserCode);
        List<MES_ItemSlipMaster> GetPOAllNumberSearchByPartner(string UserPONumber, string PartnerCode);
        List<MES_ItemSlipMaster> GetListMESItemSlipMasterForReleasePartner(string startDate, string endDate, string status, string userProjectNo, string userPoNumber, string loginUser);
        Result SavePartnerPODelivery(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify);
        List<MES_SaleProject> getDetailByProjectCodeList(string listProjectCode);
        List<MES_ItemSlipMaster> getCreatedPO(string userId);
        Result PONotified(string PONumber);
    }
}

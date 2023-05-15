using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modules.Pleiger.PurchaseOrder.Services.IService
{
    public interface IPORequestService
    {
        List<MES_PORequest> GetListData();
        List<MES_PORequest> SearchListPORequest(string projectCode, string poNumber,string UserPONumber,string projectName, string requestDateFrom, string requestDateTo,string partnerCode,string poStatus, string SalesClassification,bool isPartner);
        
         MES_PORequest GetPODetail( string poNumber);
        List<MES_ItemPO> GetListItemPORequest(string poNumber);
        Result SaveDataPORequest(string projectCode, string saveDataType, string requestCode, string poNumber, string partnerCode, string arrivalRequestDate, string listItemPORequest, string userModify,string UserPONumber);

        /// <summary>
        /// Added By PVN
        /// Date Added: 2020-08-31
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        //MES_PORequest GetPOData(string poNumber);
        //List<MES_PORequest> GetPODetailData(string poNumber);
        Task<string> CreatePOPrint(string poNumber);
        Task<string> ExportExcelPO(string poNumber);
        
        List<MES_ItemClass> GetListItemClassCode();
        //
        Result SavePO(string flag,MES_PORequest PORequest, List<MES_ItemPO> listDetails,string UserId);
        Result UpdatePODetailPartner(string flag,MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId);
        Result UpdatePODetailPartnerETA(MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId);
        List<MES_SaleProject> SearchProjectCodeByParams(string itemCode, string itemName,string productionProjectCode,string productionProjectName,string projectType);
        MES_Partner getPartnerByPartnerCode(string partnerCode);
        List<MES_SaleProject> GetProjectCodeByStatus();
        Result DeletePOMst(string PoNumber);
        Result POClose(string PoNumber);
        string GetPOStatus(string PoNumber);      
        Result CheckStatusPOClose(string PoNumber);
        List<MES_PORequest> GetPODataPrint(string poNumber);
        List<MES_PORequest> GetPODetailDataPrint(string poNumber);
        List<DynamicCombobox> GetListPartnerCombobox();
        Result CoppyPO(string PoNumber,string userId);
        Result UpdateRemakAfterConfirmed(string PoNumber, string Remark);

        List<MES_PurchaseDetail> GetListPurcharDetail(string poNumber);
        
        Result UpdateStatusPoRemarkItem(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId);
        Result UpdateStatusPoChangedDayItem(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId);
        Result UpdateHistoryDeliveryDateItem(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserID);

        Result UpdateStatusPartnerToClosePopup(string poNumber, string flag);

        List<MES_HistoryDeliveryItem> GetListHistoryUpdateDeliveryItem(string Ponumber, string ItemCode, string UserID);
    }
}

using InfrastructureCore;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IPORequestService
    {
        List<MES_PORequest> GetListData();
        List<MES_PORequest> SearchListPORequest(string projectCode, string poNumber,string UserPONumber,string UserProjectCode, string requestDateFrom, string requestDateTo,string partnerCode,string poStatus);
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
        System.Threading.Tasks.Task<string> CreatePOPrint(string poNumber);
        List<MES_ItemClass> GetListItemClassCode();
        //
        Result SavePO(string flag,MES_PORequest PORequest, List<MES_ItemPO> listDetails,string UserId);
        Result UpdatePODetailPartner(string flag,MES_PORequest PORequest, List<MES_ItemPO> listDetails);
        List<MES_SaleProject> SearchProjectCodeByParams(string projectCode, string itemCode, string itemName);
        MES_Partner getPartnerByPartnerCode(string partnerCode);
        List<MES_SaleProject> GetProjectCodeByStatus();
        Result DeletePOMst(string PoNumber);

        

    }
}

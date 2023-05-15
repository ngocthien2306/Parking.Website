using InfrastructureCore;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Modules.Pleiger.PurchaseOrder.Services.IService
{
    public interface IPurchaseService
    {
        public List<MES_Purchase> SearchAll(string StartPurchaseDate,
            string EndPurchaseDate, string ItemCode,
            string ItemName, string ProjectCode,
            string PONumber, string PartnerCode,
            string ProjectName, string UserPONumber, string POStatus, string SalesClassification);
        public List<MES_Purchase> GetDataPurchaseOrderList
        (string startDate,string endDate,string userPONumber, string projectName,string poStatus,string itemCode,string itemName,string Remark1,string partnerCode);

        public Result Update_Data_MES_PurchaseOrderList(string flag, List<MES_Purchase> listPurchaseOrder);      
    }
}

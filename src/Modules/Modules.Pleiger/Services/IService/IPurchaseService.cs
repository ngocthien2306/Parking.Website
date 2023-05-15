using InfrastructureCore;
using Modules.Pleiger.Models;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.IService
{
    public interface IPurchaseService
    {
        public List<MES_Purchase> SearchAll(string StartPurchaseDate,
            string EndPurchaseDate, string ItemCode,
            string ItemName, string ProjectCode,
            string PONumber, string PartnerCode,
            string UserProjectCode,string UserPONumber);
        public Result Update_Data_MES_PurchaseOrderList(string flag, List<MES_Purchase> listPurchaseOrder);
    }
}

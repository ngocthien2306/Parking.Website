using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.PurchaseOrder.Services.ServiceImp
{
    public class MESPurchaseOrderDeliveryService : IMESPurchaseOrderDeliveryService
    {
        private const string SP_MES_PURCHASSE_ORDER_DELIVERY = "SP_MES_PURCHASSE_ORDER_DELIVERY";

        public List<MES_PurchaseDeliveryOrder> SearchPurchaseOrderDelivery(string deliveryStart, string deliveryEnd, string userPONumber)
        {
            var result = new List<MES_PurchaseDeliveryOrder>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@PartnerDeliveryStart";
                arrParams[2] = "@PartnerDeliveryEnd";
                arrParams[3] = "@UserPONumber";
                object[] arrParamsValue = new string[4];
                arrParamsValue[0] = "search";
                arrParamsValue[1] = deliveryStart;
                arrParamsValue[2] = deliveryEnd;
                arrParamsValue[3] = userPONumber;
                var data = conn.ExecuteQuery<MES_PurchaseDeliveryOrder>(
                    SP_MES_PURCHASSE_ORDER_DELIVERY, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MES_PurchaseDeliveryOrder> SearchDeliveryDetail(string PONumber)
        {
            var result = new List<MES_PurchaseDeliveryOrder>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "searchDeliveryDetail";
                arrParamsValue[1] = PONumber;
                var data = conn.ExecuteQuery<MES_PurchaseDeliveryOrder>(
                    SP_MES_PURCHASSE_ORDER_DELIVERY, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
    }
}

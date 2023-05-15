using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.PurchaseOrder.Services.IService
{
    public interface IMESPurchaseOrderDeliveryService
    {
        List<MES_PurchaseDeliveryOrder> SearchPurchaseOrderDelivery(string deliveryStart, string deliveryEnd, string userPONumber);
        List<MES_PurchaseDeliveryOrder> SearchDeliveryDetail(string PONumber);
    }
}

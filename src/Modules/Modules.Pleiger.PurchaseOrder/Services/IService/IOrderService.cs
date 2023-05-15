using InfrastructureCore;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace Modules.Pleiger.PurchaseOrder.Services.IService
{
    public interface IOrderService
    {
        List<MES_Purchase> GetOrderItem();

    }
}

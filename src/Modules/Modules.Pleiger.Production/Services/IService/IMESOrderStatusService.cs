using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESOrderStatusService
    {
        List<MES_OrderStatus> searchOrderStatus(MES_OrderStatus orderStatus);
    }
}

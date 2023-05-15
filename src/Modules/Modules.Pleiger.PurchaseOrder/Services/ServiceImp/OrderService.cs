using InfrastructureCore;
using InfrastructureCore.DAL;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;


namespace Modules.Pleiger.PurchaseOrder.Services.ServiceImp
{
    class OrderService : IOrderService
    {
        private string SP_GET_LIST_ORDER_STATUS = "SP_GET_LIST_ORDER_STATUS";
        public List<MES_Purchase> GetOrderItem()
        {
            using(var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParam = new string[1];
                arrParam[0] = "@Method";
                var varArrparam = new string[1];
                varArrparam[0] = "data";
                var result = connect.ExecuteQuery<MES_Purchase>(SP_GET_LIST_ORDER_STATUS, arrParam, varArrparam).ToList();
                return result.ToList();
            }
        }
    }
}

using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    public class MESOrderStatusService : IMESOrderStatusService
    {
        private const string SP_MES_ORDER_STATUS = "SP_MES_SCM_ORDER_STATUS_GET_DATA";

        public List<MES_OrderStatus> searchOrderStatus( MES_OrderStatus orderStatus)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[7];
                    arrParams[0] = "@StartDate";
                    arrParams[1] = "@EndDate";
                    arrParams[2] = "@P_UserPONumber";
                    arrParams[3] = "@P_PleigerRmk";
                    arrParams[4] = "@P_POStatus";
                    arrParams[5] = "@P_ItemName";
                    arrParams[6] = "@Method";


                    object[] arrParamsValue = new object[7];
                    arrParamsValue[0] = orderStatus.StartDate;
                    arrParamsValue[1] = orderStatus.EndDate;
                    arrParamsValue[2] = orderStatus.UserPONumber;
                    arrParamsValue[3] = orderStatus.PleigerRemark;
                    arrParamsValue[4] = orderStatus.POStatus;
                    arrParamsValue[5] = orderStatus.ItemName;
                    arrParamsValue[6] = "searchOrderStatus";

                    var result = conn.ExecuteQuery<MES_OrderStatus>(SP_MES_ORDER_STATUS, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}

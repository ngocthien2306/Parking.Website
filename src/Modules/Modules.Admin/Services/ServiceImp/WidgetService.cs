
using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Admin.Services.ServiceImp
{
    public class WidgetService : IWidgetService
    {
        public const string SP_Web_SYWidgetMst = "SP_Web_SYWidgetMst";
        public const string SP_Web_SYWidgetDtl = "SP_Web_SYWidgetDtl";

        public List<SYWidgetElementDetail> GetAllWidgetDtl()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetAllWidgetDtl";
                var result = conn.ExecuteQuery<SYWidgetElementDetail>(SP_Web_SYWidgetDtl, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        public List<SYWidgetElement> GetAllWidgetMst()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "SelectAll";
                var result = conn.ExecuteQuery<SYWidgetElement>(SP_Web_SYWidgetMst, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        /// <summary>
        /// Get detail Widget
        /// </summary>
        /// <param name="widgetNumber"></param>
        /// <returns></returns>
        public List<SYWidgetElementDetail> GetWidgetDtl(string widgetNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@WidgetNumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetWidgetDtl";
                arrParamsValue[1] = widgetNumber;
                var result = conn.ExecuteQuery<SYWidgetElementDetail>(SP_Web_SYWidgetDtl, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
namespace Modules.Pleiger.Services.ServiceImp
{
    public class ChartService : IChartService
    {
        private readonly string SP_MES_CHARTDATA = "SP_MES_CHARTDATA ";
        public List<ChartDataViewModel> GetDataChartByMonth()
        {
            var result = new List<ChartDataViewModel>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetProducedQtyPlannedQty30DayInMonth";
                var data = conn.ExecuteQuery<ChartDataViewModel>(SP_MES_CHARTDATA, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<ChartDataViewModel> GetDataChart12MonthsInThisYear()
        {
            var result = new List<ChartDataViewModel>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetProducedQtyPlannedQtyInThisYear";
                var data = conn.ExecuteQuery<ChartDataViewModel>(SP_MES_CHARTDATA, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<ChartDataProdcnLineCodeViewModel> GetDataChartProdcnLineCode()
        {
            var result = new List<ChartDataProdcnLineCodeViewModel>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetDataProdcnLine";
                var data = conn.ExecuteQuery<ChartDataProdcnLineCodeViewModel>(SP_MES_CHARTDATA, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<ChartDataViewModel> GetProducedQtyPlannedQty4WeeksInMonth()
        {
            // GetProducedQtyPlannedQty4WeeksInMonth
            var result = new List<ChartDataViewModel>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetProducedQtyPlannedQty4WeeksInMonth";
                var data = conn.ExecuteQuery<ChartDataViewModel>(SP_MES_CHARTDATA, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
    }
}

using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class CompanyService : ICompanyService
    {
        private const string SP_Name = "SP_Web_SYCompanyInformation";
        // Get list User Group access Menu
        public List<SYCompanyInformation> GetListCompanyInfor()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "Select";
                var result = conn.ExecuteQuery<SYCompanyInformation>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        public SYCompanyInformation GetCompanyInforByID(int BusinessNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@BusinessNumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "SelectByID";
                arrParamsValue[1] = BusinessNumber;
                var result = conn.ExecuteQuery<SYCompanyInformation>(SP_Name, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }
    }
}

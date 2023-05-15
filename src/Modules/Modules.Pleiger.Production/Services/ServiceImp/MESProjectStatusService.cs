using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    class MESProjectStatusService : IMESProjectStatusService
    {
        private const string SP_GET_COMBOBOX_VALUE_ITEMCLASS = "SP_GET_COMBOBOX_VALUE_ITEMCLASS";
        private const string SP_MES_PROJECT_STATUS_GET_DATA = "SP_MES_PROJECT_STATUS_GET_DATA";

        public List<MES_SaleProject> getMESSaleProject()
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                //GetAllProjectStatusData
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new string[1];
                arrParamsValue[0] = "SearchProjectStatusData"; // Dung chung method voi search nhung ko truyen tham so
                var data = conn.ExecuteQuery<MES_SaleProject>(
                    SP_MES_PROJECT_STATUS_GET_DATA, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MES_SaleProject> searchMESSaleProject(string projectOrderType, string saleOrderProjectCode, string projectStatus, string productType, string projectName, string userProjectCode, string salesClassification, string Userlogin,string checkCode)
        {
            string check = "";
            if (checkCode == "Yes")
            {
                check = "1";
            }
            else if (checkCode == "No")
            {
                check = "0";
            }
            else
            {
                check = null;
            }
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[10];
                arrParams[0] = "@Method";
                arrParams[1] = "@P_ProjectStatus";
                arrParams[2] = "@P_ProductType";
                arrParams[3] = "@P_ProjectName";
                arrParams[4] = "@P_UserProjectCode";
                arrParams[5] = "@P_SalesClassification";
                arrParams[6] = "@P_ProjectOrderType";
                arrParams[7] = "@P_SaleOrderProjectCode";
                arrParams[8] = "@Userlogin";
                arrParams[9] = "@InitialCode";

                object[] arrParamsValue = new string[10];
                arrParamsValue[0] = "SearchProjectStatusData";
                arrParamsValue[1] = projectStatus;
                arrParamsValue[2] = productType;
                arrParamsValue[3] = projectName;
                arrParamsValue[4] = userProjectCode;
                arrParamsValue[5] = salesClassification;
                arrParamsValue[6] = projectOrderType;
                arrParamsValue[7] = saleOrderProjectCode;
                arrParamsValue[8] = Userlogin;
                arrParamsValue[9] = check;

                var data = conn.ExecuteQuery<MES_SaleProject>(
                    SP_MES_PROJECT_STATUS_GET_DATA, arrParams, arrParamsValue);
                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        public List<DynamicCombobox> getProductTypeCombobox(string itemClassCD, string lang)
        {
            var result = new List<DynamicCombobox>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@ITEMCLASS_CD";
                arrParams[1] = "@Lang";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = itemClassCD;
                arrParamsValue[1] = lang;
                var data = conn.ExecuteQuery<DynamicCombobox>(
                    SP_GET_COMBOBOX_VALUE_ITEMCLASS,
                    arrParams,
                    arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MES_SaleProject> SearchSalesStatusByCustomer(string ProjectOrderType, string SaleOrderProjectName, string ProductionProjectCode, string ProductProjectName, string ProductProjectStatus, string Customer, string LoginUser)
        {
            if(ProjectOrderType == "All")
            {
                ProjectOrderType = null;
            }
            if(ProductProjectStatus == "All")
            {
                ProductProjectStatus = null;
            }
            if(Customer == "all")
            {
                Customer = null;    
            }
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[7];
                arrParams[0] = "@ProjectOrderType";
                arrParams[1] = "@SaleOrderProjectName";
                arrParams[2] = "@ProductionProjectCode";
                arrParams[3] = "@ProductProjectName";
                arrParams[4] = "@ProductProjectStatus";
                arrParams[5] = "@Customer";
                arrParams[6] = "@Userlogin";

                object[] arrParamsValue = new string[7];
                arrParamsValue[0] = ProjectOrderType;
                arrParamsValue[1] = SaleOrderProjectName;
                arrParamsValue[2] = ProductionProjectCode;
                arrParamsValue[3] = ProductProjectName;
                arrParamsValue[4] = ProductProjectStatus;
                arrParamsValue[5] = Customer;
                arrParamsValue[6] = LoginUser;


                var data = conn.ExecuteQuery<MES_SaleProject>(
                    "SP_MES_SALES_STATUS_BY_CUS_GET_DATA", arrParams, arrParamsValue);
                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }
        public List<Combobox> GetCustomer(string lang)
        {

            var result = new List<Combobox>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[1];
                arrParams[0] = "@Lang";
                object[] arrParamsValue = new string[1];
                arrParamsValue[0] = lang;
                var data = conn.ExecuteQuery<Combobox>(
                    "SP_GET_COMBOBOX_VALUE_DYNAMIC_CUSTOMER", arrParams, arrParamsValue);
                result = data.ToList();
            }
           
            return result;
        }
    }
}

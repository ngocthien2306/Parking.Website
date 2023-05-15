using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.SalesProject.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.SalesProject.Services.ServiceImp
{
    public class MESProductionProjectService : IMESProductionProjectService
    {
        private string SP_MES_PRODUCTION_PROJECT = "SP_MES_PRODUCTION_PROJECT";
        private string SP_MES_REQUEST_PRODUCTION_SEARCH = "SP_MES_REQUEST_PRODUCTION_SEARCH";
        private string itemName, salesOrderProjecrtName;



        public List<DynamicCombobox> GetAllCustomerCombobox()
        {
            try
            {
                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[1];
                    arrParams[0] = "@Method";

                    object[] arrParamsValue = new object[1];
                    arrParamsValue[0] = "GetAllCustomerCombobox";

                    var result = connect.ExecuteQuery<DynamicCombobox>(SP_MES_PRODUCTION_PROJECT, arrParams, arrParamsValue).ToList();
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DynamicCombobox> GetAllOrderTeamCombobox()
        {
            try
            {
                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[1];
                    arrParams[0] = "@Method";

                    object[] arrParamsValue = new object[1];
                    arrParamsValue[0] = "GetAllOrderTeamCombobox";

                    var result = connect.ExecuteQuery<DynamicCombobox>(SP_MES_PRODUCTION_PROJECT, arrParams, arrParamsValue).ToList();
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public MES_SaleProject GetListSalesProjectDetail(string ProjectCode)
        {
            try
            {
                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@ProjectCode";

                    object[] arrParamsValue = new object[2];
                    arrParamsValue[0] = "GetSalesProjectDetail";
                    arrParamsValue[1] = ProjectCode;

                    var result = connect.ExecuteQuery<MES_SaleProject>(SP_MES_PRODUCTION_PROJECT, arrParams, arrParamsValue).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<MES_SalesOrderProjectNew> GetListSalesOrderProjectPopup(string ProjectOrderType, string OrderNumber, string SalesOrderProjectName,string SalesOrderProjectCode)
        {
            try
            {
                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[5];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@OrderNumber";
                    arrParams[2] = "@SalesOrderProjectName";
                    arrParams[3] = "@ProjectOrderType";
                    arrParams[4] = "@SalesOrderProjectCode";


                    object[] arrParamsValue = new object[5];
                    arrParamsValue[0] = "GetListSalesOrderProjectPopup";
                    arrParamsValue[1] = OrderNumber;
                    arrParamsValue[2] = SalesOrderProjectName;
                    arrParamsValue[3] = ProjectOrderType;
                    arrParamsValue[4] = SalesOrderProjectCode;

                    var result = connect.ExecuteQuery<MES_SalesOrderProjectNew>(SP_MES_PRODUCTION_PROJECT, arrParams, arrParamsValue).ToList();
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<MES_SaleProject> SearchRequestProduction(MES_SaleProject model)
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                if ( model.ItemName == null)
                {
                    itemName = model.ItemName;

                }
                else
                {
                    itemName = model.ItemName.Trim();

                }
                if ( model.SalesOrderProjectName == null)
                {
                    salesOrderProjecrtName = model.SalesOrderProjectName;

                }
                else
                {
                    salesOrderProjecrtName = model.SalesOrderProjectName.Trim();
                }
                string[] arrParams = new string[6];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@ProductType";         
                arrParams[3] = "@ItemCode";
                arrParams[4] = "@ItemName";        
                arrParams[5] = "@SalesOrderProjectName";

                object[] arrParamsValue = new object[6];
                arrParamsValue[0] = "SearchRequestProduction";
                arrParamsValue[1] = model.ProjectCode;
                arrParamsValue[2] = model.ProductType;            
                arrParamsValue[3] = model.ItemCode;
                arrParamsValue[4] = itemName;       
                arrParamsValue[5] = salesOrderProjecrtName;
          

                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_REQUEST_PRODUCTION_SEARCH, arrParams, arrParamsValue);

                result = data.ToList();
            }
            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;
        }

        public Result SaveRequestProduction(List<MES_SaleProject> list,string userID, string ProdReqRequestType)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in list)
                        {
                            string[] arrParams = new string[4];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@ProjectCode";
                            arrParams[2] = "@UserID";
                            arrParams[3] = "@RequestType";
                            object[] arrParamsValue = new object[4];
                            arrParamsValue[0] = "UpdateRequestProduction";
                            arrParamsValue[1] = item.ProjectCode;
                            arrParamsValue[2] = userID;
                            arrParamsValue[3] = ProdReqRequestType;
                            
                            var rs = conn.ExecuteNonQuery(SP_MES_REQUEST_PRODUCTION_SEARCH, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        }
                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }

            return result;
        }

        public Result SaveSingleRequestProduction(string ProjectCode, string userID)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        string[] arrParams = new string[3];
                        arrParams[0] = "@Method";
                        arrParams[1] = "@ProjectCode";
                        arrParams[2] = "@UserID";
                        object[] arrParamsValue = new object[3];
                        arrParamsValue[0] = "UpdateRequestProduction";
                        arrParamsValue[1] = ProjectCode;
                        arrParamsValue[2] = userID;
                        var rs = conn.ExecuteNonQuery(SP_MES_REQUEST_PRODUCTION_SEARCH, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        
                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }

            return result;
        }
    }
}

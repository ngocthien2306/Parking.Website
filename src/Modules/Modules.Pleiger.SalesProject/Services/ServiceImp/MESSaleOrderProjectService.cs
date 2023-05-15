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
using InfrastructureCore.Models.Identity;

namespace Modules.Pleiger.SalesProject.Services.ServiceImp
{
    public class MESSaleOrderProjectService : IMESSaleOrderProjectService
    {
        private string SP_MES_SALE_ORDER_PROJECT_NEW = "SP_MES_SALE_ORDER_PROJECT_NEW";


        public List<DynamicCombobox> GetProjectOrderType(string GROUP_CD)
        {
            try
            {
                using(var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParam = new string[2];
                    arrParam[0] = "@Method";
                    arrParam[1] = "@GROUP_CD";
                    object[] varArrParam = new object[2];
                    varArrParam[0] = "GetProjectOrderType";
                    varArrParam[1] = GROUP_CD;
                    var result = connect.ExecuteQuery<DynamicCombobox>(SP_MES_SALE_ORDER_PROJECT_NEW, arrParam, varArrParam).ToList();
                    return result.ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public MES_SalesOrderProjectNew GetDetailSaleOrderProject(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@SalesOrderProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListSalesOrderProject";
                arrParamsValue[1] = projectCode;
                var result = conn.ExecuteQuery<MES_SalesOrderProjectNew>(SP_MES_SALE_ORDER_PROJECT_NEW, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }

        public MES_SalesOrderProjectNew GetDetailSaleOrderProjectNew(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@SalesOrderProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListSalesOrderProjectNew";
                arrParamsValue[1] = projectCode;
                var result = conn.ExecuteQuery<MES_SalesOrderProjectNew>(SP_MES_SALE_ORDER_PROJECT_NEW, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }
        public List<MES_SalesOrderProjectNew> GetListProjectOrder(MES_SalesOrderProjectNew model)
        {
            try
            {
                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[7];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@OrderNumber";
                    arrParams[2] = "@SalesOrderProjectName";
                    arrParams[3] = "@ProjectOrderType";
                    arrParams[4] = "@PartnerCode";
                    arrParams[5] = "@SalesOrderStatus";
                    arrParams[6] = "@SalesOrderProjectCode";
                    object[] arrParamsValue = new object[7];
                    arrParamsValue[0] = "GetListSalesOrderProject";
                    arrParamsValue[1] = model.OrderNumber;
                    arrParamsValue[2] = model.SalesOrderProjectName;
                    arrParamsValue[3] = model.ProjectOrderType;
                    arrParamsValue[4] = model.PartnerCode;
                    arrParamsValue[5] = model.SalesOrderStatus;
                    arrParamsValue[6] = model.saleOrderProjectCode;
                    var result = connect.ExecuteQuery<MES_SalesOrderProjectNew>(SP_MES_SALE_ORDER_PROJECT_NEW, arrParams, arrParamsValue).ToList();
                    int i = 1;
                    result.ForEach(x => x.No = i++);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Result DeleteSaleOrderProject(string PId)
        {
            try
            {

                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connect.BeginTransaction())
                    {
                        var resultIns = "N";
                        try
                        {
                            //string ProjectStatus = "PJMST01";
                            //if (model.SalesClassification != null && model.SalesClassification == "SCS007")
                            //{
                            //    ProjectStatus = "PJMST09";
                            //}
                            var arrParams = new string[2];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@SalesOrderProjectCode";
                            var arrParamValue = new object[2];
                            arrParamValue[0] = "DeleteSaleProject";
                            arrParamValue[1] = PId;
                            resultIns = connect.ExecuteScalar<string>(SP_MES_SALE_ORDER_PROJECT_NEW, CommandType.StoredProcedure, arrParams, arrParamValue, transaction);
                            transaction.Commit();
                            if (resultIns != "N")
                            {
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0008,
                                    Data = resultIns
                                };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0015,
                                    Data = ""
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                Success = false,
                                Message = "Save data not success! + Exception: " + ex.ToString(),
                                Data = ""
                            };
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Result SaveDataSaleProject(MES_SalesOrderProjectNew model, string userId)
        {
            try
            {

                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = connect.BeginTransaction())
                    {
                        var resultIns = "N";
                        try
                        {
                            if (model.Check == "Copy" )
                            {
                                if (ValidateName(model.UserSalesOrderProjectCode) > 0 )
                                {
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD00018,
                                        Data = "2"
                                    };
                                }
                            } 
                            else
                            {
                                if (ValidateName(model.UserSalesOrderProjectCode) > 0 && model.SalesOrderProjectCode == null)
                                {
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD00018,
                                        Data = "2"
                                    };
                                }
                            }

                            resultIns = connect.ExecuteScalar<string>(SP_MES_SALE_ORDER_PROJECT_NEW, CommandType.StoredProcedure,
                                new string[] {  "@Method", "@ProjectOrderType", "@UserSalesOrderProjectCode",
                                                "@SalesOrderProjectCode", "@SalesOrderProjectName",
                                                "@InCharge", "@OrderTeamCode",
                                                "@PartnerCode", "@OrderNumber",
                                                "@CreateUser", "@SalesOrderStatus", "@ETC", "@EditUser"
                                },
                                new object[] {  "SaveSaleOrderProjectData", model.ProjectOrderType, model.UserSalesOrderProjectCode,
                                                model.SalesOrderProjectCode, model.SalesOrderProjectName,
                                                model.InCharge, model.OrderTeamCode,
                                                model.PartnerCode, model.OrderNumber,
                                                userId, model.SalesOrderStatus, model.ETC, userId

                                }, transaction);
                            transaction.Commit();
                            if (resultIns != "N")
                            {
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0004,
                                    Data = resultIns
                                };
                            }
                            else
                            {
                                transaction.Rollback();
                                return new Result
                                {   
                                    Success = false,
                                    Message = MessageCode.MD0005,
                                    Data = "1"
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ValidateName(string Name)
        {
            try
            {
                using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@UserSalesOrderProjectCode";

                    object[] arrParamsValue = new object[2];
                    arrParamsValue[0] = "ValidateName";
                    arrParamsValue[1] = Name;
                    var result = connect.ExecuteScalar<int>(SP_MES_SALE_ORDER_PROJECT_NEW, arrParams, arrParamsValue);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

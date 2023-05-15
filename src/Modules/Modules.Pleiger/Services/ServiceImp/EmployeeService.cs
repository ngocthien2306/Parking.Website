using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using InfrastructureCore.Models.Identity;
using Microsoft.AspNetCore.Http;
using Modules.Admin.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class EmployeeService : IEmployeeService
    {
        private string SP_Name = "SP_Web_SYGroupAccessMenus";
        private string SP_Name_MES = "SP_MES_EMPLOYEES";
        private string SP_Name_Save = "SP_MES_EMPLOYEES_SAVE_DATA_GRID";
        private readonly IMESPartnerService _mesPartnerService;
        IDBContextConnection dbConnection;
        IDbConnection conn2;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SP_WEB_SY_USER = "SP_Web_SY_User";
        #region "Constructor"
        public EmployeeService(IDBContextConnection dbConnection, IHttpContextAccessor httpContextAccessor, IMESPartnerService mesPartnerService)
        {
            this.dbConnection = dbConnection;
            this._httpContextAccessor = httpContextAccessor;
            conn2 = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            _mesPartnerService = mesPartnerService;
        }

        #endregion
        public SYUser GetUserFromEmployessCode(string EmpCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@MenuID";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "SelectByMenuID";
                arrParamsValue[1] = EmpCode;
                var result = conn.ExecuteQuery<SYUser>(SP_Name, arrParams, arrParamsValue);
                return result.FirstOrDefault();
            }
        }

        public MESEmployees GetEmployess(string EmpCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@EmpCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "SelectByEmpCode";
                arrParamsValue[1] = EmpCode;
                var result = conn.ExecuteQuery<MESEmployees>(SP_Name_MES, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }

        public List<MESEmployees> GetListEmployess()
        {
            var result = new List<MESEmployees>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@DIV";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListEmployee";
                var data = conn.ExecuteQuery<MESEmployees>(SP_Name_MES, arrParams, arrParamsValue);
                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        public List<MESEmployees> ListSearchEmployee(string PartnerCode, string EmployeeNumber, string EmployeeNameKr, string EmployeeNameEng, string UseYN)

        {
            var result = new List<MESEmployees>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[6];
                arrParams[0] = "@DIV";
                arrParams[1] = "@PartnerCode";
                arrParams[2] = "@EmployeeNumber";
                arrParams[3] = "@EmployeeNameKr";
                arrParams[4] = "@EmployeeNameEng";
                arrParams[5] = "@UseYN";
                object[] arrParamsValue = new object[6];
                arrParamsValue[0] = "ListSearchEmployee";
                arrParamsValue[1] = PartnerCode;
                arrParamsValue[2] = EmployeeNumber;
                arrParamsValue[3] = EmployeeNameKr;
                arrParamsValue[4] = EmployeeNameEng;
                arrParamsValue[5] = UseYN;
                var data = conn.ExecuteQuery<MESEmployees>(SP_Name_MES, arrParams, arrParamsValue);
                result = data.ToList();
            }
            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        #region "Insert - Update - Delete"       
        public Result SaveMESEmployee(MESEmployees model, string CurrentUser)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    string sysusertype = GetSystemUserTypeByPartnerCode(model.PartnerCode);
                    result = conn.ExecuteNonQuery(SP_Name_Save,
                      new string[] { "@DIV",
                       "@EmployeeNumber","@OrgNumber","@EmployeeNameKr","@EmployeeNameEng", "@Level",
                       "@Company","@PartnerCode","@Birthday","@Address","@Email","@RfidTag","@UseYN","@MobileNumber","@SystemUserType","@CompanyType","@CurrentUser"
                      },
                      new object[] { "INSERT",
                      model.EmployeeNumber, model.OrgNumber,model.EmployeeNameKr,model.EmployeeNameEng,model.Level,
                      model.Company, model.PartnerCode, model.Birthday, model.Address,model.Email,model.RfidTag,model.UseYN,model.MobileNumber,sysusertype,model.CompanyType,CurrentUser
                            });
                    if (result > 0)
                    {
                        using (var conn2 = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                        {
                            string[] arrParams = new string[2];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@UserCode";
                            object[] arrParamsValue = new object[2];
                            arrParamsValue[0] = "GetListDataByUserCode";
                            arrParamsValue[1] = model.EmployeeNumber;
                            var checkDuplicate = conn2.ExecuteQuery<SYUser>(SP_WEB_SY_USER, arrParams, arrParamsValue);
                            if (checkDuplicate.ToList().Count <= 0)
                            {
                                //check partner type .if ACCT03 (partnercustomer)=> G000C007
                                string sysusertypeStr = GetSystemUserTypeByPartnerCode(model.PartnerCode);
                                //InsertSYuser(CurrentUser, model, sysusertypeStr);
                                if (model.CompanyType == null || model.CompanyType == "null" || model.CompanyType == "" )
                                {
                                    model.CompanyType = "G000C003";
                                }
                                InsertSYuser(CurrentUser, model, model.CompanyType);
                            }
                        }
                        return new Result
                        {
                            Success = true,
                            Message = "Save data success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Save data not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Save data not success! + Exception: " + ex.ToString(),
                    };
                }
            }
        }

        public string GetSystemUserTypeByPartnerCode(string partnerCode)
        {
            List<MES_Partner> listPartner = _mesPartnerService.GetPartnerByPartnerCode(partnerCode);
            MES_Partner partner = new MES_Partner();
            string systemUserType = "";
            if (partner != null)
            {
                partner = listPartner.FirstOrDefault();
            }
            if (partner?.PartnerType == "ACCT02") //partner
            {
                systemUserType = "G000C005";
            }
            if (partner?.PartnerType == "ACCT01") //customer
            {
                systemUserType = "G000C006";
            }
            if (partner?.PartnerType == "ACCT03") //partner-customer
            {
                systemUserType = "G000C007";
            }
            return systemUserType;
        }

        public Result DeleteMESEmployee(string EmpCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_Name_Save,
                        new string[] { "@DIV", "@EmployeeNumber" },
                        new object[] { "DELETE", EmpCode });

                    if (result > 0)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = "Delete data success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Delete data not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Delete data not success! + Exception: " + ex.ToString(),
                    };
                }
            }
        }

        public Result InsertSYuser(string userModify, MESEmployees model, string systemUserType)
        {
            var result = new Result();
            try
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                string method = string.IsNullOrEmpty(model.UserID) ? "InsertData" : "UpdateData";
                model.EmployeeNumber = string.IsNullOrEmpty(model.EmployeeNumber) ? "" : model.EmployeeNumber;
                string password = method == "InsertData" ? "123456" : null; //set Password Default

                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, method);
                dyParam.Add("@UserId", SqlDbType.VarChar, ParameterDirection.Input, method == "InsertData" ? Guid.NewGuid().ToString() : model.EmployeeNumber); // Generate GUID id for user
                dyParam.Add("@UserCode", SqlDbType.VarChar, ParameterDirection.Input, model.EmployeeNumber);
                dyParam.Add("@UserName", SqlDbType.VarChar, ParameterDirection.Input, model.EmployeeNameKr);
                dyParam.Add("@Email", SqlDbType.VarChar, ParameterDirection.Input, model.Email);
                dyParam.Add("@FirstName", SqlDbType.NVarChar, ParameterDirection.Input, model.EmployeeNameKr); //FirstName
                dyParam.Add("@LastName", SqlDbType.NVarChar, ParameterDirection.Input, "");//LastName
                if (method == "InsertData")
                {
                    dyParam.Add("@Password", SqlDbType.VarChar, ParameterDirection.Input, PasswordExtensions.HashPassword(password));
                }
                dyParam.Add("@UserType", SqlDbType.VarChar, ParameterDirection.Input, "G000C003");
                dyParam.Add("@SiteID", SqlDbType.VarChar, ParameterDirection.Input, "1");
                dyParam.Add("@UserModify", SqlDbType.VarChar, ParameterDirection.Input, userModify);
                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);
                dyParam.Add("@SystemUserType", SqlDbType.VarChar, ParameterDirection.Input, systemUserType);

                var data = SqlMapper.Execute(conn2, SP_WEB_SY_USER, param: dyParam, commandType: CommandType.StoredProcedure);

                string status = dyParam.GetOracleParameterByName("Message").Value.ToString();
                result.Success = status == "1" ? true : false;
                result.Message = status == "1" ? "Save data success!" : (status == "-1" ? "Save data not success" : "SYUser Name or SYUser Code is duplicate!");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.ToString();
            }
            finally
            {
                conn2.Close();
            }

            return result;
        }

        public Result DeleteEmployeeInfo(List<MESEmployees> listEmployeeInfo)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in listEmployeeInfo)
                        {
                            string[] arrParams = new string[2];
                            arrParams[0] = "@DIV";
                            arrParams[1] = "@EmployeeNumber";
                            object[] arrParamsValue = new object[2];
                            arrParamsValue[0] = "DELETE";
                            arrParamsValue[1] = item.EmployeeNumber;
                            var rs = conn.ExecuteNonQuery(SP_Name_Save, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        }
                        result.Success = true;
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Data = ex;
                    }

                }
            }
            using (var conn3 = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var transaction3 = conn3.BeginTransaction())
                {
                    try
                    {

                        foreach (var item in listEmployeeInfo)
                        {
                            string[] arrParams = new string[2];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@UserCode";
                            object[] arrParamsValue = new object[2];
                            arrParamsValue[0] = "DeleteUserforEmployee";
                            arrParamsValue[1] = item.EmployeeNumber;
                            var checkDuplicate = conn3.ExecuteNonQuery(SP_WEB_SY_USER, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction3);

                        }
                        result.Success = true;
                        transaction3.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction3.Rollback();
                        result.Success = false;
                        result.Data = ex;
                    }

                }

                return result;
            }
        }
        #endregion
        public List<DynamicCombobox> GetListEmployees()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@DIV";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListEmp";
                var result = conn.ExecuteQuery<DynamicCombobox>(SP_Name_MES, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
    }
}

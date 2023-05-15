using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Web;
using Microsoft.AspNetCore.Http;
//using Modules.Admin.Extensions;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using InfrastructureCore.Models.Menu;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Admin.Services.ServiceImp
{
    public class UserService : IUserService
    {
        #region "Properties"

        IDBContextConnection dbConnection;
        IDbConnection conn;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        private const string SUPER_ADMIN_ROLE = "G000C001";
        #region "Constructor"

        public UserService(IDBContextConnection dbConnection, IHttpContextAccessor httpContextAccessor)
        {
            this.dbConnection = dbConnection;
            this._httpContextAccessor = httpContextAccessor;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }

        #endregion

        #region Store Procedure Constant

        private const string SP_WEB_SY_USER = "SP_Web_SY_User";
        private const string SP_WEB_SY_USER_GROUPS = "SP_Web_SY_UserGroups";
        private const string SP_WEB_SY_USER_IN_GROUP = "SP_Web_SY_SYUsersInGroup";
        private const string SP_GET_USERTYPE_BYUSERID = "SP_GET_USERTYPE_BYUSERID";
        private const string SP_MES_CHECK_USER_ROLE = "SP_MES_CHECK_USER_ROLE";
        #endregion

        #region "Get Data"

        // Get all SYUser of Site for Login
        public List<SYUser> GetListDataByCode(string siteCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method", "@SiteCode" },
                    new object[] { "GetListDataByCode", siteCode });
                return result.ToList();
            }
        }

        // Get list SYUser not include SYUser superadimin
        public List<SYUser> GetListData()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method" },
                    new object[] { "GetListData" });
                return result.ToList();
            }
        }
        // Get list SYUser by SiteID
        public List<SYUser> GetListDataUserBySite(int siteId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method", "@SiteId" },
                    new object[] { "GetListDataUser", siteId });
                return result.ToList();
            }
        }
        // Get list SYUser by UserCode
        public List<SYUser> GetListDataByUserCode(string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method", "@UserCode" },
                    new object[] { "GetListDataByUserCode", UserCode });
                return result.ToList();
            }
        }
        // Get all User
        public List<SYUser> GetListDataAll(int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method", "@SiteId" },
                    new object[] { "GetListDataAll", SiteID });
                return result.ToList();
            }
         

        }
        public List<SYUser> GetListUserAll()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method" },
                    new object[] { "GetListUserAll" });
                return result.ToList();
            }

        }
        
        // Get list SYUser by SiteID and search
        public List<SYUser> GetListDataUserBySiteSearch(int siteId, SYUser model)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                int IsBlock = 2;

                if (model.IsBlock == false)
                {
                    IsBlock = 1;
                }
                if (model.IsBlock == true)
                {
                    IsBlock = 0;
                }

                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method", "@SiteId", "@UserCode", "@UserName", "@FirstName", "@UserType", "@IsBlock", "@UserYN" },
                    new object[] { "GetListDataUserSearch", siteId, model.UserCode, model.UserName, model.FirstName, model.UserType,Convert.ToInt32(IsBlock), model.UseYN });
                return result.ToList();
            }
        }
        #endregion

        #region "Insert - Update - Delete"

        // Insert - Update User
        public Result SaveUser(SYUser dataUser, string userModify)
        {
            var result = new Result();
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                string method = string.IsNullOrEmpty(dataUser.UserID) ? "InsertData" : "UpdateData";
                dataUser.UserCode = string.IsNullOrEmpty(dataUser.UserCode) ? "" : dataUser.UserCode;
                string password = method == "InsertData" ? dataUser.UserName.ToLower() : null;

                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, method);
                dyParam.Add("@UserId", SqlDbType.VarChar, ParameterDirection.Input, method == "InsertData" ? Guid.NewGuid().ToString() : dataUser.UserID); // Generate GUID id for user
                dyParam.Add("@UserCode", SqlDbType.VarChar, ParameterDirection.Input, dataUser.UserCode);
                dyParam.Add("@UserName", SqlDbType.VarChar, ParameterDirection.Input, dataUser.UserName);
                dyParam.Add("@Email", SqlDbType.VarChar, ParameterDirection.Input, dataUser.Email);
                dyParam.Add("@FirstName", SqlDbType.NVarChar, ParameterDirection.Input, dataUser.FirstName);
                dyParam.Add("@LastName", SqlDbType.NVarChar, ParameterDirection.Input, dataUser.LastName);
                if (method == "InsertData")
                {
                    dyParam.Add("@Password", SqlDbType.VarChar, ParameterDirection.Input, PasswordExtensions.HashPassword(password));
                }
                dyParam.Add("@UserType", SqlDbType.VarChar, ParameterDirection.Input, dataUser.UserType);
                // Quan change
                dyParam.Add("@SystemUserType", SqlDbType.VarChar, ParameterDirection.Input, dataUser.SystemUserType);


                dyParam.Add("@SiteID", SqlDbType.VarChar, ParameterDirection.Input, dataUser.SiteID);
                dyParam.Add("@UserModify", SqlDbType.VarChar, ParameterDirection.Input, userModify);
                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);
                int IsBlock = 0;
                if (dataUser.IsBlock == true)
                {
                    IsBlock = 1;
                }
                dyParam.Add("@IsBlock", SqlDbType.VarChar, ParameterDirection.Input, IsBlock);
                int PoMessage = 0;
                if (dataUser.PoMessage == true)
                {
                    PoMessage = 1;
                }
                dyParam.Add("@PoMessage", SqlDbType.VarChar, ParameterDirection.Input, PoMessage);


                var data = SqlMapper.Execute(conn, SP_WEB_SY_USER, param: dyParam, commandType: CommandType.StoredProcedure);

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
                conn.Close();
            }

            return result;
        }

        public Result InsertUpdateData(SYUser dataUser, string userModify)
        {
            var result = new Result();
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                string method = string.IsNullOrEmpty(dataUser.UserID) ? "InsertData" : "UpdateData";
                dataUser.UserCode = string.IsNullOrEmpty(dataUser.UserCode) ? "" : dataUser.UserCode;
                string password = method == "InsertData" ? dataUser.UserName.ToLower() : null;

                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, method);
                dyParam.Add("@UserId", SqlDbType.VarChar, ParameterDirection.Input, method == "InsertData" ? Guid.NewGuid().ToString() : dataUser.UserID); // Generate GUID id for user
                dyParam.Add("@UserCode", SqlDbType.VarChar, ParameterDirection.Input, dataUser.UserCode);
                dyParam.Add("@UserName", SqlDbType.VarChar, ParameterDirection.Input, dataUser.UserName);
                dyParam.Add("@Email", SqlDbType.VarChar, ParameterDirection.Input, dataUser.Email);
                dyParam.Add("@FirstName", SqlDbType.NVarChar, ParameterDirection.Input, dataUser.FirstName);
                dyParam.Add("@LastName", SqlDbType.NVarChar, ParameterDirection.Input, dataUser.LastName);
                if (method == "InsertData")
                {
                    dyParam.Add("@Password", SqlDbType.VarChar, ParameterDirection.Input, PasswordExtensions.HashPassword(password));
                }
                dyParam.Add("@UserType", SqlDbType.VarChar, ParameterDirection.Input, dataUser.UserType);
                // Quan change
                dyParam.Add("@SystemUserType", SqlDbType.VarChar, ParameterDirection.Input, dataUser.SystemUserType);


                dyParam.Add("@SiteID", SqlDbType.VarChar, ParameterDirection.Input, dataUser.SiteID);
                dyParam.Add("@UserModify", SqlDbType.VarChar, ParameterDirection.Input, userModify);
                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);
                int IsBlock = 0;
                if (dataUser.IsBlock == true)
                {
                    IsBlock = 1;
                }
                dyParam.Add("@IsBlock", SqlDbType.VarChar, ParameterDirection.Input, IsBlock);
                int PoMessage = 0;
                if (dataUser.PoMessage == true)
                {
                    PoMessage = 1;
                }
                dyParam.Add("@PoMessage", SqlDbType.VarChar, ParameterDirection.Input, PoMessage);


                var data = SqlMapper.Execute(conn, SP_WEB_SY_USER, param: dyParam, commandType: CommandType.StoredProcedure);               

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
                conn.Close();
            }

            return result;
        }

        // Delete User
        public Result DeleteData(string userId,string userCode)
        {
            // Quan add apply transaction
            var result = new Result();            
                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "DeleteUser");
                    dyParam.Add("@UserId", SqlDbType.VarChar, ParameterDirection.Input, userId);
                    dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);

                    var data = SqlMapper.Execute(conn, SP_WEB_SY_USER, param: dyParam, commandType: CommandType.StoredProcedure);
                    if (data > 0)
                    {
                        DeleteMESEmployee(userCode);
                    }
                    string status = dyParam.GetOracleParameterByName("Message").Value.ToString();
                    result.Success = status == "1" ? true : false;
                    result.Message = status == "1" ? "Delete data success!" : "Delete data not success";                  
                    result.Success = true;
                }              
                catch (Exception ex)
                {                   
                   
                    result.Success = false;
                    result.Message = ex.ToString();
                }
                finally
                {
                    conn.Close();
                }
            
            return result;
        }
        // Quan add delete Employee
        public Result DeleteMESEmployee(string userCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_MES_EMPLOYEES_SAVE_DATA_GRID",
                        new string[] { "@DIV", "@EmployeeNumber" },
                        new object[] { "DELETE", userCode });

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
        // Change password
        public Result ChangePassword(bool adminUpdate, string userId, string oldPassWord, string newPassword,int SiteID)
        {
            Result result = new Result();
            SYUser user = GetListDataAll(SiteID).Where(m => m.UserID == userId).FirstOrDefault();
            if (user != null)
            {
                if (!adminUpdate)
                {
                    if (!PasswordExtensions.VerifyPassword(user.Password, oldPassWord))
                    {
                        result.Success = false;
                        result.Message = "Old password is incorrect";
                    }
                }

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "ChangePassword");
                    dyParam.Add("@UserId", SqlDbType.VarChar, ParameterDirection.Input, userId);
                    dyParam.Add("@Password", SqlDbType.VarChar, ParameterDirection.Input, PasswordExtensions.HashPassword(newPassword));
                    dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);

                    var data = SqlMapper.Execute(conn, SP_WEB_SY_USER, param: dyParam, commandType: CommandType.StoredProcedure);
                    string status = dyParam.GetOracleParameterByName("Message").Value.ToString();
                    result.Success = status == "1" ? true : false;
                    result.Message = status == "1" ? "Change password data success!" : "Change password not success";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.ToString();
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                result.Success = false;
                result.Message = "SYUser not exist.";
            }

            return result;
        }

        // Update User Information
        public Result UpdateUserInformation(string userId, string firstName, string lastName, string email, string oldPassword, string newPassword
            , string userTypeName, int SiteID)
        {
            Result result = new Result();
            SYUser user = GetListDataAll(SiteID).Where(m => m.UserID == userId).FirstOrDefault();
            if (user != null)
            {
                if (!string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword))
                {
                    if (!PasswordExtensions.VerifyPassword(user.Password, oldPassword))
                    {
                        result.Success = false;
                        result.Message = "Old password is incorrect";

                        return result;
                    }
                }

                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "UpdateUserInfor");
                    dyParam.Add("@UserId", SqlDbType.VarChar, ParameterDirection.Input, userId);
                    dyParam.Add("@FirstName", SqlDbType.VarChar, ParameterDirection.Input, firstName);
                    dyParam.Add("@LastName", SqlDbType.VarChar, ParameterDirection.Input, lastName);
                    dyParam.Add("@Email", SqlDbType.VarChar, ParameterDirection.Input, email);
                    dyParam.Add("@Password", SqlDbType.VarChar, ParameterDirection.Input, string.IsNullOrEmpty(newPassword) ? null : PasswordExtensions.HashPassword(newPassword));
                    dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);
                    dyParam.Add("@UserTypeName", SqlDbType.VarChar, ParameterDirection.Input, userTypeName);

                    var data = SqlMapper.Execute(conn, SP_WEB_SY_USER, param: dyParam, commandType: CommandType.StoredProcedure);
                    string status = dyParam.GetOracleParameterByName("Message").Value.ToString();
                    result.Success = status == "1" ? true : false;
                    result.Message = status == "1" ? "Update information success!" : "Update information not success";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = ex.ToString();
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                result.Success = false;
                result.Message = "User not exist.";
            }

            return result;
        }

        #endregion

        #region SY Group Master For SuperAdmin

        public List<SYUserGroups> GetListGroupMasterBySiteID(SYLoggedUser info, int siteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                ///SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYUserGroups>(SP_WEB_SY_USER_GROUPS,
                    new string[] { "@DIV", "@SITE_ID" },
                    new object[] { CommonAction.SELECT, siteID }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }

        public List<SYUser> GetListDataUserBySiteForSuperAdmin(SYLoggedUser info, int siteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUser>(SP_WEB_SY_USER,
                    new string[] { "@Method", "@SiteId" },
                    new object[] { "GetListDataUser", siteID });
                return result.ToList();
            }
        }

        public List<SYUserGroups> GetListGroupBySiteID(int siteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUserGroups>(SP_WEB_SY_USER_GROUPS,
                    new string[] { "@DIV", "@SITE_ID" },
                    new object[] { CommonAction.SELECT, siteID }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }


        #endregion

        public string GetSystemUserTypeByUserId(string userId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                string name = "";
                var result = conn.ExecuteQuery<SYUser>(SP_GET_USERTYPE_BYUSERID, new string[] { "@UserId" }, new object[] { userId }).ToList();
                foreach (SYUser user in result)
                {
                    name = user.SystemUserType;
                    break;
                }
                return name;
            }
        }

        //PVN Added
        #region "New User Permission UI"

        private const string SP_NEW_USER_PERMISSON = "SP_NEW_USER_PERMISSON";

        public List<SYUserAccessMenus> GetAllUser(int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYUserAccessMenus>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@SITE_ID" },
                    new object[] { "GetListUser", SiteID });


                return result.ToList();
            }
        }

        public List<SYMenu> GetAllSite(int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYMenu>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@SITE_ID" },
                    new object[] { "GetAllMenu",SiteID });


                return result.ToList();
            }
        }

        public List<SYMenu> GetMenuByUserId(string UserCode, int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYMenu>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@UserCode" , "@SITE_ID" },
                    new object[] { "GetMenuByUserID", UserCode, SiteID });


                return result.ToList();
            }
        }

        public List<SYMenu> GetCheckedMenu(string UserCode, int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYMenu>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@UserCode", "@SITE_ID" },
                    new object[] { "GetSelectedMenu", UserCode, SiteID });
                return result.ToList();
            }
        }

        public List<SYGroupAccessMenus> GetPermission(string UserCode, int MenuId, int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYGroupAccessMenus>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@UserCode", "@MenuId", "@SITE_ID" },
                    new object[] { "GetPermission", UserCode, MenuId, SiteID });
                return result.ToList();
            }
        }
        public List<SYGroupAccessMenus> GetPermissionByUserAndMenuID(string UserID, int MenuID, int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
             
                var result = conn.ExecuteQuery<SYGroupAccessMenus>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@UserCode", "@MenuId", "@SITE_ID" },
                    new object[] { "GetPermissionByUserAndMenuID", UserID, MenuID, SiteID });
                return result.ToList();
            }
        }
        public List<SYGroupAccessMenus> GetPermissionByGroup(string UserCode, int MenuId, int SiteID,int GroupID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYGroupAccessMenus>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@UserCode", "@MenuId", "@SITE_ID","@GROUP_ID"},
                    new object[] { "GetPermissionByGroup", UserCode, MenuId, SiteID, GroupID });
                return result.ToList();
            }
        }
        public List<SYGroupAccessMenus> GetGroupByUser(string UserCode, int MenuId, int SiteID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                //SYLoggedUser info = GetCurrentUser();
                var result = conn.ExecuteQuery<SYGroupAccessMenus>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@UserCode", "@MenuId", "@SITE_ID" },
                    new object[] { "GetGroupByUser", UserCode, MenuId, SiteID });
                return result.ToList();
            }
        }
        

        public Result UpdateUserPermission(List<UserPermissionUpdate> userPermissionUpdates, SYLoggedUser CurrentUser)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in userPermissionUpdates)
                        {
                            string[] arrParamsAdd = new string[18];
                            arrParamsAdd[0] = "@Method";
                            arrParamsAdd[1] = "@USER_ID";
                            arrParamsAdd[2] = "@MenuId";
                            arrParamsAdd[3] = "@CREATE_YN";
                            arrParamsAdd[4] = "@PRINT_YN";
                            arrParamsAdd[5] = "@EDIT_YN";
                            arrParamsAdd[6] = "@DELETE_YN";
                            arrParamsAdd[7] = "@SEARCH_YN";
                            arrParamsAdd[8] = "@EXCEL_YN";
                            arrParamsAdd[9] = "@SAVE_YN";
                            arrParamsAdd[10] = "@UPDATED_BY";
                            arrParamsAdd[11] = "@SITE_ID";
                            arrParamsAdd[12] = "@UPLOAD_FILE_YN";
                            arrParamsAdd[13] = "@DELETE_FILE_YN";
                            arrParamsAdd[14] = "@INVENTORY_YN";
                            arrParamsAdd[15] = "@PURCHASE_ORDER_YN";
                            arrParamsAdd[16] = "@STATE";
                            arrParamsAdd[17] = "@EXPORT_EXCEL_ICUBE_YN";                          
                            object[] arrParamsAddValue = new object[18];
                            arrParamsAddValue[0] = "UpdateUserPermission";
                            arrParamsAddValue[1] = item.UserId;
                            foreach (var permissionUpdate in item.PermissionUpdate)
                            {
                                arrParamsAddValue[2] = permissionUpdate.MenuId;
                                arrParamsAddValue[3] = permissionUpdate.CREATE_YN;
                                arrParamsAddValue[4] = permissionUpdate.PRINT_YN;
                                arrParamsAddValue[5] = permissionUpdate.EDIT_YN;
                                arrParamsAddValue[6] = permissionUpdate.DELETE_YN;
                                arrParamsAddValue[7] = permissionUpdate.SEARCH_YN;
                                arrParamsAddValue[8] = permissionUpdate.EXCEL_YN;
                                arrParamsAddValue[9] = permissionUpdate.SAVE_YN;
                                arrParamsAddValue[10] = CurrentUser.UserID;
                                arrParamsAddValue[11] = CurrentUser.SiteID;
                                arrParamsAddValue[12] = permissionUpdate.UPLOAD_FILE_YN;
                                arrParamsAddValue[13] = permissionUpdate.DELETE_FILE_YN;
                                arrParamsAddValue[14] = permissionUpdate.INVENTORY_YN;
                                arrParamsAddValue[15] = permissionUpdate.PURCHASE_ORDER_YN;
                                arrParamsAddValue[16] = permissionUpdate.STATE;
                                arrParamsAddValue[17] = permissionUpdate.EXPORT_EXCEL_ICUBE_YN;

                                var rsAdd = conn.ExecuteNonQuery(SP_NEW_USER_PERMISSON, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, trans);
                            }
                        }

                        trans.Commit();
                        result.Success = true;
                    }
                    catch
                    {
                        trans.Rollback();
                        result.Success = false;
                    }
                }
            }

            return result;
        }
        public List<SYUsersInGroup> GetUserInGroupsByUserId(string UserId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUsersInGroup>(SP_NEW_USER_PERMISSON,
                    new string[] { "@Method", "@UserCode"},
                    new object[] { "GetUserInGroupsByUserId", UserId });
                 return result.ToList();
            }
        }
        #endregion

        #region Get User Type

        public CHECKRESULT CheckUserEmployee(string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@UserCode";
                var arrParamValues = new object[2];
                arrParamValues[0] = "CheckUserEmployee";
                arrParamValues[1] = UserCode;
                var result = conn.ExecuteQuery<CHECKRESULT>(SP_MES_CHECK_USER_ROLE, arrParams, arrParamValues).ToList();
                if (result != null || result.Count != 0)
                {
                    return result.FirstOrDefault();
                }
                return new CHECKRESULT();
            }
        }

        public List<CHECKRESULT> CheckUserRole_Partner(string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var ArrParams = new string[1];
                ArrParams[0] = "@UserCode";
                var ArrValues = new string[1];
                ArrValues[0] = UserCode;
                var result = conn.ExecuteQuery<CHECKRESULT>(SP_MES_CHECK_USER_ROLE, ArrParams, ArrValues).ToList();
                if (result == null || result.Count == 0)
                {
                    return result;
                }
                if (result != null && result.First().CheckResult > 0)
                {

                    return result;
                }
                return result;
            }
        }

        public CHECKRESULT CheckUserType(string UserId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@UserId";
                var arrParamValues = new object[2];
                arrParamValues[0] = "CheckUserType";
                arrParamValues[1] = UserId;
                var result = conn.ExecuteQuery<CHECKRESULT>(SP_MES_CHECK_USER_ROLE, arrParams, arrParamValues).ToList();
                if (result != null || result.Count != 0)
                {
                    return result.FirstOrDefault();
                }
                return new CHECKRESULT();
            }
        }
        #endregion
    }
}

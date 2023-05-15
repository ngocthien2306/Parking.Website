using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.Models.Identity;
using Microsoft.AspNetCore.Http;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Common.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Admin.Services.ServiceImp
{
    public class GroupUserService : IGroupUserService
    {
        #region "Properties"

        IDBContextConnection dbConnection;
        IDbConnection conn;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        private const string SUPER_ADMIN_ROLE = "G000C001";

        #region "Constructor"

        public GroupUserService(IDBContextConnection dbConnection, IHttpContextAccessor httpContextAccessor)
        {
            this.dbConnection = dbConnection;
            this._httpContextAccessor = httpContextAccessor;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }

        #endregion

        #region Store Procedure Constant

        private const string SP_WEB_SY_USER_GROUPS = "SP_Web_SY_UserGroups";
        private const string SP_WEB_SY_USER_IN_GROUP = "SP_Web_SY_SYUsersInGroup";

        #endregion

        #region "Get Data"

        #region "SYUser Group"

        // Get list SYUser Group of Site
        public List<SYUserGroups> GetListUserGroup(SYLoggedUser info, int siteId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYUserGroups>(SP_WEB_SY_USER_GROUPS,
                    new string[] { "@DIV", "@SITE_ID" },
                    new object[] { CommonAction.SELECT, info.UserType != SUPER_ADMIN_ROLE ? info.SiteID : siteId }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }

        #endregion

        #region "SYUser in user Group"

        // Get list SYUser in SYUser Group
        public List<SYUsersInGroup> GetListUserGroupSeletedByGroupID(string groupId, int siteId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@GROUP_ID";
                arrParams[2] = "@SITE_ID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetSYUserGroup";
                arrParamsValue[1] = groupId;
                arrParamsValue[2] = siteId;
                var result = conn.ExecuteQuery<SYUsersInGroup>(SP_WEB_SY_USER_IN_GROUP, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        public List<SYUsersInGroup> CheckUserInGroup(string userID, int siteId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@USER_ID";
                arrParams[2] = "@SITE_ID";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "CheckUserInGroup";
                arrParamsValue[1] = userID;
                arrParamsValue[2] = siteId;
                var result = conn.ExecuteQuery<SYUsersInGroup>(SP_WEB_SY_USER_IN_GROUP, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        #endregion

        #endregion

        #region "Insert - Update - Delete"

        #region "SYUser Group"

        // Save SYUser Group
        public Result SaveDataGroupMaster(SYUserGroups item, SYLoggedUser info, int siteId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string actionType = "";
                actionType = item.GROUP_ID == 0 ? CommonAction.INSERT : CommonAction.UPDATE;

                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_WEB_SY_USER_GROUPS,
                        new string[] { "@DIV", "@GROUP_ID", "@GROUP_NAME", "@DESCRIPTION", "@DEPT_CODE", "@SITE_ID", "@USER_MODIFY"},
                        new object[] { actionType, item.GROUP_ID, item.GROUP_NAME, item.DESCRIPTION, item.DEPT_CODE, info.UserType != SUPER_ADMIN_ROLE ? info.SiteID : siteId, info.UserID });

                    if (result == -1)
                    {
                        return new Result
                        {

                            Success = true,
                            Message = "Save changed data success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Save changed data not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Save changed data not success! + Exception: " + ex.ToString(),
                    };
                }

            }
        }

        // Delete SYUser Group
        public Result DeleteGroupMaster(SYUserGroups item, SYLoggedUser info, int siteId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    ///SYLoggedUser info = GetCurrentUser();
                    result = conn.ExecuteNonQuery(SP_WEB_SY_USER_GROUPS,
                        new string[] { "@DIV", "@GROUP_ID", "@SITE_ID" },
                        new object[] { CommonAction.DELETE, item.GROUP_ID, info.UserType != SUPER_ADMIN_ROLE ? info.SiteID : siteId });
                    if (result == -1)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = "Delete success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Delete not success!",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "Delete not success! + Exception: " + ex.ToString(),
                    };
                }

            }
        }

        #endregion

        #region "SYUser in SYUser Group"

        // Set SYUser into SYUser Group
        public Result SetUserIntoUserGroup(string groupId, List<SYUser> data, SYLoggedUser info, int siteId)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string[] arrParams = new string[3];
                        arrParams[0] = "@Method";
                        arrParams[1] = "@GROUP_ID";
                        arrParams[2] = "@SITE_ID";
                        object[] arrParamsValue = new object[3];
                        arrParamsValue[0] = "DeleteSYUserGroup";
                        arrParamsValue[1] = groupId;
                        arrParamsValue[2] = info.UserType != SUPER_ADMIN_ROLE ? info.SiteID : siteId;
                        var rsDel = conn.ExecuteNonQuery(SP_WEB_SY_USER_IN_GROUP, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        foreach (var item in data)
                        {
                            string[] arrParamsAdd = new string[5];
                            arrParamsAdd[0] = "@Method";
                            arrParamsAdd[1] = "@USER_CODE";
                            arrParamsAdd[2] = "@GROUP_ID";
                            arrParamsAdd[3] = "@SITE_ID";
                            arrParamsAdd[4] = "@CREATED_BY";
                            object[] arrParamsAddValue = new object[5];
                            arrParamsAddValue[0] = "InsertSYUserGroup";
                            arrParamsAddValue[1] = item.UserID;
                            arrParamsAddValue[2] = groupId;
                            arrParamsAddValue[3] = info.UserType != SUPER_ADMIN_ROLE ? info.SiteID : siteId;
                            arrParamsAddValue[4] = info.UserID;
                            var rsAdd = conn.ExecuteNonQuery(SP_WEB_SY_USER_IN_GROUP, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                        }
                        transaction.Commit();
                        result.Success = true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        result.Success = false;
                    }
                }
            }
            return result;
        }

        #endregion

        #endregion
    }
}

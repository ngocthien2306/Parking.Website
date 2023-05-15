using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoMapper;
using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using InfrastructureCore.Models.Identity;
using InfrastructureCore.Models.Menu;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;


namespace Modules.Admin.Services.ServiceImp
{
    public class WebAuthentication : IWebAuthentication
    {
        private readonly IDBContextConnection dbConnection;
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IAccessMenuService accessMenuService;
        private const string superAdminRole = "G000C001";
        private const string adminRole = "G000C002";

        public WebAuthentication(IDBContextConnection dbConnection, IUserService userService, IMapper mapper, IMenuService menuService, IAccessMenuService accessMenuService)
        {
            this.dbConnection = dbConnection;
            this.userService = userService;
            this.mapper = mapper;
            this.accessMenuService = accessMenuService;
        }

        public Result CheckLogIn(string username, string password)
        {
            Result result = new Result();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);

            try
            {

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var query = "SP_WEB_LOGIN_CHECK";
                var spInfor = dbConnection.GetSPInfor(query);
                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);

                //dyParam.Add("p_div", null, ParameterDirection.Input, value: "CheckLogIn");
                //dyParam.Add("p_empcode", null, ParameterDirection.Input, value: username);
                //dyParam.Add("p_password", null, ParameterDirection.Input, value: password);
                //dyParam.Add("MESSAGE", Oracle.ManagedDataAccess.Client.OracleDbType.NVarchar2, ParameterDirection.Output, size: 1000);
                //dyParam.Add("IO_CURSOR", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, ParameterDirection.Output);

                foreach (var info in spInfor)
                {
                    if (info.argument_name.ToUpper().Contains("P_DIV".ToUpper()))
                        dyParam.Add(info.argument_name, info.data_type, ParameterDirection.Input, value: "CheckLogIn");
                    if (info.argument_name.ToUpper().Contains("p_empcode".ToUpper()))
                        dyParam.Add(info.argument_name, info.data_type, ParameterDirection.Input, value: username);
                    if (info.argument_name.ToUpper().Contains("p_password".ToUpper()))
                        dyParam.Add(info.argument_name, info.data_type, ParameterDirection.Input, value: password);
                    if (info.argument_name.ToUpper().Contains("MESSAGE".ToUpper()))
                        dyParam.Add(info.argument_name, info.data_type, ParameterDirection.Output, size: 1000);
                    if (info.argument_name.ToUpper().Contains("IO_CURSOR".ToUpper()))
                        dyParam.Add(info.argument_name, info.data_type, ParameterDirection.Output);
                }


                var temp = SqlMapper.Query<SYLoggedUser>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                result.Message = dyParam.GetOracleParameterByName("MESSAGE").Value.ToString();
                result.Success = true;
                result.Message = "OK";
                result.Data = temp.FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            finally
            {
                conn.Close();
            }


            return result;
        }

        /// <summary>
        /// 1. Check passửok
        /// 2. Check locked
        /// 3. Show warning passwork time..
        /// 4. Return login failed count...
        /// </summary>
        /// <param name="siteCode"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Result CheckLogInTL(string siteCode, string username, string password)
        {
            var result = new Result();

            var listUserOfSite = userService.GetListDataByCode(siteCode);
            if (listUserOfSite.Count > 0)
            {
                var user = listUserOfSite.Where(m => m.UserName == username).FirstOrDefault();
                if (user != null)
                {
                    if (PasswordExtensions.VerifyPassword(user.Password, password))
                    {
                        SYLoggedUser UserInfo = SetInfoSYLoggedUser(user);
                        result.Data = UserInfo;
                        result.Success = true;
                        result.Message = "Login is successfully.";
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Password is incorrect.";
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "SYUser not exist.";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "SYUser not exist.";
            }

            return result;
        }

        // Set SYUser info after login success
        private SYLoggedUser SetInfoSYLoggedUser(SYUser user)
        {
            SYLoggedUser userLogin = new SYLoggedUser();

            userLogin = mapper.Map<SYLoggedUser>(user);
            List<SYMenu> lstMenu = new List<SYMenu>();
            if (userLogin.UserType == superAdminRole)
            {
                lstMenu = accessMenuService.GetAccessMenuWithSuperAdmin();
            }
            else if (userLogin.UserType == adminRole)
            {
                lstMenu = accessMenuService.GetAccessMenuWithAdmin(userLogin.SiteID);
            }
            else
            {
                lstMenu = accessMenuService.GetAccessMenuWithUserCode(userLogin.UserCode, userLogin.SiteID, userLogin.UserID);
            }
            userLogin.AuthorizedMenus = lstMenu;

            // get toolbar
            userLogin.MenuAccessList = accessMenuService.GetListAccessToobarWithUser(userLogin);

            return userLogin;
        }
       
    }

}
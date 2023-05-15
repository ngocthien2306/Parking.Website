using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.Models.Site;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Admin.Services.ServiceImp
{
    public class SiteService : ISiteService
    {
        #region Properties

        IDBContextConnection dbConnection;
        IDbConnection conn;

        #endregion

        #region "Constructor"

        public SiteService(IDBContextConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }

        #endregion

        #region Store Procedure Constant

        private const string SP_WEB_SY_SITE = "SP_Web_SY_Site";

        #endregion

        #region "Get Data"

        // Get list Site
        public List<SYSite> GetListData()
        {
            var result = new List<SYSite>();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetListData");

                var data = SqlMapper.Query<SYSite>(conn, SP_WEB_SY_SITE, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                result = data;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        // Get Site detail
        public SYSite GetDetail(int siteID)
        {
            var result = new SYSite();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetDetail");
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);

                var data = SqlMapper.Query<SYSite>(conn, SP_WEB_SY_SITE, param: dyParam, commandType: CommandType.StoredProcedure).FirstOrDefault();
                result = data;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        // Get Site detail by Code
        public SYSite GetDetailByCode(string siteCode)
        {
            var result = new SYSite();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetDetailByCode");
                dyParam.Add("@SiteCode", SqlDbType.VarChar, ParameterDirection.Input, siteCode);

                var data = SqlMapper.Query<SYSite>(conn, SP_WEB_SY_SITE, param: dyParam, commandType: CommandType.StoredProcedure).FirstOrDefault();
                result = data;
            }
            catch 
            {

            }
            finally
            {
                conn.Close();
            }

            return result;
        }

        #endregion

        #region "Insert - Update - Delete"

        // Insert - Update Site
        public Result SaveData(int siteID, string siteCode, string siteName, string siteDescription, string userModify)
        {
            var result = new Result();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);

                string method = siteID == 0 ? "InsertData" : "UpdateData";

                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, method);
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
                dyParam.Add("@SiteCode", SqlDbType.VarChar, ParameterDirection.Input, siteCode);
                dyParam.Add("@SiteName", SqlDbType.VarChar, ParameterDirection.Input, siteName);
                dyParam.Add("@SiteDescription", SqlDbType.VarChar, ParameterDirection.Input, siteDescription);
                dyParam.Add("@UserModify", SqlDbType.VarChar, ParameterDirection.Input, userModify);
                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);

                var data = SqlMapper.Execute(conn, SP_WEB_SY_SITE, param: dyParam, commandType: CommandType.StoredProcedure);
                string status = dyParam.GetOracleParameterByName("Message").Value.ToString();
                result.Success = status == "1" ? true : false;
                result.Message = status == "1" ? "Save data success!" : "Save data not success";
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

        // Update Site detail
        public Result UpdateData(SYSite model, string userModify)
        {
            var result = new Result();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);

                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "UpdateDetail");
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, model.SiteID);
                dyParam.Add("@IconPath", SqlDbType.VarChar, ParameterDirection.Input, model.IconPath);
                // Login
                dyParam.Add("@LoginBackgroundImage", SqlDbType.VarChar, ParameterDirection.Input, model.LoginBackgroundImage);
                dyParam.Add("@LoginTitle", SqlDbType.VarChar, ParameterDirection.Input, model.LoginTitle);
                dyParam.Add("@LoginTextColor", SqlDbType.VarChar, ParameterDirection.Input, model.LoginTextColor);
                // Logo
                dyParam.Add("@LogoPath", SqlDbType.VarChar, ParameterDirection.Input, model.LogoPath);
                dyParam.Add("@LogoName", SqlDbType.VarChar, ParameterDirection.Input, model.LogoName);
                dyParam.Add("@LogoBackgroundColor", SqlDbType.VarChar, ParameterDirection.Input, model.LogoBackgroundColor);
                dyParam.Add("@LogoComponentName", SqlDbType.VarChar, ParameterDirection.Input, model.LogoComponentName);
                dyParam.Add("@AccountComponentName", SqlDbType.VarChar, ParameterDirection.Input, model.AccountComponentName);
                // Menu
                dyParam.Add("@MenuType", SqlDbType.VarChar, ParameterDirection.Input, model.MenuType);
                dyParam.Add("@SideBarType", SqlDbType.VarChar, ParameterDirection.Input, model.SideBarType);
                // Left Menu
                dyParam.Add("@SideParentActiveBAckgroundColor", SqlDbType.VarChar, ParameterDirection.Input, model.SideParentActiveBackgroundColor);
                dyParam.Add("@SideActiveBackgroundColor", SqlDbType.VarChar, ParameterDirection.Input, model.SideActiveBackgroundColor);
                dyParam.Add("@SideHoverBackgroundColor", SqlDbType.VarChar, ParameterDirection.Input, model.SideHoverBackgroundColor);
                dyParam.Add("@SideMenuComponentName", SqlDbType.VarChar, ParameterDirection.Input, model.SideMenuComponentName);
                dyParam.Add("@ShowLeftMenuBottom", SqlDbType.VarChar, ParameterDirection.Input, model.ShowLeftMenuBottom == "True" ? 1 : 0);
                dyParam.Add("@SideMenuBottomComponentName", SqlDbType.VarChar, ParameterDirection.Input, model.SideMenuBottomComponentName);
                // Top Background
                dyParam.Add("@TopBackgroundColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopBackgroundColor);
                dyParam.Add("@TopBackgroundHoverColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopBackgroundHoverColor);
                dyParam.Add("@TopBackgroundActiveColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopBackgroundActiveColor);
                dyParam.Add("@TopBackgroundActiveHoverColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopBackgroundActiveHoverColor);
                // Top Text
                dyParam.Add("@TopTextColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopTextColor);
                dyParam.Add("@TopTextHoverColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopTextHoverColor);
                dyParam.Add("@TopTextActiveColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopTextActiveColor);
                dyParam.Add("@TopTextActiveHoverColor", SqlDbType.VarChar, ParameterDirection.Input, model.TopTextActiveHoverColor);
                // Footer
                dyParam.Add("@FooterVisible", SqlDbType.Bit, ParameterDirection.Input, model.FooterVisible == "True" ? 1 : 0);
                dyParam.Add("@FooterComponentName", SqlDbType.VarChar, ParameterDirection.Input, model.FooterComponentName);
                dyParam.Add("@FooterBackgroundColor", SqlDbType.VarChar, ParameterDirection.Input, model.FooterBackgroundColor);
                dyParam.Add("@FooterLeftText", SqlDbType.VarChar, ParameterDirection.Input, model.FooterLeftText);
                dyParam.Add("@FooterRightText", SqlDbType.VarChar, ParameterDirection.Input, model.FooterRightText);
                // Account Policy
                dyParam.Add("@ChangePassPeriod", SqlDbType.Int, ParameterDirection.Input, model.ChangePassPeriod == (int?)null ? 90 : model.ChangePassPeriod.Value); // Default is 90 days
                dyParam.Add("@FailedWaitTime", SqlDbType.Int, ParameterDirection.Input, model.FailedWaitTime == (int?)null ? 15 : model.FailedWaitTime.Value); // Default is 15 mins
                dyParam.Add("@MaxLogFail", SqlDbType.Int, ParameterDirection.Input, model.MaxLogFail == (int?)null ? 5 : model.MaxLogFail.Value); // Default is 5 times 
                dyParam.Add("@SessionTimeout", SqlDbType.Int, ParameterDirection.Input, model.SessionTimeOut == (int?)null ? 60 : model.SessionTimeOut.Value); //session timeout

                dyParam.Add("@UserModify", SqlDbType.VarChar, ParameterDirection.Input, userModify);

                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);

                var data = SqlMapper.Execute(conn, SP_WEB_SY_SITE, param: dyParam, commandType: CommandType.StoredProcedure);
                string status = dyParam.GetOracleParameterByName("Message").Value.ToString();
                result.Success = status == "1" ? true : false;
                result.Message = status == "1" ? "Save data success!" : "Save data not success";
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

        // Delete Site
        public Result DeleteData(int siteID)
        {
            var result = new Result();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "DeleteData");
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);

                var data = SqlMapper.Execute(conn, SP_WEB_SY_SITE, param: dyParam, commandType: CommandType.StoredProcedure);
                string status = dyParam.GetOracleParameterByName("Message").Value.ToString();
                result.Success = status == "1" ? true : false;
                result.Message = status == "1" ? "Delete data success!" : "Delete data not success";
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

        #endregion
    }
}

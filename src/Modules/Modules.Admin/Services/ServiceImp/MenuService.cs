using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.Models.Menu;
using Modules.Admin.Models;
using Modules.Admin.Services.IService;
using Modules.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Admin.Services.ServiceImp
{
    public class MenuService : IMenuService
    {
        #region Properties

        IDBContextConnection dbConnection;
        IDbConnection conn;

        #endregion

        #region "Constructor"

        public MenuService(IDBContextConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }

        #endregion

        #region Store Procedure Constant

        private const string SP_WEB_SY_MENU = "SP_Web_SY_Menu";

        #endregion

        #region "Get Data"

        // Get list Menu by siteID
        public List<SYMenu> GetListDataByGroup(int siteID)
        {
            var result = new List<SYMenu>();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetListByGroup");
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);

                var data = SqlMapper.Query<SYMenu>(conn, SP_WEB_SY_MENU, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
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

        // Get list Menu Level
        public List<DropDownInt> GetListMenuLevel()
        {
            var result = new List<DropDownInt>();

            result.Add(new DropDownInt() { Value = 1, Text = "1" });
            result.Add(new DropDownInt() { Value = 2, Text = "2" });
            result.Add(new DropDownInt() { Value = 3, Text = "3" });

            return result;
        }

        // Get list Menu Parent
        public List<SYMenu> GetListMenuParent(int siteID, int menuLevel)
        {
            var result = new List<SYMenu>();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetMenuByParent");
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
                dyParam.Add("@MenuLevel", SqlDbType.VarChar, ParameterDirection.Input, menuLevel);

                var data = SqlMapper.Query<SYMenu>(conn, SP_WEB_SY_MENU, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
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

        // Get list PageLayout
        public List<DropDownInt> GetListPageLayout()
        {
            var result = new List<DropDownInt>();

            var data = new List<SYPageLayout>();
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetListPageLayout");

                data = SqlMapper.Query<SYPageLayout>(conn, SP_WEB_SY_MENU, param: dyParam, commandType: CommandType.StoredProcedure).ToList();

            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }

            if (data.Count > 0)
            {
                result.AddRange(data.Select(x => new DropDownInt()
                {
                    Text = x.PAG_KEY,
                    Value = x.PAG_ID
                }));
            }

            return result;
        }
        // Get list PageLayout Board
        public List<SYBoardInfo> GetListPageBoard(int siteID)
        {
            var result = new List<SYBoardInfo>();

            
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetListPageBoard");
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);

                result = SqlMapper.Query<SYBoardInfo>(conn, SP_WEB_SY_MENU, param: dyParam, commandType: CommandType.StoredProcedure).ToList();

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
        
        public List<DropDownInt> GetListMenuIcon()
        {
            var result = new List<DropDownInt>();

            var data = new List<SYIcon>();
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, "GetListMenuIcon");

                data = SqlMapper.Query<SYIcon>(conn, SP_WEB_SY_MENU, param: dyParam, commandType: CommandType.StoredProcedure).ToList();

            }
            catch 
            {

            }
            finally
            {
                conn.Close();
            }

            if (data.Count > 0)
            {
                result.AddRange(data.Select(x => new DropDownInt()
                {
                    Text = x.ICON_CODE,
                    Value = x.ICON_ID
                }));
            }

            return result;
        }

        #endregion

        #region "Insert - Update - Delete"

        // Insert - Update Menu
        public Result InsertUpdateData(int siteID, int menuID, string menuName,string menuNameEng, string menuPath, int menuLevel,
            int? menuParentID, int? menuSeq, string adminLevel, string menuType, int? programID, string mobileUse,
            string intraUse, string menuDesc, string startupPageUse, string isCanClose, int? menuIcon)
        {
            var result = new Result();

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);

                string method = menuID == 0 ? "InsertData" : "UpdateData";

                dyParam.Add("@Method", SqlDbType.VarChar, ParameterDirection.Input, method);
                dyParam.Add("@SiteID", SqlDbType.Int, ParameterDirection.Input, siteID);
                dyParam.Add("@MenuID", SqlDbType.VarChar, ParameterDirection.Input, menuID);
                dyParam.Add("@MenuName", SqlDbType.VarChar, ParameterDirection.Input, menuName);
                dyParam.Add("@MenuNameEng", SqlDbType.VarChar, ParameterDirection.Input, menuNameEng);
                dyParam.Add("@MenuPath", SqlDbType.VarChar, ParameterDirection.Input, menuPath);

                dyParam.Add("@MenuLevel", SqlDbType.Int, ParameterDirection.Input, menuLevel);
                dyParam.Add("@MenuParentID", SqlDbType.Int, ParameterDirection.Input, menuParentID == -1 ? null : menuParentID);
                dyParam.Add("@MenuSeq", SqlDbType.Int, ParameterDirection.Input, menuSeq);

                dyParam.Add("@AdminLevel", SqlDbType.VarChar, ParameterDirection.Input, adminLevel);
                dyParam.Add("@MenuType", SqlDbType.VarChar, ParameterDirection.Input, menuType);
                dyParam.Add("@ProgramID", SqlDbType.Int, ParameterDirection.Input, programID == -1 ? null : programID);

                dyParam.Add("@MobileUse", SqlDbType.VarChar, ParameterDirection.Input, mobileUse);
                dyParam.Add("@IntraUse", SqlDbType.VarChar, ParameterDirection.Input, intraUse);

                dyParam.Add("@MenuDesc", SqlDbType.VarChar, ParameterDirection.Input, menuDesc);
                dyParam.Add("@StartupPageUse", SqlDbType.VarChar, ParameterDirection.Input, startupPageUse);
                dyParam.Add("@IsCanClose", SqlDbType.VarChar, ParameterDirection.Input, isCanClose);
                dyParam.Add("@MenuIcon", SqlDbType.Int, ParameterDirection.Input, menuIcon);              
                // OutPut
                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);

                var data = SqlMapper.Execute(conn, SP_WEB_SY_MENU, param: dyParam, commandType: CommandType.StoredProcedure);
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

        // Delete Menu
        public Result DeleteData(int menuID)
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
                dyParam.Add("@MenuID", SqlDbType.VarChar, ParameterDirection.Input, menuID);
                dyParam.Add("@Message", SqlDbType.Int, ParameterDirection.Output, null, size: 1);

                var data = SqlMapper.Execute(conn, SP_WEB_SY_MENU, param: dyParam, commandType: CommandType.StoredProcedure);
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

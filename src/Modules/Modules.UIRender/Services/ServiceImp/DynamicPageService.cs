using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.DataAccess;
using InfrastructureCore.Helpers;
using InfrastructureCore.Utils;
using Microsoft.AspNetCore.Http;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Common.Utils;
using Modules.Pleiger.CommonModels;
using Modules.UIRender.Models;
using Modules.UIRender.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.UIRender.Services.ServiceImp
{
    public partial class DynamicPageService : IDynamicPageService
    {
        #region
        private const string EXCEL_EXPORT_FOLDER = @"RenderExcel/";
        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "hhmmss";
        #endregion

        #region Properties
        IDBContextConnection dbConnection;
        IDbConnection conn;
        #endregion

        #region Constructor
        public DynamicPageService(IDBContextConnection dbConnection)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            this.dbConnection = dbConnection;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }
        #endregion

        #region Page element
        public List<SYPageRelationship> GetRelationship(int PageID)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    var lstPageRelationship = conn.ExecuteQuery<SYPageRelationship>("SP_Web_DynamicPage",
            //        new string[] { "@DIV", "@PAG_ID" },
            //        new object[] { "PageRelationship", PageID });
            //    return lstPageRelationship.ToList();
            //}


            var lstPageRelationship = new List<SYPageRelationship>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";


            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                        //get PageRelationship
                        dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "PageRelationship");
                        dyParam.Add("@PAG_ID", SqlDbType.NVarChar, ParameterDirection.Input, PageID);
                        var result = SqlMapper.Query<SYPageRelationship>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        lstPageRelationship = result.ToList();

                        transaction.Commit();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                    }
                }
            }
            return lstPageRelationship;
        }

        #endregion

        #region Get Info Page
        public async Task<List<PageElement>> GetInforPageLayout(string PageID)
        {
            var result = new List<PageElement>();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);

            var query = "SP_Web_DynamicPage";


            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                        dyParam.Add("@div", SqlDbType.NVarChar, ParameterDirection.Input, "SYPageElement");
                        dyParam.Add("@PageID", SqlDbType.NVarChar, ParameterDirection.Input, PageID);
                        var temp1 = await SqlMapper.QueryAsync<PageElement>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        result = temp1.ToList();
                        var temp = result.Where(m => m.Type == "grid").ToList();
                        foreach (var item in temp)
                        {
                            var dyParam1 = dbConnection.CreateParameters(DbmsTypes.Mssql);
                            dyParam1.Add("@div", SqlDbType.NVarChar, ParameterDirection.Input, "SYPageElementDetail");
                            dyParam1.Add("@PageID", SqlDbType.NVarChar, ParameterDirection.Input, PageID);
                            dyParam1.Add("@EleID", SqlDbType.NVarChar, ParameterDirection.Input, item.EleID);
                            var temp2 = await SqlMapper.QueryAsync<PageElementDetail>(conn, query, param: dyParam1, transaction: transaction, commandType: CommandType.StoredProcedure);
                            item.lstElementDetail = temp2.ToList();
                        }

                        transaction.Commit();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                    }
                }
            }
            //if (conn.State == ConnectionState.Open)
            //{

            //    result = SqlMapper.Query<PageElement>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
            //}


            return result;
        }

        public string GetJSPath(int pagID)
        {
            string JSPath = "";
            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    //get SYpagelayout
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "GetJSPath");
                    dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, pagID);
                    var rs = SqlMapper.Query<string>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    JSPath = rs.FirstOrDefault().ToString();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }

            return JSPath;
        }

        public SYPageLayout GetInfoPage(int PageID)
        {
            var pageLayout = new SYPageLayout();
            var lstPageElement = new List<SYPageLayElements>();
            var lstDataMap = new List<DataMapInfo>();
            //var listReference = new List<SYPageLayElementReference>();
            //var conn = dbConnection.GetDbConnection();
            // var query = "SP_Web_DynamicPage";
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    pageLayout = GetPageLayout(PageID);
                    //get sypage lay element                       
                    lstPageElement = GetPageElements(PageID);
                    //get data mapping
                    foreach (var item in lstPageElement)
                    {
                        List<SYDataMap> listMap = new List<SYDataMap>();
                        listMap = GetDataMapping(item.PAG_ID, item.PEL_ID);
                        foreach (var item1 in listMap)
                        {
                            item1.lstMapDetail = GetDataMappingDetails(item1.MAP_ID);
                        }
                        item.listDataMap = listMap;

                        // Get list reference
                        List<SYPageLayElementReference> listReference = new List<SYPageLayElementReference>();
                        listReference = GetDataElementReference(PageID);
                        if (listReference != null && listReference.Count > 0)
                        {
                            item.listReference = listReference;
                        }
                    }

                    //// Get list reference
                    //listReference = GetDataElementReference(PageID);
                    //if (listReference != null && listReference.Count > 0)
                    //{
                    //    pageLayout.listReference = listReference;
                    //}
                    //get list toolbar
                    pageLayout.listToolbar = GetToolbarActionsNormalWithPageID(PageID);
                    //get list action for page                               
                    List<SYPageActions> listAct = new List<SYPageActions>();
                    listAct = GetPageAction(PageID);
                    //get action details
                    foreach (var itemAct in listAct)
                    {
                        itemAct.listActionDetail = GetPageActionDetail(itemAct.ACT_ID);
                        foreach (var itemActDL in itemAct.listActionDetail)
                        {
                            if (itemActDL.ACT_TYPE == "G010C001" || itemActDL.ACT_TYPE == "G010C003" || itemActDL.ACT_TYPE == "G010C005" || itemActDL.ACT_TYPE == "G010C006") // MAP and CBMAP AND REDIẺCT
                            {
                                itemActDL.dataMap = GetDataMappingWithMapID(itemActDL.SOURCE_ID);
                                if (itemActDL.dataMap != null)
                                {
                                    itemActDL.dataMap.lstMapDetail = GetDataMappingDetails(itemActDL.dataMap.MAP_ID);
                                }
                            }
                            if (itemActDL.ACT_TYPE == "G010C002")
                            {
                                if (itemAct.ACT_PEL_TYP == "G007C001")
                                {
                                    itemActDL.dataMap = GetDataMappingWithMapID(itemActDL.SOURCE_ID);
                                    if (itemActDL.dataMap != null)
                                    {
                                        itemActDL.dataMap.lstMapDetail = GetDataMappingDetails(itemActDL.dataMap.MAP_ID);
                                    }
                                }
                                var tempPage = GetInfoPage(itemActDL.SOURCE_ID);
                                itemActDL.dataPage = tempPage;
                            }
                        }

                        ////Get Clear Element
                        var listClear = GetDataSelectedGridClearElements(PageID, itemAct.ACT_ID);
                        foreach (var item in listClear)
                        {
                            if (item.PEL_TYP == "G002C001")
                            {
                                item.listChild = GetDataClearElementsWithTypeForm(PageID, itemAct.ACT_ID, item.PEL_ID, item.PAG_ID_SRC);
                            }
                        }
                        itemAct.listClearPEL = listClear;
                    }
                    if (listAct.Count > 0)
                    {
                        pageLayout.listAction = listAct;
                    }
                    if (pageLayout != null)
                    {
                        pageLayout.listPageElement = lstPageElement;
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    //transaction.Rollback();
                }
            }
            return pageLayout;
        }
        public SYPageLayout GetPageLayout(int pageID)
        {
            var pageLayout = new SYPageLayout();
            //var conn = dbConnection.GetDbConnection();
            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    //get SYpagelayout
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "PageLayout");
                    dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, pageID);
                    var rs = SqlMapper.Query<SYPageLayout>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    pageLayout = rs.FirstOrDefault();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return pageLayout;
        }
        public List<SYPageLayElements> GetPageElements(int pageID)
        {
            var pageElement = new List<SYPageLayElements>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "PageLayElements");
                    dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, pageID);
                    var rs = SqlMapper.Query<SYPageLayElements>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    pageElement = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return pageElement;
        }
        public List<SYDataMap> GetDataMapping(int pageID, string pelID)
        {
            var dataMaps = new List<SYDataMap>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "DataMap");
                    dyParam.Add("@MAP_PEL_ID", SqlDbType.NVarChar, ParameterDirection.Input, pelID);
                    dyParam.Add("@MAP_PAG_ID", SqlDbType.Int, ParameterDirection.Input, pageID);
                    var rs = SqlMapper.Query<SYDataMap>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    dataMaps = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataMaps;
        }
        public List<SYPageLayElementReference> GetDataElementReference(int pageID)
        {
            var listRefernce = new List<SYPageLayElementReference>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "ElementReference");
                    //dyParam.Add("@MAP_PEL_ID", SqlDbType.NVarChar, ParameterDirection.Input, pelID);
                    dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, pageID);
                    var rs = SqlMapper.Query<SYPageLayElementReference>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    listRefernce = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return listRefernce;
        }
        public List<SYDataMapDetails> GetDataMappingDetails(int mapID)
        {
            var dataMapsDl = new List<SYDataMapDetails>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "DataMapDetail");
                    dyParam.Add("@MAP_ID", SqlDbType.Int, ParameterDirection.Input, mapID);
                    var rs = SqlMapper.Query<SYDataMapDetails>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    dataMapsDl = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataMapsDl;
        }
        public List<SYPageActions> GetPageAction(int pageID)
        {
            var dataActions = new List<SYPageActions>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParamAct = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParamAct.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "PageAction");
                    dyParamAct.Add("@PAG_ID", SqlDbType.NVarChar, ParameterDirection.Input, pageID);
                    //dyParamAct.Add("@ACT_ID", SqlDbType.NVarChar, ParameterDirection.Input, item.PEL_CLICK);
                    var rs = SqlMapper.Query<SYPageActions>(conn, query, param: dyParamAct, commandType: CommandType.StoredProcedure).ToList();
                    dataActions = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataActions;
        }
        public List<SYPageActionDetails> GetPageActionDetail(int actID)
        {
            var dataActionDLs = new List<SYPageActionDetails>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParamActD = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParamActD.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "PageActionDetail");
                    dyParamActD.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, actID);
                    var rs = SqlMapper.Query<SYPageActionDetails>(conn, query, param: dyParamActD, commandType: CommandType.StoredProcedure);
                    dataActionDLs = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataActionDLs;
        }
        public SYDataMap GetDataMappingWithMapID(int mapID)
        {
            var dataMap = new SYDataMap();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParamMAP = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParamMAP.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "DataMapByMapID");
                    dyParamMAP.Add("@MAP_ID", SqlDbType.Int, ParameterDirection.Input, mapID);
                    var rs = SqlMapper.Query<SYDataMap>(conn, query, param: dyParamMAP, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    dataMap = rs;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return dataMap;
        }
        public async Task<List<SYToolbarActions>> GetToolbarActionsWithPageID(int PAG_ID)
        {
            var result = new List<SYToolbarActions>();
            var query = "SP_Web_SYDataPageToolbarActionsSPLayout";
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                        dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, "SelectWithPagID");
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PAG_ID);
                        var temp = await SqlMapper.QueryAsync<SYToolbarActions>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure).ConfigureAwait(true);
                        result = temp.ToList();
                        transaction.Commit();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                    }
                }
            }
            return result;
        }
        public List<SYToolbarActions> GetToolbarActionsNormalWithPageID(int PAG_ID)
        {
            var result = new List<SYToolbarActions>();
            var query = "SP_Web_SYDataPageToolbarActionsSPLayout";
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                        dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, "SelectWithPagID");
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PAG_ID);
                        var temp = SqlMapper.Query<SYToolbarActions>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        result = temp.ToList();
                        transaction.Commit();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                    }
                }
            }
            return result;
        }
        public List<SYClearElements> GetDataSelectedGridClearElements(int PAG_ID, int ACT_ID)
        {
            var result = new List<SYClearElements>();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_Web_SYClearElements";
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                        dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, "SELECT");
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PAG_ID);
                        dyParam.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, ACT_ID);
                        result = SqlMapper.Query<SYClearElements>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure).ToList();

                        transaction.Commit();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                    }
                }
            }
            return result;
        }
        public List<SYClearElements> GetDataClearElementsWithTypeForm(int PAG_ID, int ACT_ID, string PEL_ID, int PAG_ID_SRC)
        {
            var result = new List<SYClearElements>();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_Web_SYClearElements";
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                try
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, "SELECT_PEL_WITH_TYP");
                    dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PAG_ID);
                    dyParam.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, ACT_ID);
                    dyParam.Add("@PEL_ID", SqlDbType.VarChar, ParameterDirection.Input, PEL_ID);
                    dyParam.Add("@PAG_ID_SRC", SqlDbType.VarChar, ParameterDirection.Input, PAG_ID_SRC);
                    result = SqlMapper.Query<SYClearElements>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).ToList();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                }

            }
            return result;
        }
        public SYPageLayElements GetPageElementsWithPELID(string pelID)
        {
            var pageElement = new SYPageLayElements();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "ElementsWithPELID");
                    dyParam.Add("@PEL_ID", SqlDbType.NVarChar, ParameterDirection.Input, pelID);
                    var rs = SqlMapper.Query<SYPageLayElements>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    pageElement = rs.FirstOrDefault();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return pageElement;
        }
        #endregion

        #region Another view component
        public async Task<List<SYPageLayout>> GetPageLayoutWithType(string type)
        {
            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //{
            //    var pageLayout = conn.ExecuteQuery<List<SYPageLayout>>("SP_Web_DynamicPage",
            //        new string[] { "@DIV", "@PAG_TYPE" },
            //        new object[] { "PageLayoutWithType", type });
            //    return pageLayout;
            //}


            var pageLayout = new List<SYPageLayout>();
            //var conn = dbConnection.GetDbConnection();

            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    //get SYpagelayout
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "PageLayoutWithType");
                    dyParam.Add("@PAG_TYPE", SqlDbType.NVarChar, ParameterDirection.Input, type);
                    var rs = await SqlMapper.QueryAsync<SYPageLayout>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    pageLayout = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return pageLayout;
        }
        public List<SYPageLayElements> GetElementsWithPelID(string pelID, int pagID)
        {
            var pageElement = new List<SYPageLayElements>();
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
            var query = "SP_Web_DynamicPage";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                    dyParam.Add("@DIV", SqlDbType.NVarChar, ParameterDirection.Input, "PageElementsWithPelID");
                    dyParam.Add("@PEL_ID", SqlDbType.NVarChar, ParameterDirection.Input, pelID);
                    dyParam.Add("@PAG_ID", SqlDbType.NVarChar, ParameterDirection.Input, pagID);
                    var rs = SqlMapper.Query<SYPageLayElements>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    pageElement = rs.ToList();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
            }
            return pageElement;
        }
        public dynamic InitModelGridDynamic(string pelID, int pagID)
        {
            var lstElements = GetElementsWithPelID(pelID, pagID);
            //List<string> temp = new List<string>
            //{
            //    "obljA",
            //    "objB",
            //    "objC"
            //};
            List<Field> fields = new List<Field>();
            //for (int i = 0; i < reader.FieldCount; i++)
            //{
            //    var typeTemp = reader.GetFieldType(i).Name.Equals("DateTime") ? typeof(DateTime?) : reader.GetFieldType(i);
            //    //fields.Add(new Field { FieldName = reader.GetName(i), FieldType =  typeof(string) });
            //    fields.Add(new Field { FieldName = reader.GetName(i), FieldType = typeTemp });
            //    // fields.Add(new Field { FieldName = reader.GetName(i).ToLower(), FieldType = typeof(string) });
            //}
            foreach (var item in lstElements)
            {
                fields.Add(new Field { FieldName = item.PEL_ID, FieldType = typeof(string) });
            }
            var _class = MyTypeBuilder.CompileResultType(fields);
            var propts = _class.GetProperties();
            var model = Activator.CreateInstance(_class);
            foreach (var l in propts)
            {
                l.SetValue(model, null);
            }
            return model;
        }
        #endregion

        #region Execute SP Dynamic Render

        public int ExecuteProc2Count(string procName, ICollection<SPParameter> paras, string connectionType)
        {
            int totalCount = 0;
            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(connectionType))
                {

                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(connectionType)))
                    {
                        List<string> arrr = new List<string>();
                        List<object> varrr = new List<object>();
                        if (paras != null)
                        {

                            paras.Add(new SPParameter { Key = "Method", Value = "Count" });
                            foreach (var item in paras)
                            {
                                // Note: MSQL, SQL having @
                                // Oracle not have it
                                arrr.Add("@" + item.Key);
                                varrr.Add(item.Value);
                            }
                        }

                        var resultDs = conn.ExecuteQuery(procName, arrr.ToArray(), varrr.ToArray());

                        foreach (DataRow dataRow in resultDs.Tables[0].Rows)
                        {
                            totalCount = (int)dataRow["Count"];
                        }

                        return totalCount;
                    }
                }
                return totalCount;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public dynamic ExecuteProc2(string procName, ICollection<SPParameter> paras, string connectionType)
        {
            var rtnList = new List<object>();
            //IDbCommand cmd = conn.CreateCommand();
            //cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //cmd.CommandText = procName;
            ///TODO: Get parameter od sp
            ///SELECT *
            ///FROM SYS.ALL_ARGUMENTS
            ///where object_name = 'SP_WEB_VIEW_FORM' order by position asc
            ///

            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(connectionType))
                {

                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(connectionType)))
                    {
                        List<string> arrr = new List<string>();
                        List<object> varrr = new List<object>();
                        if (paras != null)
                        {
                            foreach (var item in paras)
                            {
                                // Note: MSQL, SQL having @
                                // Oracle not have it
                                arrr.Add("@" + item.Key);
                                varrr.Add(item.Value);
                            }
                        }

                        var result = conn.ExecuteQuery2(procName, arrr.ToArray(), varrr.ToArray());
                        List<Field> fields = new List<Field>();
                        foreach (var item in result)
                        {
                            foreach (var item1 in item)
                            {
                                fields.Add(new Field { FieldName = item1.Key, FieldType = item1.Type });
                            }

                        }

                        //Quan test
                        var number = result.FirstOrDefault();
                        List<Field> fields_1 = new List<Field>();
                        if (result.Count > 0)
                        {
                            for (int i = 0; i < number.Count; i++)
                            {
                                fields_1.Add(new Field { FieldName = number[i].Key, FieldType = number[i].Type });
                            }
                        }
                        // end Quan test

                        var fieldNames = fields.Select(x => x.FieldName);
                        /// var _class = MyTypeBuilder.CompileResultType(fields);
                        var _class = MyTypeBuilder.CompileResultType(fields_1);
                        var propts = _class.GetProperties();
                        object val;
                        //var model = Activator.CreateInstance(_class);
                        foreach (var item in result)
                        {
                            var model = Activator.CreateInstance(_class);
                            foreach (var l in propts)
                            {
                                var temp = item.Where(m => m.Key == l.Name).FirstOrDefault();
                                if (temp != null)
                                {
                                    l.SetValue(model, temp.Value);
                                }
                                else
                                {
                                    l.SetValue(model, null);
                                }

                            }
                            rtnList.Add(model);
                        }
                    }
                }
                return rtnList;
            }
            catch (Exception e)
            {
                throw e;
            }

            //var spInfor = dbConnection.GetSPInfor(procName);

            //foreach (var info in spInfor)
            //{
            //    var p = paras.FirstOrDefault(x => info.argument_name.ToUpper().EndsWith(x.Key.ToUpper()));
            //    p = p ?? new SPParameter();
            //    IDataParameter parameter = dbConnection.CreateDataParameter(info, p, DbmsTypes.Mssql);
            //    cmd.Parameters.Add(parameter);
            //}

            //try
            //{
            //    if (conn.State == System.Data.ConnectionState.Closed || conn.State == System.Data.ConnectionState.Broken)
            //    {
            //        conn.Open();
            //    }

            //    using (var reader = cmd.ExecuteReader())
            //    {
            // List<Field> fields = new List<Field>();
            //        for (int i = 0; i < reader.FieldCount; i++)
            //        {
            //            var typeTemp = reader.GetFieldType(i).Name.Equals("DateTime") ? typeof(DateTime?) : reader.GetFieldType(i);
            //            //fields.Add(new Field { FieldName = reader.GetName(i), FieldType =  typeof(string) });
            //            fields.Add(new Field { FieldName = reader.GetName(i), FieldType = typeTemp });
            //            // fields.Add(new Field { FieldName = reader.GetName(i).ToLower(), FieldType = typeof(string) });
            //        }
            //        var fieldNames = fields.Select(x => x.FieldName);
            //        var _class = MyTypeBuilder.CompileResultType(fields);
            //        var propts = _class.GetProperties();
            //        //T model;
            //        object val;
            //        while (reader.Read())
            //        {
            //            var model = Activator.CreateInstance(_class);
            //            foreach (var l in propts)
            //            {
            //                if (fieldNames.Contains(l.Name))
            //                    val = reader[l.Name];
            //                else val = DBNull.Value;
            //                //val = reader[l.Name];
            //                if (val == DBNull.Value)
            //                    l.SetValue(model, null);
            //                else
            //                    //l.SetValue(model, val.ToString());
            //                    l.SetValue(model, val);
            //            }
            //            rtnList.Add(model);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            //finally
            //{
            //    conn.Close();
            //}
        }

        public Result ExecuteProCRUD(string procName, ICollection<SPParameter> paras)
        {
            var res = new Result();

            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = procName;

            // Get list Param of Store procedure
            var spInfor = dbConnection.GetSPInfor(procName);

            // foreach param in store procedure
            foreach (var info in spInfor)
            {
                var p = paras.FirstOrDefault(x => !string.IsNullOrEmpty(x.Key) && info.argument_name.ToUpper().EndsWith(x.Key.ToUpper()));
                p = p ?? new SPParameter();
                IDataParameter parameter = dbConnection.CreateDataParameter(info, p, DbmsTypes.Mssql);
                cmd.Parameters.Add(parameter);
            }

            try
            {
                if (conn.State == System.Data.ConnectionState.Closed || conn.State == System.Data.ConnectionState.Broken)
                {
                    conn.Open();
                }


                IDbDataAdapter ad;
                //ad.Update(); // batch execution
                var countResult = cmd.ExecuteNonQuery();
                if (countResult > 0)
                {
                    res.Success = true;
                }
                //foreach (IDataParameter param in cmd.Parameters)
                //{
                //    if (param.Direction == ParameterDirection.Output && param.ParameterName.ToUpper().Contains("message".ToUpper()))
                //    {
                //        res.Message = !string.IsNullOrEmpty(param.Value.ToString()) ? param.Value.ToString() : "";
                //        res.IsSuccess = res.Message == "OK";
                //    }
                //}
            }
            catch (Exception ex)
            {
                res.Message = ex.ToString();
            }
            finally
            {
                conn.Close();
            }

            return res;
        }
        #endregion

        #region EXPORT EXCEL BY PVN

        public string ExportExcelDynamic(DataTable dt, string fileName,string spname)
        {
            var log = new LogWriter($"Download File Dynamic: {fileName}");
            log.LogWrite("Download File Dynamic Start");

            FileInfo newExcelFile;
            string curDate = DateTime.Today.ToString("yyyyMM");
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", curDate);
            var tempFilePath = Path.Combine(tempPath, EXCEL_EXPORT_FOLDER);
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            if (dt.Rows.Count == 0) return "";

            using (var excel = new ExcelPackage())
            {
                try
                {
                    var workSheet = excel.Workbook.Worksheets.Add(fileName);

                    workSheet.Cells.LoadFromDataTable(dt, true);

                    int colNumber = 1;

                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.DataType == typeof(DateTime))
                        {
                            //workSheet.Column(colNumber).Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss AM/PM";
                            workSheet.Column(colNumber).Style.Numberformat.Format = "yyyy-MM-dd";
                        }
                        if (col.DataType == typeof(Int64) || col.DataType == typeof(Double) || col.DataType == typeof(Decimal))
                        {
                            workSheet.Column(colNumber).Style.Numberformat.Format = "#,###";
                        }
                        colNumber++;
                    }

                    workSheet.Cells[1, 1, 1, dt.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    workSheet.Cells[1, 1, 1, dt.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1, 1, dt.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    // Quan add 2021-03-29
                    //SetHeaderColor(workSheet, spname, dt.Columns.Count);

                    workSheet.Cells.AutoFitColumns();

                    newExcelFile = new FileInfo(tempFilePath + fileName + "_" + DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT) + ".xlsx");
                    excel.SaveAs(newExcelFile);

                }
                catch (Exception ex)
                {
                    log.LogWrite("tempFilePath + excelFileName : ex" + ex.ToString());
                    throw;
                }
            }
            //return tempFilePath + fileName + "_" + DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT) + ".xlsx";
            return newExcelFile.FullName;
        }

        private void SetHeaderColor(ExcelWorksheet ws, string PageName, int columnCount)
        {
            switch(PageName)
            {
                case "SP_INVENTORY_STATUS_EXPORT_EXCEL_DYNAMIC_CHANGE":
                    ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    ws.Cells["A1"].Style.Font.Color.SetColor(Color.White);
                    ws.Cells["A1"].Style.Font.Bold = true;

                    ws.Cells["G1"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    ws.Cells["G1"].Style.Font.Color.SetColor(Color.White);
                    ws.Cells["G1"].Style.Font.Bold = true;                 

                    ws.Cells["J1"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    ws.Cells["J1"].Style.Font.Color.SetColor(Color.White);
                    ws.Cells["J1"].Style.Font.Bold = true;

                    ws.Cells["K1"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    ws.Cells["K1"].Style.Font.Color.SetColor(Color.White);
                    ws.Cells["K1"].Style.Font.Bold = true;

                    ws.Cells["Q1"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    ws.Cells["Q1"].Style.Font.Color.SetColor(Color.White);
                    ws.Cells["Q1"].Style.Font.Bold = true;

                    ws.Cells["R1"].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    ws.Cells["R1"].Style.Font.Color.SetColor(Color.White);
                    ws.Cells["R1"].Style.Font.Bold = true;

                    break;
                default:
                    ws.Cells[1, 1, 1, columnCount].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    break;
            }
        }
        #endregion

        #region IMPORT EXCEL DYNAMIC

        public Result UploadFileDynamicForImport(IFormFile myFile, Type type, string Lang, List<SPParameter> spParam, string SPName, string UserID)
        {
            Result result = new Result();
            var tempPath = "";
            string curDate = DateTime.Today.ToString("yyyyMM");
            tempPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", curDate);

            string FileGuid = Guid.NewGuid().ToString();

            var tempFilePath = Path.Combine(tempPath, FileGuid + ".tmp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            //Copy file to local
            ExcelExport.AppendContentToFile(tempFilePath, myFile);
            ExcelExport.ProcessUploadedFile(tempFilePath, FileGuid + myFile.FileName, tempPath);
            ExcelExport.RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 5, 0));

            string fileName = FileGuid + myFile.FileName;

            ExcelHelperTest excelHelperTest = new ExcelHelperTest();
            // Get data from excecl to DataTable
            var dt = excelHelperTest.ReadFromExcelfile(tempPath + "\\" + fileName, null, null);

            //Change Column Name To Column Name in DB
            foreach (DataColumn colName in dt.Columns)
            {
                foreach (var spPara in spParam)
                {
                    if (colName.ColumnName == spPara.Key)
                    {
                        colName.ColumnName = spPara.Value;
                    }
                }
            }

            string jsonObj = JsonConvert.SerializeObject(dt);

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var resultInsDtl = 0;
                        resultInsDtl = conn.ExecuteNonQuery(SPName, CommandType.StoredProcedure,
                                new string[] { "@ListObj", "@UserID" },
                                new object[] { jsonObj, UserID },
                                transaction);

                        if (resultInsDtl > 0)
                        {
                            transaction.Commit();
                            result.Success = true;
                            result.Message = MessageCode.MD0004;
                        }
                        else
                        {
                            transaction.Rollback();
                            result.Success = false;
                            result.Message = MessageCode.MD0005;
                        }

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = MessageCode.MD0005 + ex.ToString(),
                        };
                    }

                }
            }

            return result;
        }

        #endregion

        #region POPUP UPLOAD FILE
        //private const int MAX_EXCEL_ROW = 1000000000;

        public DynamicPopupModel GetMenuImportAction(int PageID, int PageAct, string PageKey)
        {
            string SP_GET_SYMENU_IMPORT_ACTION = "SP_GET_SYMENU_IMPORT_ACTION";
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@PageID";
                arrParams[2] = "@PageAct";
                arrParams[3] = "@PageKey";
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetAction";
                arrParamsValue[1] = PageID;
                arrParamsValue[2] = PageAct;
                arrParamsValue[3] = PageKey;
                var result = conn.ExecuteQuery<DynamicPopupModel>(SP_GET_SYMENU_IMPORT_ACTION, arrParams, arrParamsValue).FirstOrDefault();

                return result;
            }
        }

        public List<SPParameter> GetListParam(int ActID, int PageID)
        {
            string SP_GET_SYMENU_IMPORT_ACTION = "SP_GET_SYMENU_IMPORT_ACTION";
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@PageID";
                arrParams[2] = "@PageAct";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetParams";
                arrParamsValue[1] = PageID;
                arrParamsValue[2] = ActID;
                var result = conn.ExecuteQuery<SPParameter>(SP_GET_SYMENU_IMPORT_ACTION, arrParams, arrParamsValue);

                return result.ToList();
            }
        }


        /// <summary>
        /// Added By PVN
        /// Date Added: 2020-11-16
        /// Description: Save file upload to server
        /// </summary>
        /// <param name="myFile"></param>
        /// <param name="chunkMetadata"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UploadFileDynamicForImportFromPopup(IFormFile myFile, string chunkMetadata, Type type)
        {
            var tempPath = "";
            string fileName = "";
            string curDate = DateTime.Today.ToString("yyyyMM");
            tempPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", curDate);
            if (!string.IsNullOrEmpty(chunkMetadata))
            {
                var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                // CheckFileExtensionValid(metaDataObject.FileName);                    
                // Uncomment to save the file
                var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + ".tmp");
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);

                ExcelExport.AppendContentToFile(tempFilePath, myFile);

                if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                {
                    ExcelExport.ProcessUploadedFile(tempFilePath, metaDataObject.FileGuid + metaDataObject.FileName, tempPath);
                    ExcelExport.RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 5, 0));
                    //file = mapper.Map<SYFileUpload>(metaDataObject);
                    //rs = filesService.InsertSYFileUpload(file);
                }

                fileName = metaDataObject.FileGuid + metaDataObject.FileName;

               
            }

            return tempPath + "\\" + fileName;
        }

        private Type GetDataType(string typeName)
        {
            if(!string.IsNullOrEmpty(typeName))
            {
                switch (typeName)
                {
                    case "Decimal": return Type.GetType("System.Decimal, System.Private.CoreLib");
                    case "Double": return Type.GetType("System.Double, System.Private.CoreLib");
                    case "Char": return Type.GetType("System.String, System.Private.CoreLib");
                    case "Integer": return Type.GetType("System.Int32, System.Private.CoreLib");
                    case "Bit": return Type.GetType("System.Boolean, System.Private.CoreLib");
                    case "DateTime": return Type.GetType("System.DateTime, System.Private.CoreLib");
                    default: return Type.GetType("System.String, System.Private.CoreLib");
                }
            }    
            else
            {
                return Type.GetType("System.String, System.Private.CoreLib");
            }    
        }

        /// <summary>
        /// Added By PVN
        /// Date Added: 2020-11-17
        /// Description: Import data from excel file to database
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="SPName"></param>
        /// <param name="spParam"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public Result SaveToDB_DynamicData_Excel(string filePath, string SPName, List<SPParameter> spParam, string UserID)
        {
            Result result = new Result();
            ExcelHelperTest excelHelperTest = new ExcelHelperTest();
            // Get data from excecl to DataTable
            DataTable dt = excelHelperTest.ReadFromExcelfile(filePath, null, null);
            // Add DataTable to Dataset

            foreach (DataColumn colName in dt.Columns)
            {
                foreach (var spPara in spParam)
                {
                    if (colName.ColumnName == spPara.Value)
                    {
                        colName.ColumnName = spPara.Key;

                        foreach (DataRow row in dt.Rows)
                        {
                            if(Type.GetType(colName.DataType.AssemblyQualifiedName) != GetDataType(spPara.DataType))
                            {
                                if(spPara.DataType == "Double" || spPara.DataType == "Integer")
                                {
                                    if(String.IsNullOrEmpty(row[colName.ColumnName].ToString()))
                                    {
                                        row[colName.ColumnName] = Convert.ChangeType(0, GetDataType(spPara.DataType));
                                    }
                                    else
                                    {
                                        row[colName.ColumnName] = Convert.ChangeType(row[colName.ColumnName].ToString(), GetDataType(spPara.DataType));
                                    }
                                }
                                else if(spPara.DataType == "DateTime")
                                {
                                    if (String.IsNullOrEmpty(row[colName.ColumnName].ToString()))
                                    {
                                        row[colName.ColumnName] = Convert.ChangeType(DateTime.Now, GetDataType(spPara.DataType));
                                    }
                                    else
                                    {
                                        row[colName.ColumnName] = Convert.ChangeType(row[colName.ColumnName].ToString(), GetDataType(spPara.DataType));
                                    }
                                }
                                else
                                {
                                    row[colName.ColumnName] = Convert.ChangeType(row[colName.ColumnName].ToString(), GetDataType(spPara.DataType));
                                }
                            }    
                        }
                    }
                }
            }

            int sendingTimes = (dt.Rows.Count / 50) + 1;
            for (int i = 0; i < sendingTimes; i++)
            {
                string jsonObjTest;
                DataTable dtInsert;
                if (i == 0)
                {
                    dtInsert = dt.AsEnumerable().Skip(0).Take(50).CopyToDataTable();
                    jsonObjTest = JsonConvert.SerializeObject(dtInsert);
                }
                else
                {
                    dtInsert = dt.AsEnumerable().Skip(50 * i).Take(50).CopyToDataTable();
                    jsonObjTest = JsonConvert.SerializeObject(dtInsert);
                }

                if(!string.IsNullOrEmpty(jsonObjTest))
                {
                    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                var resultInsDtl = 0;
                                resultInsDtl = conn.ExecuteNonQuery(SPName, CommandType.StoredProcedure,
                                        new string[] { "@ListObj", "@UserID" },
                                        new object[] { jsonObjTest, UserID },
                                        transaction);

                                if (resultInsDtl > 0)
                                {
                                    transaction.Commit();
                                    result.Success = true;
                                    result.Message = MessageCode.MD0004;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    result.Success = false;
                                    result.Message = MessageCode.MD0005;
                                }

                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                LogWriter log = new LogWriter("Insert batch data failed");
                                log.LogWrite(ex.Message);
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0005 + ex.ToString(),
                                };
                            }

                        }
                    }
                }
            }

            //string jsonObj = JsonConvert.SerializeObject(dt);

            //using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            //{
            //    using (var transaction = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            var resultInsDtl = 0;
            //            resultInsDtl = conn.ExecuteNonQuery(SPName, CommandType.StoredProcedure,
            //                    new string[] { "@ListObj", "@UserID" },
            //                    new object[] { jsonObj, UserID },
            //                    transaction);

            //            if (resultInsDtl > 0)
            //            {
            //                transaction.Commit();
            //                result.Success = true;
            //                result.Message = MessageCode.MD0004;
            //            }
            //            else
            //            {
            //                transaction.Rollback();
            //                result.Success = false;
            //                result.Message = MessageCode.MD0005;
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            transaction.Rollback();
            //            LogWriter log = new LogWriter("Insert batch data failed");
            //            log.LogWrite(ex.Message);
            //            return new Result
            //            {
            //                Success = false,
            //                Message = MessageCode.MD0005 + ex.ToString(),
            //            };
            //        }

            //    }
            //}

            return result;
        }

        public void ImportDataToTemplateExcelFile(string pageName, string filePath)
        {
            FileInfo excelTemplate = new FileInfo(filePath);
            switch (pageName)
            {
                case "ITEMS_MGT":
                    ItemMgtTemplateExcelFile(excelTemplate);
                    break;
                case "ITEM_PARTNER_INFO":
                    ItemPartnerTemplateExcelFile(excelTemplate);
                    break;
                default: break;
            }    
        }

        private void ItemMgtTemplateExcelFile(FileInfo pathFile)
        {
            using (var excel = new ExcelPackage(pathFile))
            {
                var category         = GetComCodeDtls().Where(m => m.GROUP_CD == "IMTP00").ToList();
                var itemClassCode    = GetItemClass();
                var monetaryUnit     = GetComCodeDtls().Where(m => m.GROUP_CD == "MOUT00").ToList();
                var unit             = GetComCodeDtls().Where(m => m.GROUP_CD == "ITUN00").ToList();
                var leadTimeType     = GetComCodeDtls().Where(m => m.GROUP_CD == "LDTM00").ToList();
                var inspectionMethod = GetComCodeDtls().Where(m => m.GROUP_CD == "ICPM00").ToList();
                var partner          = GetItemPartner();
                var warehouse        = GetWarehouse();
                int row = 3;

                ExcelWorksheet templateSheet = excel.Workbook.Worksheets["Template"];
                ExcelWorksheet commonDataSheet = excel.Workbook.Worksheets["CommonDataSheet"];

                //Remove dropdown data from cell A2 to cell M2
                var AllDataValidation = templateSheet.DataValidations["A2:M2"];
                if (AllDataValidation != null)
                {
                    foreach (var item in templateSheet.DataValidations)
                    {
                        templateSheet.DataValidations.Remove(item);
                    }
                }

                #region ADD DATA TO CommonDataSheet
                //clear all data in excel file from cell P5 
                commonDataSheet.Cells[$"A3:W{row + partner.Count - 1}"].Clear();

                InsertCommonDataToExcel("A2", category, row, 1, 2, templateSheet, "B", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("B2", itemClassCode, row, 4, 5, templateSheet, "E", "ItemClassCode", "ClassNameKor", commonDataSheet);
                InsertCommonDataToExcel("E2", partner, row, 7, 8, templateSheet, "H", "PartnerCode", "PartnerName", commonDataSheet);
                InsertCommonDataToExcel("G2", monetaryUnit, row, 10, 11, templateSheet, "K", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("M2", unit, row, 13, 14, templateSheet, "N", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("H2", leadTimeType, row, 16, 17, templateSheet, "Q", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("J2", inspectionMethod, row, 19, 20, templateSheet, "T", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("K2", warehouse, row, 22, 23, templateSheet, "W", "WarehouseCode", "WarehouseName", commonDataSheet);

                commonDataSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

                #endregion

                excel.Save();
            }    
        }

        private void ItemPartnerTemplateExcelFile(FileInfo pathFile)
        {
            using (var excel = new ExcelPackage(pathFile))
            {
                var partner = GetPartnerNew();
                var leadTimeType = GetComCodeDtls().Where(m => m.GROUP_CD == "LDTM00").ToList();
                var monetaryUnit = GetComCodeDtls().Where(m => m.GROUP_CD == "MOUT00").ToList();
                var itemCode = GetItems();
                int row = 3;

                ExcelWorksheet templateSheet = excel.Workbook.Worksheets["Template"];
                ExcelWorksheet commonDataSheet = excel.Workbook.Worksheets["CommonDataSheet"];

                //Remove dropdown data from cell A2 to cell M2
                var AllDataValidation = templateSheet.DataValidations["A2:M2"];
                if (AllDataValidation != null)
                {
                    foreach (var item in templateSheet.DataValidations)
                    {
                        templateSheet.DataValidations.Remove(item);
                    }
                }

                #region ADD DATA TO CommonDataSheet
                //clear all data in excel file from cell P5 
                commonDataSheet.Cells[$"A3:W{row + partner.Count - 1}"].Clear();

                InsertCommonDataToExcel("A2", itemCode, row, 1, 2, templateSheet, "B", "ItemCode", "ItemName", commonDataSheet);
                InsertCommonDataToExcel("B2", partner, row, 7, 8, templateSheet, "H", "PartnerCode", "PartnerName", commonDataSheet);
                InsertCommonDataToExcel("C2", leadTimeType, row, 16, 17, templateSheet, "Q", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("E2", monetaryUnit, row, 10, 11, templateSheet, "K", "BASE_CODE", "BASE_NAME", commonDataSheet);

                commonDataSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

                #endregion

                excel.Save();
            }
        }

        /// <summary>
        /// Added By PVN
        /// Date Added: 2020-11-20
        /// Description: Write Common Data To Excel File And Create Dropdown List Base On It
        /// </summary>
        private void InsertCommonDataToExcel<T>(string colDropdownName, 
            IList<T> listData, int startRow, int startCol, int endCol, 
            ExcelWorksheet templateSheet, string dataColName, string propCode, string propName, ExcelWorksheet dataSheet)
        {
            try
            {
                //CommonDataSheet!$B$3:$B$6
                var dropdown = templateSheet.Cells[colDropdownName].DataValidation.AddListDataValidation();
                for (int i = 0; i < listData.Count; i++)
                {
                    dataSheet.Cells[startRow + i, startCol].Value = listData[i].GetType().GetProperty(propCode).GetValue(listData[i], null);
                    dataSheet.Cells[startRow + i, endCol].Value = listData[i].GetType().GetProperty(propName).GetValue(listData[i], null);
                }
                dropdown.Formula.ExcelFormula = $"{dataSheet.Name}!${dataColName}${startRow}:${dataColName}${(startRow + listData.Count - 1)}";
            }
            catch(Exception ex)
            {
                LogWriter log = new LogWriter(ex.Message);
            }
            
        }
        #endregion

        #region GET COMMON DATA
        private const string SP_MES_GET_COMMON_DATA = "SP_MES_GET_COMMON_DATA";

        private List<MES_ComCodeDtls> GetComCodeDtls()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeDtls>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetComCodeDtls" }).ToList();

                return result;
            }
        }

        private List<MES_ItemClass> GetItemClass()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetItemClass" }).ToList();

                return result;
            }
        }

        private List<MES_Partner> GetItemPartner()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetItemPartner" }).ToList();

                return result;
            }
        }

        private List<MES_Warehouse> GetWarehouse()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Warehouse>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetWarehouse" }).ToList();

                return result;
            }
        }

        private List<MES_Item> GetItems()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetItem" }).ToList();

                return result;
            }
        }

        private List<MES_Partner> GetPartnerNew()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetPartnerNew" }).ToList();

                return result;
            }
        }

        #endregion
    }
}

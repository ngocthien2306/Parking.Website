using Dapper;
using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using Modules.Admin.Services.IService;
using Modules.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Modules.Common.Models;
using Modules.Common;
using Modules.Common.Utils;
using InfrastructureCore.Models.Identity;

namespace Modules.Admin.Services.ServiceImp
{
    public class AdminLayoutService : IAdminLayoutService
    {
        #region Properties
        IDBContextConnection dbConnection;
        IDbConnection conn;
        #endregion

        #region Constructor
        public AdminLayoutService(IDBContextConnection dbConnection)
        {
            this.dbConnection = dbConnection;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }
        #endregion

        #region Store Procedure Constant
        private const string SP_WEB_DYNAMIC_PAGE = "SP_Web_DynamicPage";
        private const string SP_WEB_PAGE_RELATIONSHIP_LAYOUT = "SP_Web_SYPageRelationshipLayout";


        private const string SP_WEB_SY_COMMON_CODE = "SP_Web_SYComCode";
        private const string SP_WEB_SY_PAGE_LAYOUT = "SP_Web_SYPageLayout";
        private const string SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP_FRAMEWORK = "GetComboboxValueDynamicBySP";

        private const string SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP = "SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP";
        private const string SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP_IN_GRID = "SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP_IN_GRID";
        private const string SP_GET_RADIO_CHECKBOX_VALUE_DYNAMIC_BY_SP = "SP_GET_RADIO_CHECKBOX_VALUE_DYNAMIC_BY_SP";




        #endregion

        #region Page layout master
        public Result SaveDataPageLayout(SYPageLayout item, string action, SYLoggedUser info)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string actionType = "";
                if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                {
                    actionType = CommonAction.INSERT;
                }
                else
                {
                    actionType = CommonAction.UPDATE;
                }
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYPageLayout",
                        new string[] { "@DIV", "@PAG_ID", "@PAG_KEY", "@PAG_TYPE", "@PAG_TITLE", "@PAG_WDT", "@PAG_HGT", "@CUSTM_VIEW", "@SITE_ID" },
                        new object[] { actionType, item.PAG_ID, item.PAG_KEY, item.PAG_TYPE, item.PAG_TITLE, item.PAG_WDT, item.PAG_HGT, item.CUSTM_VIEW, info.SiteID });
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

        public Result DeletePageLayout(SYPageLayout item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYPageLayout",
                        new string[] { "@DIV", "@PAG_ID", "@PAG_KEY" },
                        new object[] { CommonAction.DELETE, item.PAG_ID, item.PAG_KEY });
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

        public List<SYPageLayout> SelectPageLayout()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYPageLayout>("SP_Web_SYPageLayout",
                    new string[] { "@DIV" },
                    new object[] { CommonAction.SELECT });
                return result.ToList();
            }
        }

        public Result SaveDataListControls(SYPageLayElements item, string action)
        {

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string actionType = "";
                if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                {
                    actionType = CommonAction.INSERT;
                }
                else
                {
                    actionType = CommonAction.UPDATE;
                }
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYPageLayElements",
                        new string[] { "@DIV", "@ID", "@PAG_ID", "@PEL_ID", "@PEL_LBL", "@PEL_TYP","@PEL_PRN", "@PEL_DAT_TYPE", "@PEL_COL", "@PEL_ROW", "@PEL_CSPN",
                                        "@PEL_RSPN", "@PEL_WDT", "@PEL_HGT", "@PEL_VIS", "@PEL_MAPYN", "@PEL_BINDYN", "@PEL_SEQ", "@PEL_DFVALUE", "@PEL_SM",
                                        "@PEL_MD", "@PEL_LG", "@PEL_FIX", "@PEL_FORL", "@PEL_ALGN", "@PEL_CLICK", "@PEL_DBLCLICK", "@PEL_REF_PAG_ID","@CUSTM_VIEW"},
                        new object[] { actionType, item.ID, item.PAG_ID, item.PEL_ID, item.PEL_LBL, item.PEL_TYP, item.PEL_PRN, item.PEL_DAT_TYPE, item.PEL_COL,
                                        item.PEL_ROW, item.PEL_CSPN, item.PEL_RSPN, item.PEL_WDT, item.PEL_HGT, item.PEL_VIS, item.PEL_MAPYN, item.PEL_BINDYN,
                                        item.PEL_SEQ, item.PEL_DFVALUE, item.PEL_SM, item.PEL_MD, item.PEL_LG, item.PEL_FIX, item.PEL_FORL, item.PEL_ALGN, item.PEL_CLICK,
                                        item.PEL_DBLCLICK, item.PEL_REF_PAG_ID, item.CUSTM_VIEW});
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

        public List<SYPageLayout> GetPageELWithType(string type)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYPageLayout>("SP_Web_DynamicPage",
                    new string[] { "@DIV", "@PAG_TYPE" },
                    new object[] { "PageLayoutWithType", type });
                return result.ToList();
            }
        }
        #endregion

        #region Page element
        public List<SYPageLayElements> SelectPageLayoutElement(string PAG_ID, string PEL_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYPageLayElements>("SP_Web_SYPageLayElements",
                        new string[] { "@DIV", "@PAG_ID", "@PEL_ID" },
                        new object[] { CommonAction.SELECT, PAG_ID, PEL_ID });
                    return result.ToList();
                }
            }
            catch 
            {
                throw;
            }
        }

        public Result SaveDataElementControls(SYPageLayElements item, string action)
        {
            int result = -1;
            var query = "SP_Web_SYPageLayElements";
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

                        // check form or grid, form cannot set grid click, grid DBclick
                        //form
                        if (item.PEL_TYP != null && item.PEL_TYP.Equals("C001"))
                        {
                            if (item.PEL_CLICK != null || item.PEL_DBLCLICK != null)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = "Cannot save setting grid action click row for page type Form."
                                };
                            }
                        }

                        // Cannot existed Groupcode and groupcode custom Sp at the same time
                        if (item != null && item.GRP_CD != null && item.GRP_CD_CUSTOM != null)
                        {
                            return new Result
                            {
                                Success = false,
                                Message = "Page element has only existed Group code setting or Store procedure."
                            };
                        }

                        // Cannot existed Pel ID duplicate at the same time
                        if (item != null && item.PEL_ID != null)
                        {
                            var checkPelIDExistedQuery = "SELECT * FROM SYPageLayElements WHERE PAG_ID = " + item.PAG_ID + " AND PEL_ID = '" + item.PEL_ID + "'" + " AND ID <> " + item.ID
                                 + " AND PEL_PRN = '" + item.PEL_PRN + "'";
                            var checkPelIDExisted = SqlMapper.Query<SYPageLayElements>(conn, checkPelIDExistedQuery, transaction: transaction, commandType: CommandType.Text).ToList();

                            if (checkPelIDExisted != null && checkPelIDExisted.Count() >= 1)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = "Page element has only one Page element ID."
                                };
                            }
                        }

                        if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                        {
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.INSERT);
                        }
                        else
                        {
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.UPDATE);
                        }

                        dyParam.Add("@ID", SqlDbType.BigInt, ParameterDirection.Input, item.ID);
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PAG_ID);
                        dyParam.Add("@PEL_ID", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_ID);
                        dyParam.Add("@PEL_LBL", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_LBL);
                        dyParam.Add("@PEL_TYP", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_TYP != null ?  "G002" + item.PEL_TYP : ""); // add group code                     

                        dyParam.Add("@PEL_PRN", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_PRN);// add child parent
                        dyParam.Add("@PEL_DAT_TYPE", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_DAT_TYPE != null ? "G003" + item.PEL_DAT_TYPE : ""); // add group code 
                        dyParam.Add("@PEL_LEN", SqlDbType.Int, ParameterDirection.Input, item.PEL_LEN);
                        dyParam.Add("@PEL_COL", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_ROW", SqlDbType.Int, ParameterDirection.Input, item.PEL_ROW);
                        dyParam.Add("@PEL_CSPN", SqlDbType.Int, ParameterDirection.Input, item.PEL_CSPN);
                        dyParam.Add("@PEL_RSPN", SqlDbType.Int, ParameterDirection.Input, item.PEL_RSPN);
                        dyParam.Add("@PEL_WDT", SqlDbType.Int, ParameterDirection.Input, item.PEL_WDT);
                        dyParam.Add("@PEL_HGT", SqlDbType.Int, ParameterDirection.Input, item.PEL_HGT);
                        dyParam.Add("@PEL_VIS", SqlDbType.Bit, ParameterDirection.Input, item.PEL_VIS);
                        dyParam.Add("@PEL_MAPYN", SqlDbType.Bit, ParameterDirection.Input, item.PEL_MAPYN);
                        dyParam.Add("@PEL_BINDYN", SqlDbType.Bit, ParameterDirection.Input, item.PEL_BINDYN);
                        dyParam.Add("@PEL_SEQ", SqlDbType.Int, ParameterDirection.Input, item.PEL_SEQ);
                        dyParam.Add("@PEL_DFVALUE", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_DFVALUE);
                        // Responsive with PEL_COL
                        dyParam.Add("@PEL_XS", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_SM", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_MD", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_LG", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);

                        dyParam.Add("@PEL_FIX", SqlDbType.Bit, ParameterDirection.Input, item.PEL_FIX);
                        dyParam.Add("@PEL_FORL", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_FORL);
                        dyParam.Add("@PEL_ALGN", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_ALGN != null ? "G008" + item.PEL_ALGN : "");// add group code 
                        dyParam.Add("@PEL_CLICK", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_CLICK);
                        dyParam.Add("@PEL_DBLCLICK", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_DBLCLICK);
                        dyParam.Add("@PEL_REF_PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PEL_REF_PAG_ID);

                        // 2020-04-29: Minh add 
                        dyParam.Add("@IS_KEY", SqlDbType.Bit, ParameterDirection.Input, item.IS_KEY);
                        dyParam.Add("@IS_EDIT", SqlDbType.Bit, ParameterDirection.Input, item.IS_EDIT);
                        // 2021-03-07:Quan add
                        dyParam.Add("@IS_CREATE", SqlDbType.Bit, ParameterDirection.Input, item.IS_CREATE);
                        dyParam.Add("@IS_UPDATE", SqlDbType.Bit, ParameterDirection.Input, item.IS_UPDATE);
                        dyParam.Add("@IS_DELETE", SqlDbType.Bit, ParameterDirection.Input, item.IS_DELETE);
                        // End
                        dyParam.Add("@EDIT_TYPE", SqlDbType.VarChar, ParameterDirection.Input, item.EDIT_TYPE);
                        dyParam.Add("@EDIT_ACT", SqlDbType.Int, ParameterDirection.Input, item.EDIT_ACT);
                        dyParam.Add("@PEL_EXP_TEXT", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_EXP_TEXT);
                        dyParam.Add("@GRP_CD", SqlDbType.VarChar, ParameterDirection.Input, item.GRP_CD);
                        dyParam.Add("@GRP_CD_CUSTOM", SqlDbType.VarChar, ParameterDirection.Input, item.GRP_CD_CUSTOM);
                        dyParam.Add("@PEL_IS_REQUIRED", SqlDbType.Bit, ParameterDirection.Input, item.PEL_IS_REQUIRED);
                        dyParam.Add("@CONNECTION_NM", SqlDbType.VarChar, ParameterDirection.Input, item.CONNECTION_NM != null ? "G013" + item.CONNECTION_NM : "");
                        dyParam.Add("@CUSTM_VIEW", SqlDbType.VarChar, ParameterDirection.Input, item.CUSTM_VIEW);
                        dyParam.Add("@PEL_VALIDATE_RULE_API", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_VALIDATE_RULE_API);
                        dyParam.Add("@PEL_VALIDATE_RULE_API_MSG", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_VALIDATE_RULE_API_MSG);
                        dyParam.Add("@PEL_VALIDATE_REGULAR_EXP", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_VALIDATE_REGULAR_EXP);
                        dyParam.Add("@PEL_VALIDATE_REGULAR_EXP_MSG", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_VALIDATE_REGULAR_EXP_MSG);
                        dyParam.Add("@LST_PEL_REFER", SqlDbType.VarChar, ParameterDirection.Input, item.LST_PEL_REFER);
                        dyParam.Add("@SP_CUSTOM_REFER", SqlDbType.VarChar, ParameterDirection.Input, item.SP_CUSTOM_REFER);
                        dyParam.Add("@GRID_MODE_EDIT", SqlDbType.VarChar, ParameterDirection.Input, item.GRID_MODE_EDIT != null ? "G014" + item.GRID_MODE_EDIT : "");
                        // Quan add 2021-03-30
                        dyParam.Add("@PAGING_TYP", SqlDbType.VarChar, ParameterDirection.Input, item.PAGING_TYP);
                        result = SqlMapper.ExecuteScalar<int>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        transaction.Commit();
                        conn.Close();

                        return new Result
                        {

                            Success = true,
                            Message = "Save changed data Success!"
                        };
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            if (result == 1)
            {
                return new Result
                {

                    Success = true,
                    Message = "Save changed data Success!"
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not Success!",
                };
            }
        }
        public Result DeleteDataElementControls(SYPageLayElements item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYPageLayElements",
                        new string[] { "@DIV", "@ID", "@PAG_ID", "@PEL_ID" },
                        new object[] { CommonAction.DELETE, item.ID, item.PAG_ID, item.PEL_ID });
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

        #region GridDataPageLayElementDatasourceForCBRC
        public List<SYPageLayElements> GridDataPageLayElementDatasourceForCBRC(string PAG_ID, string PEL_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYPageLayElements>("SP_Web_SYPageLayElements",
                        new string[] { "@DIV", "@PAG_ID", "@PEL_ID" },
                        new object[] { "SelectOnlyCBRB", PAG_ID, PEL_ID });
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Result SaveDataPageLayElementDatasourceForCBRC(SYPageLayElements item, string action)
        {
            int result = -1;
            var query = "SP_Web_SYPageLayElements";
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

                        // check form or grid, form cannot set grid click, grid DBclick
                        //form
                        if (item.PEL_TYP != null && item.PEL_TYP.Equals("C001"))
                        {
                            if (item.PEL_CLICK != null || item.PEL_DBLCLICK != null)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = "Cannot save setting grid action click row for page type Form."
                                };
                            }
                        }

                        // Cannot existed Groupcode and groupcode custom Sp at the same time
                        if (item != null && item.GRP_CD != null && item.GRP_CD_CUSTOM != null)
                        {
                            return new Result
                            {
                                Success = false,
                                Message = "Page element has only existed Group code setting or Store procedure."
                            };
                        }

                        // Cannot existed Pel ID duplicate at the same time
                        if (item != null && item.PEL_ID != null)
                        {
                            var checkPelIDExistedQuery = "SELECT * FROM SYPageLayElements WHERE PAG_ID = " + item.PAG_ID + " AND PEL_ID = '" + item.PEL_ID + "'" + " AND ID <> " + item.ID
                                 + " AND PEL_PRN = '" + item.PEL_PRN + "'";
                            var checkPelIDExisted = SqlMapper.Query<SYPageLayElements>(conn, checkPelIDExistedQuery, transaction: transaction, commandType: CommandType.Text).ToList();

                            if (checkPelIDExisted != null && checkPelIDExisted.Count() >= 1)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = "Page element has only one Page element ID."
                                };
                            }
                        }

                        if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                        {
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.INSERT);
                        }
                        else
                        {
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.UPDATE);
                        }

                        dyParam.Add("@ID", SqlDbType.BigInt, ParameterDirection.Input, item.ID);
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PAG_ID);
                        dyParam.Add("@PEL_ID", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_ID);
                        dyParam.Add("@PEL_LBL", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_LBL);
                        dyParam.Add("@PEL_TYP", SqlDbType.VarChar, ParameterDirection.Input, "G002" + item.PEL_TYP); // add group code 
                        dyParam.Add("@PEL_PRN", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_PRN);// add child parent
                        dyParam.Add("@PEL_DAT_TYPE", SqlDbType.VarChar, ParameterDirection.Input, "G003" + item.PEL_DAT_TYPE); // add group code 
                        dyParam.Add("@PEL_LEN", SqlDbType.Int, ParameterDirection.Input, item.PEL_LEN);
                        dyParam.Add("@PEL_COL", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_ROW", SqlDbType.Int, ParameterDirection.Input, item.PEL_ROW);
                        dyParam.Add("@PEL_CSPN", SqlDbType.Int, ParameterDirection.Input, item.PEL_CSPN);
                        dyParam.Add("@PEL_RSPN", SqlDbType.Int, ParameterDirection.Input, item.PEL_RSPN);
                        dyParam.Add("@PEL_WDT", SqlDbType.Int, ParameterDirection.Input, item.PEL_WDT);
                        dyParam.Add("@PEL_HGT", SqlDbType.Int, ParameterDirection.Input, item.PEL_HGT);
                        dyParam.Add("@PEL_VIS", SqlDbType.Bit, ParameterDirection.Input, item.PEL_VIS);
                        dyParam.Add("@PEL_MAPYN", SqlDbType.Bit, ParameterDirection.Input, item.PEL_MAPYN);
                        dyParam.Add("@PEL_BINDYN", SqlDbType.Bit, ParameterDirection.Input, item.PEL_BINDYN);
                        dyParam.Add("@PEL_SEQ", SqlDbType.Int, ParameterDirection.Input, item.PEL_SEQ);
                        dyParam.Add("@PEL_DFVALUE", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_DFVALUE);
                        // Responsive with PEL_COL
                        dyParam.Add("@PEL_XS", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_SM", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_MD", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);
                        dyParam.Add("@PEL_LG", SqlDbType.Int, ParameterDirection.Input, item.PEL_COL);

                        dyParam.Add("@PEL_FIX", SqlDbType.Bit, ParameterDirection.Input, item.PEL_FIX);
                        dyParam.Add("@PEL_FORL", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_FORL);
                        dyParam.Add("@PEL_ALGN", SqlDbType.VarChar, ParameterDirection.Input, "G008" + item.PEL_ALGN);// add group code 
                        dyParam.Add("@PEL_CLICK", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_CLICK);
                        dyParam.Add("@PEL_DBLCLICK", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_DBLCLICK);
                        dyParam.Add("@PEL_REF_PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PEL_REF_PAG_ID);

                        // 2020-04-29: Minh add 
                        dyParam.Add("@IS_KEY", SqlDbType.Bit, ParameterDirection.Input, item.IS_KEY);
                        dyParam.Add("@IS_EDIT", SqlDbType.Bit, ParameterDirection.Input, item.IS_EDIT);
                        dyParam.Add("@EDIT_TYPE", SqlDbType.VarChar, ParameterDirection.Input, item.EDIT_TYPE);
                        dyParam.Add("@EDIT_ACT", SqlDbType.Int, ParameterDirection.Input, item.EDIT_ACT);
                        dyParam.Add("@PEL_EXP_TEXT", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_EXP_TEXT);
                        dyParam.Add("@GRP_CD", SqlDbType.VarChar, ParameterDirection.Input, item.GRP_CD);
                        dyParam.Add("@GRP_CD_CUSTOM", SqlDbType.VarChar, ParameterDirection.Input, item.GRP_CD_CUSTOM);
                        dyParam.Add("@PEL_IS_REQUIRED", SqlDbType.Bit, ParameterDirection.Input, item.PEL_IS_REQUIRED);

                        result = SqlMapper.ExecuteScalar<int>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        transaction.Commit();
                        conn.Close();

                        return new Result
                        {

                            Success = true,
                            Message = "Save changed data Success!"
                        };
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            if (result == 1)
            {
                return new Result
                {

                    Success = true,
                    Message = "Save changed data Success!"
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not Success!",
                };
            }
        }
        public Result DeleteDataPageLayElementDatasourceForCBRC(SYPageLayElements item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYPageLayElements",
                        new string[] { "@DIV", "@ID", "@PAG_ID", "@PEL_ID" },
                        new object[] { CommonAction.DELETE, item.ID, item.PAG_ID, item.PEL_ID });
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



        #region SY Page Relationship
        public List<SYPageRelationship> GetDataGridPageRelationshipLayout(string PAG_ID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = conn.ExecuteQuery<SYPageRelationship>(SP_WEB_PAGE_RELATIONSHIP_LAYOUT,
                    new string[] { "@DIV", "@PAG_ID" },
                    new object[] { CommonAction.SELECT, PAG_ID });
                return result.ToList();
            }
        }

        public Result SavePageRelationshipLayout(SYPageRelationship item, string action)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string actionType = "";
                if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                {
                    actionType = CommonAction.INSERT;
                }
                else
                {
                    actionType = CommonAction.UPDATE;
                }
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_WEB_PAGE_RELATIONSHIP_LAYOUT,
                        new string[] { "@DIV", "@ID", "@PAG_ID", "@POP_ID" },
                        new object[] { actionType, item.ID, item.PAG_ID, item.POP_ID });
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

        public Result DeletePageRelationshipLayout(SYPageRelationship item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_WEB_PAGE_RELATIONSHIP_LAYOUT,
                        new string[] { "@DIV", "@ID" },
                        new object[] { CommonAction.DELETE, item.ID });
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

        #region Get data for display in combo box
        public List<SYComCode> SelectComCodeByGRP(string GrpCode)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYComCode>(SP_WEB_SY_COMMON_CODE,
                        new string[] { "@DIV", "@GRP_CD" },
                        new object[] { "SelectByGrp", GrpCode });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<SYComCode> SelectComCodeAndGrpByGRP(string GrpCode)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYComCode>(SP_WEB_SY_COMMON_CODE,
                        new string[] { "@DIV", "@GRP_CD" },
                        new object[] { "SelectByGrp1", GrpCode });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<SYPageLayout> SelectComboBoxType(string codeType)
        {
            var result = new List<SYPageLayout>();
            var query = SP_WEB_SY_PAGE_LAYOUT;
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
                        dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.SELECT);
                        dyParam.Add("@PAG_TYPE", SqlDbType.VarChar, ParameterDirection.Input, codeType);
                        result = SqlMapper.Query<SYPageLayout>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure).ToList();

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

            //try
            //{
            //    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //    {
            //        var result = conn.ExecuteQuery<SYPageLayout>(SP_WEB_SY_PAGE_LAYOUT,
            //            new string[] { "@DIV", "@PAG_TYPE" },
            //            new object[] { CommonAction.SELECT, codeType });
            //        return result.ToList();
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        public List<SYPageLayout> SelectPageLayoutTypePopup()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYPageLayout>(SP_WEB_SY_PAGE_LAYOUT,
                        new string[] { "@DIV" },
                        new object[] { "SelectPopupPage" });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Get combo box value by group code setting for grid and form
        /// </summary>
        /// <param name="GrpCode"></param>
        /// <returns></returns>
        public List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnection(string GRP_CD, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(GRP_CD) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicCombobox>(SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP,
                                       new string[] { "@GROUP_CD" },
                                       new object[] { GRP_CD });
                        return result.ToList();
                    }
                }
                return null;


            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang(string GRP_CD, string CONNECTION_NM, string Lang)
        {
            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(GRP_CD) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicCombobox>(SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP,
                                       new string[] { "@GROUP_CD", "@Lang" },
                                       new object[] { GRP_CD, Lang.Trim() });
                        return result.ToList();
                    }
                }
                return null;


            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnection_MultiLang_CustomeSoure(string GRP_CD_CUSTOM, string CONNECTION_NM, string Lang)
        {
            try
            {
                // base on connection to define load data on framework db or another db.

                if (!string.IsNullOrEmpty(GRP_CD_CUSTOM) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicCombobox>(GRP_CD_CUSTOM,
                                       new string[] { "@Lang" },
                                       new object[] { Lang.Trim() });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get combo box value by group code setting for grid
        /// in grid not display all
        /// </summary>
        /// <param name="GrpCode"></param>
        /// <returns></returns>
        public List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeAndConnectionForGrid(string GRP_CD, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(GRP_CD) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicCombobox>(SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP_IN_GRID,
                                       new string[] { "@GROUP_CD" },
                                       new object[] { GRP_CD });
                        return result.ToList();
                    }
                }
                return null;


            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DynamicCombobox> GetComboboxValueDynamicByCustomSPAndConnection(string GRP_CD_CUSTOM, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.

                if (!string.IsNullOrEmpty(GRP_CD_CUSTOM) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicCombobox>(GRP_CD_CUSTOM,
                                       new string[] { },
                                       new object[] { });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<DynamicComboboxCustom> GetComboboxValueDynamicCustom(string GRP_CD_CUSTOM, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.

                if (!string.IsNullOrEmpty(GRP_CD_CUSTOM) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicComboboxCustom>(GRP_CD_CUSTOM,
                                       new string[] { },
                                       new object[] { });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DynamicComboboxCustom> GetReferDataCustomAutocomplete(string SP_CUSTOM, string value, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.

                if (!string.IsNullOrEmpty(SP_CUSTOM) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicComboboxCustom>(SP_CUSTOM,
                                       new string[] { "@value" },
                                       new object[] { value });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get combo box value by SP custom for grid
        /// </summary>
        /// <param name="GRP_CD_CUSTOM"></param>
        /// <returns></returns>
        public List<DynamicCombobox> GetComboboxValueDynamicByGroupCodeCustomSP(string GRP_CD_CUSTOM)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<DynamicCombobox>(GRP_CD_CUSTOM,
                        new string[] { },
                        new object[] { });
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get radio check box value by SP 
        /// </summary>
        /// <param name="GRP_CD_CUSTOM"></param>
        /// <returns></returns>
        public List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByGroupCodeAndConnection(string GRP_CD, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(GRP_CD) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicRadioCheckbox>(SP_GET_RADIO_CHECKBOX_VALUE_DYNAMIC_BY_SP,
                                       new string[] { "@GROUP_CD" },
                                       new object[] { GRP_CD });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByGroupCodeAndConnection_MultiLang(string GRP_CD, string CONNECTION_NM, string Lang)
        {
            try
            {
                // base on connection to define load data on framework db or another db.
                if (!string.IsNullOrEmpty(GRP_CD) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicRadioCheckbox>(SP_GET_RADIO_CHECKBOX_VALUE_DYNAMIC_BY_SP,
                                       new string[] { "@GROUP_CD", "@Lang" },
                                       new object[] { GRP_CD, Lang.Trim() });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByCustomSPAndConnection_MultiLang(string GRP_CD_CUSTOM, string CONNECTION_NM, string Lang)
        {
            try
            {
                // base on connection to define load data on framework db or another db.

                if (!string.IsNullOrEmpty(GRP_CD_CUSTOM) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicRadioCheckbox>(GRP_CD_CUSTOM,
                                       new string[] { "@Lang" },
                                       new object[] { Lang.Trim() });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get radio check box value by SP 
        /// </summary>
        /// <param name="GRP_CD_CUSTOM"></param>
        /// <returns></returns>
        public List<DynamicRadioCheckbox> GetRadiocheckboxDynamicByCustomSPAndConnection(string GRP_CD_CUSTOM, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.

                if (!string.IsNullOrEmpty(GRP_CD_CUSTOM) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicRadioCheckbox>(GRP_CD_CUSTOM,
                                       new string[] { },
                                       new object[] { });
                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DynamicAutocomplete> GetDataAutocompleteDynamicByCustomSPAndConnection(string SP_CUSTOM, string CONNECTION_NM)
        {
            try
            {
                // base on connection to define load data on framework db or another db.

                if (!string.IsNullOrEmpty(SP_CUSTOM) && !string.IsNullOrEmpty(CONNECTION_NM))
                {
                    using (var conn = DataConnectionFactory.GetConnection(ConnectionUtils.GetConnectionStringByConnectionType(CONNECTION_NM)))
                    {
                        var result = conn.ExecuteQuery<DynamicAutocomplete>(SP_CUSTOM,
                                       new string[] { },
                                       new object[] { });

                        return result.ToList();
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get all combo box master in table LzDxp200T
        /// </summary>
        /// <returns></returns>
        public List<DynamicCombobox> GetComboboxValueMasterLzDxp200T()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var query = "SELECT grpCode as 'ID', CONCAT(grpCode,' - ', grpName1) as 'Name'  FROM LzDxp200T WHERE  useFg = 'Y'";
                    var result = conn.ExecuteQuery<DynamicCombobox>(query, CommandType.Text,
                        new string[] { },
                        new object[] { });
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all combo box master in table By DS and Connection
        /// </summary>
        /// <returns></returns>
        public List<DynamicCombobox> GetComboboxValueMasterBySPDatasource()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var query = "SELECT MAP_SPNM as 'ID', MAP_SPNM as 'Name'  FROM SYDataMap WHERE  MAP_SPTYPE = 'G006C009'";
                    var result = conn.ExecuteQuery<DynamicCombobox>(query, CommandType.Text,
                        new string[] { },
                        new object[] { });
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region SY Data mapping data source
        public List<SYDataMap> GetDataGridDataMappingSPLayout(string PAG_ID, string PEL_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYDataMap>("SP_Web_SYDataMappingSPLayout",
                        new string[] { "@DIV", "@MAP_PAG_ID", "@MAP_PEL_ID" },
                        new object[] { CommonAction.SELECT, PAG_ID, PEL_ID });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Result SaveDataMappingSPLayout(SYDataMap item, string action)
        {
            int result = -1;
            var query = "SP_Web_SYDataMappingSPLayout";
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
                        // One layout cannot have more SP type
                        string checkExistedDataQuery = "SELECT * FROM SYDataMap WHERE MAP_PEL_ID = '" + item.MAP_PEL_ID + "' AND MAP_PAG_ID = " + item.MAP_PAG_ID + " AND (MAP_SPTYPE = '" + "G006" + item.MAP_SPTYPE + "' OR MAP_SPNM = '" + item.MAP_SPNM + "')";
                        //var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                        var dataExisted = SqlMapper.Query<SYDataMap>(conn, checkExistedDataQuery, transaction: transaction, commandType: CommandType.Text);
                        if (dataExisted != null && dataExisted.Count() > 1)
                        {
                            conn.Close();
                            transaction.Rollback();
                            return new Result
                            {
                                Success = false,
                                Message = "Save changed data not Success! One Page Element cannot have more one stored procedure type or one stored procedure name.",
                            };
                        }
                        else
                        {
                            var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                            if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                            {
                                dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.INSERT);
                            }
                            else
                            {
                                dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.UPDATE);
                            }
                            dyParam.Add("@MAP_ID", SqlDbType.Int, ParameterDirection.Input, item.MAP_ID);
                            dyParam.Add("@MAP_PEL_ID", SqlDbType.VarChar, ParameterDirection.Input, item.MAP_PEL_ID);
                            dyParam.Add("@MAP_PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.MAP_PAG_ID);
                            dyParam.Add("@MAP_SPNM", SqlDbType.VarChar, ParameterDirection.Input, item.MAP_SPNM);
                            dyParam.Add("@MAP_SPTYPE", SqlDbType.VarChar, ParameterDirection.Input, "G006" + item.MAP_SPTYPE); // Add Group code G006
                            dyParam.Add("@MAP_CNNAME", SqlDbType.VarChar, ParameterDirection.Input, "G013" + item.MAP_CNNAME); // Add Group code G013- connection type

                            result = SqlMapper.ExecuteScalar<int>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                            transaction.Commit();
                            conn.Close();

                            return new Result
                            {

                                Success = true,
                                Message = "Save changed data Success!"
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        transaction.Rollback();
                        var resultEx = new Result()
                        {
                            Success = false,
                            Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        };
                        return resultEx;
                    }
                }
            }
            if (result == 1)
            {
                return new Result
                {

                    Success = true,
                    Message = "Save changed data Success!"
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not Success!",
                };
            }
        }

        public Result DeleteDataMappingSPLayout(SYDataMap item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYDataMappingSPLayout",
                        new string[] { "@DIV", "@MAP_ID" },
                        new object[] { CommonAction.DELETE, item.MAP_ID });
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

        #region SY Data mapping data source detail
        public List<SYDataMapDetails> GetDataGridDataMappingDetails(int MAP_ID, string PAG_ID, string PEL_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYDataMapDetails>("SP_Web_SYDataMappingDetailsSPLayout",
                        new string[] { "@DIV", "@MAP_ID", "@PAG_ID" },
                        new object[] { CommonAction.SELECT, MAP_ID, PAG_ID });
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetMapFieldOnGridType(int PAG_ID)
        {
            //try
            //{
            //    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //    {
            //        var query = "SELECT PEL_ID FROM SYPageLayElements WHERE PAG_ID = '" + PAG_ID + "' AND  PEL_PRN  <> ''";
            //        var result = conn.ExecuteQuery<string>(query, CommandType.Text,
            //            new string[] {  },
            //            new object[] {  });
            //        return result.ToList();
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            var result = new List<string>();
            var query = "SELECT PEL_ID FROM SYPageLayElements WHERE PAG_ID = '" + PAG_ID + "' AND  PEL_PRN  <> ''";
            //var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);

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
                        result = SqlMapper.Query<string>(conn, query, transaction: transaction, commandType: CommandType.Text).ToList();

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
        public Result SaveDataMappingDetailLayout(SYDataMapDetails item, string action)
        {
            int result = -1;
            var query = "SP_Web_SYDataMappingDetailsSPLayout";
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
                        // One mapping detail cannot duplicate MapFLD, PEL Field
                        string checkExistedDataQuery = "SELECT * FROM SYDataMapDetails WHERE MAP_FROM = '" + item.MAP_FROM + "' AND MAP_ID = " + item.MAP_ID + " AND MDTL_ID <> " + item.MDTL_ID+ " AND PAG_ID="+item.PAG_ID;
                        var dataExisted = SqlMapper.Query<SYDataMap>(conn, checkExistedDataQuery, transaction: transaction, commandType: CommandType.Text);
                        if (dataExisted != null && dataExisted.Count() >= 1)
                        {
                            //conn.Close();
                            //transaction.Rollback();
                            return new Result
                            {
                                Success = false,
                                Message = "Save changed data not Success! One Mapping Detail cannot have more one mapping field and page element field.",
                            };
                        }
                        else
                        {
                            var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                            if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                            {
                                dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.INSERT);
                            }
                            else
                            {
                                dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.UPDATE);
                            }
                            dyParam.Add("@MAP_ID", SqlDbType.Int, ParameterDirection.Input, item.MAP_ID);
                            dyParam.Add("@MDTL_ID", SqlDbType.Int, ParameterDirection.Input, item.MDTL_ID);
                            dyParam.Add("@MAP_FROM", SqlDbType.VarChar, ParameterDirection.Input, item.MAP_FROM);
                            dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PAG_ID);
                            dyParam.Add("@MAP_TO", SqlDbType.VarChar, ParameterDirection.Input, item.MAP_TO);
                            dyParam.Add("@FLD_IO", SqlDbType.Int, ParameterDirection.Input, item.FLD_IO_CONVERT != null ? Int32.Parse(item.FLD_IO_CONVERT) : (int?)null);
                            dyParam.Add("@MDTL_DTYPE", SqlDbType.VarChar, ParameterDirection.Input, "G003" + item.MDTL_DTYPE);// Add Group code G003
                            dyParam.Add("@FLD_TYPE", SqlDbType.VarChar, ParameterDirection.Input, "G007" + item.FLD_TYPE);// Add Group code G007
                            dyParam.Add("@FRM_PEL_ID", SqlDbType.VarChar, ParameterDirection.Input, item.FRM_PEL_ID);

                            result = SqlMapper.ExecuteScalar<int>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                            transaction.Commit();
                            conn.Close();

                            return new Result
                            {

                                Success = true,
                                Message = "Save changed data Success!"
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        var resultEx = new Result()
                        {
                            Success = false,
                            Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        };
                        conn.Close();
                        transaction.Rollback();
                        return resultEx;
                        //return new Result
                        //{
                        //    Success = false,
                        //    Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        //};
                    }
                }
            }
            if (result == 1)
            {
                return new Result
                {

                    Success = true,
                    Message = "Save changed data Success!"
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not Success!",
                };
            }
        }
        public Result DeleteDataMappingDetailLayout(SYDataMapDetails item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYDataMappingDetailsSPLayout",
                        new string[] { "@DIV", "@MAP_ID", "@MDTL_ID" },
                        new object[] { CommonAction.DELETE, item.MAP_ID, item.MDTL_ID });
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

        #region SY Setting Page Actions
        public List<SYPageActions> GridDataPageActionsLayout(string PAG_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYPageActions>("SP_Web_SYDataPageActionsSPLayout",
                        new string[] { "@DIV", "@PAG_ID" },
                        new object[] { CommonAction.SELECT, PAG_ID });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Result SavedDataPageActionsLayout(SYPageActions item, string action)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                string actionType = "";
                if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                {
                    actionType = CommonAction.INSERT;
                }
                else
                {
                    actionType = CommonAction.UPDATE;
                }
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYDataPageActionsSPLayout",
                        new string[] { "@DIV", "@PAG_ID", "@ACT_ID", "@ACT_NM", "@ACT_FN", "@ACT_LC", "@ACT_PEL_TYP", "@IS_INIT", "@ACT_TYPE" },
                        new object[] { actionType, item.PAG_ID, item.ACT_ID, item.ACT_NM, item.ACT_FN, item.ACT_LC, item.ACT_PEL_TYP, item.IS_INIT, "G010" + item.ACT_TYPE });
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

        public Result DeletedDataPageActionsLayout(SYPageActions item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYDataPageActionsSPLayout",
                        new string[] { "@DIV", "@ACT_ID", "@PAG_ID" },
                        new object[] { CommonAction.DELETE, item.ACT_ID, item.PAG_ID });
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
        public List<SYPageActions> SelectActionMappingName(int PAG_ID)
        {
            //try
            //{
            //    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            //    {
            //        var query = "SELECT * FROM SYPageActions WHERE PAG_ID = " + PAG_ID;
            //        var result = conn.ExecuteQuery<SYPageActions>(query, CommandType.Text,
            //            new string[] { "@DIV", "@PAG_ID" },
            //            new object[] { CommonAction.SELECT, PAG_ID });
            //        return result.ToList();
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            var result = new List<SYPageActions>();
            var query = "SELECT * FROM SYPageActions WHERE PAG_ID = " + PAG_ID;
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
                        result = SqlMapper.Query<SYPageActions>(conn, query, transaction: transaction, commandType: CommandType.Text).ToList();
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
        #endregion

        #region SY Setting Page Actions Details
        public List<SYPageActionDetails> GetDataGridPageActionDetails(int PAG_ID, int ACT_ID)
        {
            try
            {
                {
                    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                    {
                        var result = conn.ExecuteQuery<SYPageActionDetails>("SP_Web_SYDataPageActionsDetailsSPLayout",
                            new string[] { "@DIV", "@PAG_ID", "@ACT_ID" },
                            new object[] { CommonAction.SELECT, PAG_ID, ACT_ID });
                        return result.ToList();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<SYPageLayElements> GetDataGridClearElements(int PAG_ID, int ACT_ID)
        {
            try
            {
                {
                    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                    {
                        var result = conn.ExecuteQuery<SYPageLayElements>("SP_Web_SYClearElements",
                            new string[] { "@DIV", "@PAG_ID", "@ACT_ID" },
                            new object[] { "SELECT_PEL", PAG_ID, ACT_ID });
                        return result.ToList();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<SYClearElements> GetDataSelectedGridClearElements(int PAG_ID, int ACT_ID)
        {
            try
            {
                {
                    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                    {
                        var result = conn.ExecuteQuery<SYClearElements>("SP_Web_SYClearElements",
                            new string[] { "@DIV", "@PAG_ID", "@ACT_ID" },
                            new object[] { "SELECT", PAG_ID, ACT_ID });
                        return result.ToList();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<SYClearElements> GetDataClearElementsWithTypeForm(int PAG_ID, int ACT_ID, string PEL_ID)
        {
            try
            {
                {
                    using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                    {
                        var result = conn.ExecuteQuery<SYClearElements>("SP_Web_SYClearElements",
                            new string[] { "@DIV", "@PAG_ID", "@ACT_ID", "@PEL_ID" },
                            new object[] { "SELECT_PEL_WITH_TYP", PAG_ID, ACT_ID, PEL_ID });
                        return result.ToList();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public int SaveDataGridClearElements(List<SYPageLayElements> data, int PAG_ID, int ACT_ID)
        {
            var result = 0;
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
                        var dyParamDel = dbConnection.CreateParameters(DbmsTypes.Mssql);
                        dyParamDel.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, "DELETE_PAG_ACT");
                        dyParamDel.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PAG_ID);
                        dyParamDel.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, ACT_ID);
                        result = SqlMapper.Execute(conn, query, param: dyParamDel, transaction: transaction, commandType: CommandType.StoredProcedure);
                        foreach (var item in data)
                        {
                            var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, "INSERT_PAG_ACT");
                            dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PAG_ID);
                            dyParam.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, ACT_ID);
                            dyParam.Add("@PEL_ID", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_ID);
                            dyParam.Add("@PEL_TYP", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_TYP);
                            dyParam.Add("@PAG_ID_SRC", SqlDbType.VarChar, ParameterDirection.Input, item.PAG_ID);
                            result = SqlMapper.Execute(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
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
            return result;
        }
        public Result SavedDataPageActionDetailsLayout(SYPageActionDetails item, string action)
        {
            int result = -1;
            var query = "SP_Web_SYDataPageActionsDetailsSPLayout";
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
                        if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                        {
                            // One Page action detail cannot duplicate PAGE_ID, ACT_ID, MAP_ID
                            string checkExistedDataQuery = "SELECT * FROM SYPageActionDetails WHERE PAG_ID = " + item.PAG_ID + " AND ACT_ID = " + item.ACT_ID + " AND SOURCE_ID = " + item.SOURCE_ID;
                            var dataExisted = SqlMapper.Query<SYDataMap>(conn, checkExistedDataQuery, transaction: transaction, commandType: CommandType.Text);
                            if (dataExisted != null && dataExisted.Count() >= 1)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = "Save changed data not Success! One Page Action Detail cannot have more one action and mapping name.",
                                };
                            }
                            else
                            {
                                dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.INSERT);

                            }
                        }
                        else
                        {
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.UPDATE);
                        }
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PAG_ID);
                        dyParam.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, item.ACT_ID);
                        dyParam.Add("@ACT_TYPE", SqlDbType.VarChar, ParameterDirection.Input, "G010" + item.ACT_TYPE);
                        dyParam.Add("@SOURCE_ID", SqlDbType.Int, ParameterDirection.Input, item.SOURCE_ID);
                        dyParam.Add("@PAG_REDIRECT", SqlDbType.Int, ParameterDirection.Input, item.PAG_REDIRECT);
                        // dyParam.Add("@MAP_ID", SqlDbType.Int, ParameterDirection.Input, item.MAP_ID_CONVERT != null ? Int32.Parse(item.MAP_ID_CONVERT) : (int?)null);
                        dyParam.Add("@EXEC_SEQ", SqlDbType.Int, ParameterDirection.Input, item.EXEC_SEQ);
                        dyParam.Add("@RUN_TRXN", SqlDbType.Bit, ParameterDirection.Input, item.RUN_TRXN);
                        dyParam.Add("@ACT_FN", SqlDbType.VarChar, ParameterDirection.Input, item.ACT_FN);

                        result = SqlMapper.ExecuteScalar<int>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        transaction.Commit();
                        conn.Close();

                        return new Result
                        {

                            Success = true,
                            Message = "Save changed data Success!"
                        };
                    }
                    catch (Exception ex)
                    {
                        var resultEx = new Result()
                        {
                            Success = false,
                            Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        };
                        conn.Close();
                        transaction.Rollback();
                        return resultEx;
                        //return new Result
                        //{
                        //    Success = false,
                        //    Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        //};
                    }
                }
            }
            if (result == 1)
            {
                return new Result
                {

                    Success = true,
                    Message = "Save changed data Success!"
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not Success!",
                };
            }
        }
        public Result DeletedDataPageActionDetailsLayout(SYPageActionDetails item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYDataPageActionsDetailsSPLayout",
                        new string[] { "@DIV", "@PAG_ID", "@ACT_ID", "@SOURCE_ID" },
                        new object[] { CommonAction.DELETE, item.PAG_ID, item.ACT_ID, item.SOURCE_ID });
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

        public List<SYDataMap> GetMapPelIDInSYDataMap(int PAG_ID)
        {
            var result = new List<SYDataMap>();
            var query = "SP_Web_SYGetMapPelIDInSYDataMap";

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
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, PAG_ID);
                        result = SqlMapper.Query<SYDataMap>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure).ToList();
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
        public List<SYPageLayout> GetPageInSYRelationship(int PAG_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYPageLayout>("SP_Web_GetPageInRelationship",
                        new string[] { "@PAG_ID" },
                        new object[] { PAG_ID });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region SY Tool bar actions
        public List<SYToolbarActions> GetDataGridPageToolbarActions(int PAG_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYToolbarActions>("SP_Web_SYDataPageToolbarActionsSPLayout",
                        new string[] { "@DIV", "@PAG_ID" },
                        new object[] { CommonAction.SELECT, PAG_ID });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public Result SavedDataPageToolbarActionsLayout(SYToolbarActions item, string action)
        {
            int result = -1;
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

                        if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                        {
                            // One Page action detail cannot duplicate PAGE_ID, ACT_ID, MAP_ID
                            string checkExistedDataQuery = "SELECT * FROM SYToolbarActions WHERE PAG_ID = " + item.PAG_ID + " AND (ACT_ID = " + item.ACT_ID + " OR ACT_TYP = '" + "G004" + item.ACT_TYP + "') AND ID <> " + item.ID;
                            var dataExisted = SqlMapper.Query<SYDataMap>(conn, checkExistedDataQuery, transaction: transaction, commandType: CommandType.Text);
                            if (dataExisted != null && dataExisted.Count() >= 1)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = "Save changed data not Success! One Page cannot have more same action name.",
                                };
                            }
                            else
                            {
                                dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.INSERT);

                            }
                        }
                        else
                        {
                            string spUpdate = "SP_Web_SYValidateCRUDToolbarActions";
                            dyParam.Add("@ID", SqlDbType.Int, ParameterDirection.Input, item.ID);
                            dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PAG_ID);
                            dyParam.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, item.ACT_ID);
                            dyParam.Add("@ACT_TYP", SqlDbType.VarChar, ParameterDirection.Input, "G004" + item.ACT_TYP);
                            var rs = SqlMapper.ExecuteScalar<int>(conn, spUpdate, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);

                            if (rs != 0)
                            {
                                dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.UPDATE);
                            }
                            else
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = "Save changed data not Success! One Page cannot have more same action name.",
                                };
                            }
                        }
                        dyParam.Add("@ID", SqlDbType.Int, ParameterDirection.Input, item.ID);
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PAG_ID);
                        dyParam.Add("@ACT_ID", SqlDbType.Int, ParameterDirection.Input, item.ACT_ID);
                        dyParam.Add("@ACT_TYP", SqlDbType.VarChar, ParameterDirection.Input, "G004" + item.ACT_TYP);

                        result = SqlMapper.ExecuteScalar<int>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        transaction.Commit();
                        conn.Close();

                        return new Result
                        {

                            Success = true,
                            Message = "Save changed data Success!"
                        };

                    }
                    catch (Exception ex)
                    {
                        var resultEx = new Result()
                        {
                            Success = false,
                            Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        };
                        conn.Close();
                        transaction.Rollback();
                        return resultEx;
                        //return new Result
                        //{
                        //    Success = false,
                        //    Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        //};
                    }
                }
            }
            if (result == 1)
            {
                return new Result
                {

                    Success = true,
                    Message = "Save changed data Success!"
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not Success!",
                };
            }
        }
        public Result DeletedDataPageToolbarActionsLayout(SYToolbarActions item)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYDataPageToolbarActionsSPLayout",
                        new string[] { "@DIV", "@ID", "@PAG_ID" },
                        new object[] { CommonAction.DELETE, item.ID, item.PAG_ID });
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

        public List<SYPageActions> GetMapPageAction(int PAG_ID)
        {
            var result = new List<SYPageActions>();
            var query = "SELECT ACT_ID, ACT_NM FROM SYPageActions WHERE PAG_ID = " + PAG_ID;

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
                        result = SqlMapper.Query<SYPageActions>(conn, query, transaction: transaction, commandType: CommandType.Text).ToList();

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
        #endregion

        #region SY Setting reference
        public List<SYPageLayElementReference> GridDataPageLayElementReference(int PAG_ID, string PEL_ID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYPageLayElementReference>("SP_Web_SYPageLayElementReference",
                        new string[] { "@DIV", "@PAG_ID", "@PEL_ID" },
                        new object[] { CommonAction.SELECT, PAG_ID, PEL_ID });
                    return result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Result SavedDataDataPageLayElementReference(SYPageLayElementReference item, string action)
        {
            int result = -1;
            var query = "SP_Web_SYPageLayElementReference";
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

                        // One mapping detail cannot duplicate MapFLD, PEL Field
                        string checkExistedDataQuery = "SELECT * FROM SYPageLayElementReference WHERE PAG_ID = " + item.PAG_ID + " AND PEL_ID = '"
                            + item.PEL_ID + "' AND REF_TYPE = 'G012" + item.REF_TYPE + "' AND (SOURCE_COL_NM = '" + item.SOURCE_COL_NM + "' OR TARGET_COL_NM = '" + item.TARGET_COL_NM +
                            "') AND ID <> " + item.ID;
                        var dataExisted = SqlMapper.Query<SYPageLayElementReference>(conn, checkExistedDataQuery, transaction: transaction, commandType: CommandType.Text);
                        if (dataExisted != null && dataExisted.Count() >= 1)
                        {
                            //conn.Close();
                            //transaction.Rollback();
                            return new Result
                            {
                                Success = false,
                                Message = "Save changed data not Success! The element which you choice have existed reference.",
                            };
                        }
                        else
                        {

                        }


                        if (!string.IsNullOrEmpty(action) && action.Equals(CommonAction.ADD))
                        {
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.INSERT);
                        }
                        else
                        {
                            dyParam.Add("@DIV", SqlDbType.VarChar, ParameterDirection.Input, CommonAction.UPDATE);
                        }
                        dyParam.Add("@ID", SqlDbType.Int, ParameterDirection.Input, item.ID);
                        dyParam.Add("@PAG_ID", SqlDbType.Int, ParameterDirection.Input, item.PAG_ID);
                        dyParam.Add("@PEL_ID", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_ID);
                        dyParam.Add("@PEL_ID_TO", SqlDbType.VarChar, ParameterDirection.Input, item.PEL_ID_TO);
                        dyParam.Add("@REF_TYPE", SqlDbType.VarChar, ParameterDirection.Input, item.REF_TYPE != null ? "G012" + item.REF_TYPE : null);
                        dyParam.Add("@SOURCE_COL_NM", SqlDbType.VarChar, ParameterDirection.Input, item.SOURCE_COL_NM);
                        dyParam.Add("@TARGET_COL_NM", SqlDbType.VarChar, ParameterDirection.Input, item.TARGET_COL_NM);
                        dyParam.Add("@IO_FL", SqlDbType.VarChar, ParameterDirection.Input, item.IO_FL);
                        dyParam.Add("@DATA_MAP_ADDON", SqlDbType.VarChar, ParameterDirection.Input, item.DATA_MAP_ADDON);

                        result = SqlMapper.ExecuteScalar<int>(conn, query, param: dyParam, transaction: transaction, commandType: CommandType.StoredProcedure);
                        transaction.Commit();
                        conn.Close();

                        return new Result
                        {

                            Success = true,
                            Message = "Save changed data Success!"
                        };

                    }
                    catch (Exception ex)
                    {
                        var resultEx = new Result()
                        {
                            Success = false,
                            Message = "Save changed data not Success! + Exception: " + ex.ToString(),
                        };
                        conn.Close();
                        transaction.Rollback();
                        return resultEx;
                    }
                }
            }
            if (result == 1)
            {
                return new Result
                {

                    Success = true,
                    Message = "Save changed data Success!"
                };
            }
            else
            {
                return new Result
                {
                    Success = false,
                    Message = "Save changed data not Success!",
                };
            }
        }

        public Result DeletedDataPageLayElementReference(SYPageLayElementReference item)
        {

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery("SP_Web_SYPageLayElementReference",
                        new string[] { "@DIV", "@ID", "@PAG_ID", "@PEL_ID" },
                        new object[] { CommonAction.DELETE, item.ID });
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

        public List<SYPageLayElementReference> GetListColumnLayout(int PAG_ID, string PEL_ID, string type)
        {
            var result = new List<SYPageLayElementReference>();
            var query = "";
            // Remove Just select element type number //AND PEL_TYP = 'G002C007'
            if (type.Equals("G002C001")) // FORM
            {
                query = "SELECT DISTINCT PEL_ID FROM SYPageLayElements WHERE" +
                    " PEL_PRN IN (SELECT PEL_ID FROM SYPageLayElements WHERE PEL_PRN = '' AND PAG_ID =" + PAG_ID + " AND PEL_TYP = 'G002C001') AND PAG_ID = " + PAG_ID;
            }
            else if (type.Equals("G002C002")) // GRID
            {
                query = "SELECT DISTINCT PEL_ID FROM SYPageLayElements WHERE" +
                    " PEL_PRN IN (SELECT PEL_ID FROM SYPageLayElements WHERE PEL_PRN = '' AND PAG_ID =" + PAG_ID + " AND PEL_TYP = 'G002C002') AND PAG_ID = " + PAG_ID;
            }

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
                        result = SqlMapper.Query<SYPageLayElementReference>(conn, query, transaction: transaction, commandType: CommandType.Text).ToList();

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

        public List<string> GetPageElementRelationship(int PAG_ID, string PEL_ID)
        {
            var result = new List<string>();
            var query = "SELECT PEL_ID FROM SYPageLayElements WHERE PAG_ID = '" + PAG_ID + "' AND  PEL_PRN  = '' AND PEL_ID != '" + PEL_ID + "'";
            //var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);

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
                        result = SqlMapper.Query<string>(conn, query, transaction: transaction, commandType: CommandType.Text).ToList();

                        transaction.Commit();
                        conn.Close();
                    }
                    catch 
                    {
                        conn.Close();
                        transaction.Rollback();
                    }
                }
            }
            return result;
        }
        #endregion

        #region Test MySQL

        public List<VerifyCodeToken> GetAllVerifyCodeToken()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    var files = conn.ExecuteQuery<VerifyCodeToken>("GET_ALL_VERIFY_TOKEN",
                        new string[] { "@param1" }, // Input param
                        new object[] { "2" });
                    //var files = conn.ExecuteQuery<VerifyCodeToken>("SELECT * FROM VERIFY_CODE_TOKEN", CommandType.Text,
                    //    new string[] {  },
                    //    new object[] {  });
                    return files.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}

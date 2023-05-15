using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.DAL.Engines;
using LinqToDB.Data;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESBOMMgtService : IMESBOMMgtService
    {
        private string SP_MES_BOM = "SP_MES_BOM";


        #region "Get Data"

        // Get list SaleProject
        public List<MES_BOM> GetItemFinish()
        {
            var result = new List<MES_BOM>();

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetItemFinish";
                var data = conn.ExecuteQuery<MES_BOM>(SP_MES_BOM, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }
        // Get list SaleProject All Data

        public List<MES_Item> GetListItem(string ItemClassCode, string categoryCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemClassCode";
                arrParams[2] = "@CategoryCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetListItem";
                arrParamsValue[1] = ItemClassCode;
                arrParamsValue[2] = categoryCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_BOM, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        public List<MES_ItemClass> LoadItemClass()
        {
            var result = new List<MES_ItemClass>();

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetItemClass";
                var data = conn.ExecuteQuery<MES_ItemClass>(SP_MES_BOM, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        public List<ItemRequest> GetBOMItemBySalePJCode(string PJCode)
        {
            var result = new List<ItemRequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParam = new string[2];
                arrParam[0] = "@Method";
                arrParam[1] = "@ProjectCode";
                object[] arrParamValues = new object[2];
                arrParamValues[0] = "GetBOMItemBySalePJCode";
                arrParamValues[1] = PJCode;

                result = conn.ExecuteQuery<ItemRequest>(SP_MES_BOM, arrParam, arrParamValues).ToList();

                int i = 1;
                result.ForEach(x => x.No = i++);
            }

            return result;
        }


        #endregion

        #region SEARCH

        public List<MES_BOM> GetDataSearch(string ItemCode, string ParentItemLevel)
        {
            var result = new List<MES_BOM>();

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemCode";
                arrParams[2] = "@ParentItemLevel";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "SEARCH";
                arrParamsValue[1] = ItemCode;
                arrParamsValue[2] = ParentItemLevel;
                var data = conn.ExecuteQuery<MES_BOM>(SP_MES_BOM, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        public List<MES_BOM> SearchItemPopup(string categoryCode, string itemClassCode, string itemName, string itemCode)
        {
            var result = new List<MES_BOM>();

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[5];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemName";
                arrParams[2] = "@CategoryCode";
                arrParams[3] = "@ItemClassCode";
                arrParams[4] = "@ItemCode";
                object[] arrParamsValue = new object[5];
                arrParamsValue[0] = "SearchItemPopup";
                arrParamsValue[1] = itemName;
                arrParamsValue[2] = categoryCode;
                arrParamsValue[3] = itemClassCode;
                arrParamsValue[4] = itemCode;
                var data = conn.ExecuteQuery<MES_BOM>(SP_MES_BOM, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        public List<MES_BOM> SearchItemPopupGetOther(string categoryCode, string itemClassCode, string itemName, string itemCode)
        {
            var result = new List<MES_BOM>();

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[5];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemName";
                arrParams[2] = "@CategoryCode";
                arrParams[3] = "@ItemClassCode";
                arrParams[4] = "@ItemCode";
                object[] arrParamsValue = new object[5];
                arrParamsValue[0] = "SearchItemPopupGetOther";
                arrParamsValue[1] = itemName;
                arrParamsValue[2] = categoryCode;
                arrParamsValue[3] = itemClassCode;
                arrParamsValue[4] = itemCode;
                var data = conn.ExecuteQuery<MES_BOM>(SP_MES_BOM, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        #endregion

        #region CRUD

        public List<MES_BOM> InsertParentItem(MES_BOM mesBOM)
        {
            var result = new List<MES_BOM>();

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[6];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemCode";
                arrParams[2] = "@NameKor";
                arrParams[3] = "@ItemLevel";
                arrParams[4] = "@ParentItemLevel";
                arrParams[5] = "@ParentItemCode";
                object[] arrParamsValue = new object[6];
                arrParamsValue[0] = "InsertParentItem";
                arrParamsValue[1] = mesBOM.ItemCode;
                arrParamsValue[2] = mesBOM.NameKor;
                arrParamsValue[3] = mesBOM.ItemLevel;
                arrParamsValue[4] = mesBOM.ParentItemLevel;
                arrParamsValue[5] = mesBOM.ParentItemCode;
                var data = conn.ExecuteQuery<MES_BOM>(SP_MES_BOM, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        public Result InsertBOMItems(List<MES_BOM> InsertArr, List<MES_BOM> UpdateArr, List<MES_BOM> DeleteArr)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    var result = -1;
                    try
                    {
                        foreach (var item in DeleteArr)
                        {
                            string[] arrParams = new string[5];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@ItemCode";
                            arrParams[2] = "@ParentItemCode";
                            arrParams[3] = "@ItemLevel";
                            arrParams[4] = "@ParentItemLevel";
                            object[] arrParamsValue = new object[5];
                            arrParamsValue[0] = "DeleteItems";
                            arrParamsValue[1] = item.ItemCode;
                            arrParamsValue[2] = item.ParentItemCode;
                            arrParamsValue[3] = item.ItemLevel;
                            arrParamsValue[4] = item.ParentItemLevel;
                            result = conn.ExecuteNonQuery(SP_MES_BOM, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        }

                        foreach (var item in InsertArr)
                        {
                            string[] arrParams = new string[7];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@ItemCode";
                            arrParams[2] = "@ParentItemCode";
                            arrParams[3] = "@ItemLevel";
                            arrParams[4] = "@ParentItemLevel";
                            arrParams[5] = "@NameKor";
                            arrParams[6] = "@Qty";
                            object[] arrParamsValue = new object[7];
                            arrParamsValue[0] = "InsertItems";
                            arrParamsValue[1] = item.ItemCode;
                            arrParamsValue[2] = item.ParentItemCode;
                            arrParamsValue[3] = item.ItemLevel;
                            arrParamsValue[4] = item.ParentItemLevel;
                            arrParamsValue[5] = item.NameKor;
                            arrParamsValue[6] = item.Qty;
                            result = conn.ExecuteNonQuery(SP_MES_BOM, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        }

                        foreach (var item in UpdateArr)
                        {
                            string[] arrParams = new string[6];
                            arrParams[0] = "@Method";
                            arrParams[1] = "@ItemCode";
                            arrParams[2] = "@ParentItemCode";
                            arrParams[3] = "@ItemLevel";
                            arrParams[4] = "@ParentItemLevel";
                            arrParams[5] = "@Qty";
                            object[] arrParamsValue = new object[6];
                            arrParamsValue[0] = "UpdateItems";
                            arrParamsValue[1] = item.ItemCode;
                            arrParamsValue[2] = item.ParentItemCode;
                            arrParamsValue[3] = item.ItemLevel;
                            arrParamsValue[4] = item.ParentItemLevel;
                            arrParamsValue[5] = item.Qty;
                            result = conn.ExecuteNonQuery(SP_MES_BOM, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                        }

                        transaction.Commit();
                        if (result > 0)
                        {
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004
                            };
                        }
                        else
                        {
                            return new Result
                            {
                                Success = false,
                                Message = MessageCode.MD0005,
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
                        };
                    }
                }
            }
        }

        public Result InsertBOMTreeJSON(string InsertArrFromGetOther)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                    var result = -1;
                try
                {
                    result = InsertBOMTree(JsonConvert.DeserializeObject<List<MES_BOM>>(InsertArrFromGetOther)
                        , conn);

                    //result = conn.ExecuteScalar<int>(SP_MES_BOM,
                    //    new string[] { "@Method", "@ListBOMTreeData" },
                    //    new object[] { "InsertBOMTreeJSON", InsertArrFromGetOther });

                    if (result > 0)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = MessageCode.MD0004
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = MessageCode.MD0005,
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

        private int InsertBOMTree(List<MES_BOM> InsertArrFromGetOther, IDataConnection conn)
        {
            int result = -1;
            List<MES_BOM> returnList = new List<MES_BOM>();
            List<MES_BOM> lstOrder = InsertArrFromGetOther.OrderBy(m => m.ItemLevel).ToList();
            int MaxLevelTree = lstOrder.Max(m => m.ItemLevel);
            try
            {
                foreach (var item in lstOrder)
                {
                    if(item.ItemLevel == 1)
                    {
                        string[] arrParams = new string[8];
                        arrParams[0] = "@Method";
                        arrParams[1] = "@ItemCode";
                        arrParams[2] = "@ParentItemCode";
                        arrParams[3] = "@ItemLevel";
                        arrParams[4] = "@ParentItemLevel";
                        arrParams[5] = "@Qty";
                        arrParams[6] = "@NameKor";
                        arrParams[7] = "@RootId";
                        object[] arrParamsValue = new object[8];
                        arrParamsValue[0] = "InsertBOMTreeJSON";
                        arrParamsValue[1] = item.ItemCode;
                        arrParamsValue[2] = lstOrder.Where(m => m.ItemLevel == 0).Select(m => m.ItemCode).FirstOrDefault();
                        arrParamsValue[3] = 1;
                        arrParamsValue[4] = lstOrder.Where(m => m.ItemLevel == 0).Select(m => m.Id).FirstOrDefault();
                        arrParamsValue[5] = item.Qty;
                        arrParamsValue[6] = item.NameKor;
                        arrParamsValue[7] = lstOrder.Where(m => m.ItemLevel == 0).Select(m => m.Id).FirstOrDefault();
                        returnList = conn.ExecuteQuery<MES_BOM>(SP_MES_BOM, arrParams, arrParamsValue).ToList();

                    }
                    else if(item.ItemLevel > 1)
                    {
                        string[] arrParams = new string[8];
                        arrParams[0] = "@Method";
                        arrParams[1] = "@ItemCode";
                        arrParams[2] = "@ParentItemCode";
                        arrParams[3] = "@ItemLevel";
                        arrParams[4] = "@ParentItemLevel";
                        arrParams[5] = "@RootId";
                        arrParams[6] = "@Qty";
                        arrParams[7] = "@NameKor";
                        object[] arrParamsValue = new object[8];
                        arrParamsValue[0] = "InsertBOMTreeJSON";
                        arrParamsValue[1] = item.ItemCode;
                        arrParamsValue[2] = item.ParentItemCode;
                        arrParamsValue[3] = item.ItemLevel;
                        //arrParamsValue[4] = item.ParentItemLevel;
                        arrParamsValue[4] = returnList.Where(m => m.ItemCode == item.ParentItemCode).Select(m => m.Id).FirstOrDefault();
                        arrParamsValue[5] = lstOrder.Where(m => m.ItemLevel == 0).Select(m => m.Id).FirstOrDefault();
                        arrParamsValue[6] = item.Qty;
                        arrParamsValue[7] = item.NameKor;
                        returnList = conn.ExecuteQuery<MES_BOM>(SP_MES_BOM, arrParams, arrParamsValue).ToList();
                    }
                }

                return 1;
            }
            catch(Exception ex)
            {
                return result;
            }
        }
    #endregion

    }
}


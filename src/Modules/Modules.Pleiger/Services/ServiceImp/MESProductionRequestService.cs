﻿using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESProductionRequestService : IMESProductionRequestService
    {
        private string SP_Name = "SP_MES_PRODUCTIONREQUEST";
        private string SP_Name_Change = "SP_MES_PRODUCTIONREQUEST_CHANGE";
        #region "Get Data"

        // Get list Item Class
        public List<MES_SaleProject> GetListData()
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListData";
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_Name, arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        // Search list Production Request
        public List<MES_SaleProject> SearchListProductionRequest(string projectCode, string userProjectCode, string requestType, string customerName,
            string itemCode, string itemName, string requestStartDate, string requestEndDate, string projectStatus,string UserCode)
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[11];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@RequestType";
                arrParams[3] = "@CustomerName";
                arrParams[4] = "@ItemCode";
                arrParams[5] = "@ItemName";
                arrParams[6] = "@UserProjectCode";
                arrParams[7] = "@RequestStartDate";
                arrParams[8] = "@RequestEndDate";
                arrParams[9] = "@ProjectStatus";
                arrParams[10] = "@UserCode";
                object[] arrParamsValue = new object[11];
                arrParamsValue[0] = "SearchListData";
                arrParamsValue[1] = projectCode;
                arrParamsValue[2] = requestType;
                arrParamsValue[3] = customerName;
                arrParamsValue[4] = itemCode;
                arrParamsValue[5] = itemName;
                arrParamsValue[6] = userProjectCode;
                arrParamsValue[7] = requestStartDate;
                arrParamsValue[8] = requestEndDate;
                arrParamsValue[9] = projectStatus;
                arrParamsValue[10] = UserCode;
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_Name, arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        // Get data Sale Project detail
        public MES_SaleProject GetDataDetail(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetDetail";
                arrParamsValue[1] = projectCode;
                var result = conn.ExecuteQuery<MES_SaleProject>(SP_Name, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }

        // Get list Item Request of Sale Project
        public List<ItemRequest> GetListItemRequest(string projectCode)
        {
            var result = new List<ItemRequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemRequest";
                arrParamsValue[1] = projectCode;
                var data = conn.ExecuteQuery<ItemRequest>(SP_Name, arrParams, arrParamsValue);
                result = data.ToList();

            }
            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }
        // Get list Item Request of Sale Project
        public ItemRequest GetListItemRequestDetail(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemRequest";
                arrParamsValue[1] = projectCode;
                var result = conn.ExecuteQuery<ItemRequest>(SP_Name, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }
        // Get list Item PO
        public List<MES_ItemPO> GetListItemPO(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemPO";
                arrParamsValue[1] = projectCode;
                var result = conn.ExecuteQuery<MES_ItemPO>(SP_Name, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        // Check Stk Qty is enough for Request Production Plan or not
        public bool CheckEnoughItemQty(string projectCode)
        {
            var result = false;
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var data = conn.ExecuteScalar<int>(SP_Name, CommandType.StoredProcedure,
                    new string[] { "@Method", "@ProjectCode" },
                    new object[] { "CheckItemStkQty", projectCode });

                return result = data == 0 ? false : true;
            }
        }

        #endregion

        #region "Insert - Update - Delete "

        // Save data Production Request
        public Result SaveDataProductionRequest(MES_SaleProject model, string listItemRequest, string listItemPO)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_Name,
                        new string[] { "@Method", "@ProjectCode", "@RequestCode", "@UserRequest", "@RequestDate", "@RequestType", "@RequestMessage", "@ListItemRequest", "@ListItemPO" },
                        new object[] { "SaveDataProdReq", model.ProjectCode, model.RequestCode, model.UserIDRequest, model.RequestDate, model.RequestType, model.RequestMessage, listItemRequest, listItemPO });
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
                            Message = MessageCode.MD0005
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

        // Send Request Production
        public Result SendRequestProduction(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteScalar<int>(SP_Name, CommandType.StoredProcedure,
                    new string[] { "@Method", "@ProjectCode" },
                    new object[] { "SendProdReq", projectCode });
                    if (result == 1)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = "Send Request Production success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = result == -1 ? "Send Request Production failed!" : "Material isn't enough to send request production",
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

        // Quan add 2020/09/11
        // Save data roductionRequest
        public Result SaveDataProductionRequestChange(
            string projectCode,
            string requestCode,
            string requestType,
            string userIDRequest,
            string requestDate,
            string requestMessage,
            List<ItemRequest> listDataSave)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string listDataSaveJs = JsonConvert.SerializeObject(listDataSave);
                var result = -1;
                try
                {
                    result = conn.ExecuteScalar<int>(SP_Name_Change,
                       new string[] { "@Method", "@ProjectCode", "@RequestCode", "@RequestType", "@UserRequest", "@RequestDate", "@RequestMessage", "@ListItemRequest" },
                       new object[] { "SaveDataProdReq", projectCode, requestCode, requestType, userIDRequest, requestDate, requestMessage, listDataSaveJs });
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
                            Message = MessageCode.MD0005
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
        public Result UpdateDataProductionRequestChange(
            string projectCode,
            string requestCode,
            string requestType,
            string userIDRequest,
            string requestDate,
            string requestMessage)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_Name_Change,
                        new string[] { "@Method", "@ProjectCode", "@RequestCode", "@RequestType", "@UserRequest", "@RequestDate", "@RequestMessage" },
                        new object[] { "UpdateDataProductionRequest", projectCode, requestCode, requestType, userIDRequest, @requestDate, requestMessage });
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
                            Message = MessageCode.MD0005
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

        //  Request Production
        public Result RequestProduction(string projectCode, string UserID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_Name_Change, CommandType.StoredProcedure,
                    new string[] { "@Method", "@ProjectCode", "@UserRequest" },
                    new object[] { "RequestProduction", projectCode, UserID });
                    if (result == 1)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = "Request Production success!"
                        };
                    }
                    else
                    {
                        return new Result
                        {
                            Success = false,
                            Message = result == -1 ? "Request Production failed!" : "Material isn't enough to send request production",
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Result
                    {
                        Success = false,
                        Message = "data not success! + Exception: " + ex.ToString(),
                    };
                }
            }
        }
        public Result DeleteGridItemPartList(string ItemCode, string RequestCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_Name_Change, CommandType.StoredProcedure,
                    new string[] { "@Method", "@ItemCode", "@RequestCode" },
                    new object[] { "DeleteGridItemPartList", ItemCode, RequestCode });
                    if (result == 1)
                    {
                        return new Result
                        {
                            Success = true,
                            Message = MessageCode.MD0008,
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
                        Message = "Delete data not success! + Exception: " + ex.ToString(),
                    };
                }
            }
        }
        
        public Result RecallProductionRequest(string projectCode, string UserID)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    var result = -1;
                    try
                    {
                        result = conn.ExecuteNonQuery("SP_MES_SALEPROJECT", CommandType.StoredProcedure,
                                    new string[] { "@Method", "@ProjectCode", "@ProjectStatus", "@UserRequest" },
                                    new object[] { "UpdateStatusProject", projectCode, "PJST02", UserID },
                                    transaction);
                        transaction.Commit();
                        if (result == 1)
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
        #endregion

        #region Functions For New PO UI
        private const string SP_GETCATEGORY = "SP_GETCATEGORY";
        private const string SP_MESSITEM = "SP_MES_ITEM";


        /// <summary>
        /// Added By PVN
        /// Date: 2020-09-14
        /// Description: Get Category Code IMTP01 & IMTP02
        /// </summary>
        /// <returns></returns>
        public List<MES_ComCodeDtls> GetListCategories()
        {
            var result = new List<MES_ComCodeDtls>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListCategories";
                var data = conn.ExecuteQuery<MES_ComCodeDtls>(SP_GETCATEGORY, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        public List<MES_ItemClass> GetListItemClass(string categoryCode)
        {
            var result = new List<MES_ItemClass>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@categoryCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemClass";
                arrParamsValue[1] = categoryCode;
                var data = conn.ExecuteQuery<MES_ItemClass>(SP_GETCATEGORY, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        //public List<MES_Item> GetListItemOfProject(string ProjectCode, string Category, string ItemClassCode, string ItemCode)
        public List<ItemRequest> GetListItemOfProject(string ProjectCode, string ItemCode)
        {
            var result = new List<ItemRequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                //arrParams[2] = "@Category";
                //arrParams[3] = "@ItemClassCode";
                arrParams[2] = "@ItemCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetListItemOfProject";
                arrParamsValue[1] = ProjectCode;
                //arrParamsValue[1] = Category;
                //arrParamsValue[1] = ItemClassCode;
                arrParamsValue[2] = ItemCode;
                var data = conn.ExecuteQuery<ItemRequest>(SP_GETCATEGORY, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        //DUY ADD
        public List<ItemRequest> GetListItemOfItemClassCode(string Category, string ItemClassCode, string ItemCode, string ItemName)
        {
            var result = new List<ItemRequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[5];
                arrParams[0] = "@Method";
                //arrParams[1] = "@ProjectCode";
                arrParams[1] = "@Category";
                arrParams[2] = "@ItemClassCode";
                arrParams[3] = "@ItemCode";
                arrParams[4] = "@ItemName";
                object[] arrParamsValue = new object[5];
                arrParamsValue[0] = "GetListMaterialByItemClassCodeNew";
                arrParamsValue[1] = Category;
                arrParamsValue[2] = ItemClassCode;
                arrParamsValue[3] = ItemCode;
                arrParamsValue[4] = ItemName;
                var data = conn.ExecuteQuery<ItemRequest>(SP_MESSITEM, arrParams, arrParamsValue);

                result = data.ToList();
            }
            return result;
        }

        public List<MES_SaleProject> GetItemByProjectCodeInStatus()
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetItemByProjectCodeInStatus";
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_Name_Change, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }

        #endregion

    }
}
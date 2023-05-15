using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESSaleProjectService : IMESSaleProjectService
    {
        private string SP_MES_SALEPROJECT = "SP_MES_SALEPROJECT";
        private string SP_MES_SALEPROJECT_SAVE_DATA_GRID = "SP_MES_SALEPROJECT_SAVE_DATA_GRID";
        private string SP_MES_SALEPROJECTSEARCH = "SP_MES_SALEPROJECTSEARCH";
        private string SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP = "SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP";
        private string SP_MES_SALEPROJECT_DELETEBYPROJECTSTATUS = "SP_MES_SALEPROJECT_DELETEBYPROJECTSTATUS";
        private string SP_GET_LIST_SALEPROJECT_STATUS = "SP_GET_LIST_SALEPROJECT_STATUS";


        #region "Get Data"

        // Get list SaleProject
        public List<MES_SaleProject> GetListData()
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListData";
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }
        // Get list SaleProject All Data
        public List<MES_SaleProject> GetListAllData()
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListAllData";
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);

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
                var result = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }
        // Get list Item Request of Sale Project
        public List<ItemRequest> GetListItemRequest(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemRequest";
                arrParamsValue[1] = projectCode;
                var result = conn.ExecuteQuery<ItemRequest>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);

                return result.ToList();
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
                var result = conn.ExecuteQuery<MES_ItemPO>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        //Check Dublicate
        public MES_SaleProject CheckDuplicate(string DuplicateCode, string Type)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@DuplicateCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = Type;
                arrParamsValue[1] = DuplicateCode;

                var result = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }
        #endregion

        #region "Insert - Update - Delete"

        // Save data Sale Project
        public Result SaveSalesProject(MES_SaleProject model, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    var resultIns = -1;
                    try
                    {
                        //sai cho64 nay/////////////// client truyen tham so sai
                        resultIns = conn.ExecuteNonQuery(SP_MES_SALEPROJECT_SAVE_DATA_GRID, CommandType.StoredProcedure,
                            new string[] { "@DIV",
                                   "@ProjectCode","@UserProjectCode", "@ProjectName", "@InCharge","@PlanDeliveryDate", "@ProductType", "@ItemCode",
                                   "@PartnerCode", "@OrderNumber", "@DomesticForeign", "@OrderQuantity", "@MonetaryUnit", "@OrderPrice",
                                   "@ExchangeRate", "@VatType", "@VatRate","@UserModify", "@OrderTeamCode", "@ExchangeRateDate"
                            },
                            new object[] { "Insert",
                                    model.ProjectCode,model.UserProjectCode, model.ProjectName,model.InCharge,model.PlanDeliveryDate,model.ProductType, model.ItemCode,
                                    model.PartnerCode, model.OrderNumber, model.DomesticForeign, model.OrderQuantity,model.MonetaryUnit,model.OrderPrice,
                                    model.ExchangeRate,model.VatType,model.VatRate,userModify, model.OrderTeamCode, model.ExchangeRateDate
                                         }, transaction);
                        transaction.Commit();
                        if (resultIns > 0)
                        {
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                Success = false,
                                Message = MessageCode.MD0005
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

            return result;
        }

        // Delete data Sale Project
        public Result DeleteSalesProject(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_MES_SALEPROJECT_SAVE_DATA_GRID,
                        new string[] { "@DIV", "@ProjectCode" },
                        new object[] { "DELETE", projectCode });

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

        // Save data Production Request
        public Result SaveDataProductionRequest(MES_SaleProject model, string listItemRequest, string listItemPO)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_MES_SALEPROJECT,
                        new string[] { "@Method", "@ProjectCode", "@RequestCode", "@UserRequest", "@RequestDate", "@RequestType", "@RequestMessage", "@ListItemRequest", "@ListItemPO" },
                        new object[] { "SaveDataProdReq", model.ProjectCode, model.RequestCode, model.UserIDRequest, model.RequestDate, model.RequestType, model.RequestMessage, listItemRequest, listItemPO });
                    if (result > 0)
                    {
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

        public List<DynamicCombobox> GetProjectStatus()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    var result = conn.ExecuteQuery<DynamicCombobox>(SP_GET_LIST_SALEPROJECT_STATUS,
                                   new string[] { "@GROUP_CD" },
                                   new object[] { "PJST00" });
                    return result.ToList();
                }


            }
            catch (Exception)
            {
                throw;
            }
        }
        //
        public List<MES_SaleProject> GetUserProjectCode()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    var result = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECT,
                                   new string[] { "@Method" },
                                   new object[] { "GetListUserProjectCodeName" });
                    return result.ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public Result DeleteSalesProjects(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var result = 0;
                        //foreach (var id in projectCode)
                        //{
                        result = conn.ExecuteNonQuery(SP_MES_SALEPROJECT, CommandType.StoredProcedure,
                                           new string[] { "@Method", "@ProjectCode" },
                                           new object[] { "DELETE_PROJECTSTATUS", projectCode }, transaction);
                        transaction.Commit();
                        //}

                        if (result > 0)
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
                                Data = projectCode
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
                        }; ;
                    }

                }

            }

        }
        #endregion
        #region Search 
        public List<MES_SaleProject> SearchSaleProject(MES_SaleProject model)
        {

            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[10];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProductType";
                arrParams[2] = "@ProjectCode";
                arrParams[3] = "@ProjectName";
                arrParams[4] = "@ItemCode";
                arrParams[5] = "@ItemName";
                arrParams[6] = "@ProjectStatus";
                arrParams[7] = "@UserProjectCode";
                arrParams[8] = "@OrderTeamCode";
                arrParams[9] = "@UserCode";

                object[] arrParamsValue = new object[10];
                arrParamsValue[0] = "SearchSaleProject";
                arrParamsValue[1] = model.ProductType;
                arrParamsValue[2] = model.ProjectCode;
                arrParamsValue[3] = model.ProjectName;
                arrParamsValue[4] = model.ItemCode;
                arrParamsValue[5] = model.ItemName;
                arrParamsValue[6] = model.ProjectStatus != null && model.ProjectStatus.Equals("all") ? "" : model.ProjectStatus;
                arrParamsValue[7] = model.UserProjectCode;
                arrParamsValue[8] = model.OrderTeamCode;
                arrParamsValue[9] = model.UserCode;

                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECTSEARCH, arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        public List<MES_SaleProject> GetListProjectCodeByStatus()
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListProjectCodeByStatus";

                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);
                result = data.ToList();
                int i = 1;
                result.ForEach(x => x.No = i++);
            }
            return result;
        }

        #endregion
    }
}


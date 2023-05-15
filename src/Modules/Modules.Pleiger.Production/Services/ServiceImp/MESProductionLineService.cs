using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    public class MESProductionLineService : IMESProductionLineService
    {
        private const string SP_MES_PRODUCTION_LINE = "SP_MES_PRODUCTION_LINE";
        private const string SP_MES_WAREHOUSE_GET_COMBOBOX_MATERIAL_WH_CD_IN_GRID = "SP_MES_WAREHOUSE_GET_COMBOBOX_MATERIAL_WH_CD_IN_GRID";
        private const string SP_MES_WAREHOUSE_GET_COMBOBOX_FINISH_WH_CD_IN_GRID = "SP_MES_WAREHOUSE_GET_COMBOBOX_FINISH_WH_CD_IN_GRID";
        private const string SP_MES_GET_COMBOBOX_USER_IN_GRID = "SP_MES_GET_COMBOBOX_USER_IN_GRID";
        private const string SP_MES_PARTNER_GET_ALL_NAME = "SP_MES_PARTNER_GET_ALL_NAME";

        public List<MES_ProductLine> searchProductionLines(string InternalExternal, string MaterialWarehouseCode, string ProductionLineNameEng, string ProductionLineNameKor, string ProductionLineCode)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[6];
                    arrParams[0] = "@P_InternalExternal";
                    arrParams[1] = "@P_MaterialWarehouseCode";
                    arrParams[2] = "@P_ProductLineNameEng";
                    arrParams[3] = "@P_ProductLineName";
                    arrParams[4] = "@P_ProductLineCode";
                    arrParams[5] = "@Method";

                    object[] arrParamsValue = new object[6];
                    arrParamsValue[0] = InternalExternal;
                    arrParamsValue[1] = MaterialWarehouseCode;
                    arrParamsValue[2] = ProductionLineNameEng;
                    arrParamsValue[3] = ProductionLineNameKor;
                    arrParamsValue[4] = ProductionLineCode;
                    arrParamsValue[5] = "SearchProductLines";

                    var result = conn.ExecuteQuery<MES_ProductLine>(SP_MES_PRODUCTION_LINE, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<DynamicCombobox> MaterialWarehouseCodeCombobox()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[1];
                    arrParams[0] = "@Method";

                    object[] arrParamsValue = new object[1];
                    arrParamsValue[0] = "GetMaterialWarehouseCodeCombobox";

                    var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_WAREHOUSE_GET_COMBOBOX_MATERIAL_WH_CD_IN_GRID, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DynamicCombobox> FinishWarehouseCodeCombobox()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[1];
                    arrParams[0] = "@Method";

                    object[] arrParamsValue = new object[1];
                    arrParamsValue[0] = "GetFinishWarehouseCodeCombobox";

                    var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_WAREHOUSE_GET_COMBOBOX_FINISH_WH_CD_IN_GRID, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<DynamicCombobox> ProductManagerLineCombobox()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[1];
                    arrParams[0] = "@Method";

                    object[] arrParamsValue = new object[1];
                    arrParamsValue[0] = "GetProductManagerLineCombobox";

                    var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_GET_COMBOBOX_USER_IN_GRID, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DynamicCombobox> GetPartnerComboboxCombobox()
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[1];
                    arrParams[0] = "@Method";

                    object[] arrParamsValue = new object[1];
                    arrParamsValue[0] = "GetPartnerCombobox";

                    var result = conn.ExecuteQuery<DynamicCombobox>(SP_MES_PARTNER_GET_ALL_NAME, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public Result CRUDProductLine(List<MES_ProductLine> ArrInsLst, List<MES_ProductLine> ArrUpdLst, List<MES_ProductLine> ArrDelLst, string curUser)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var InsertResult = "N";
                        var UpdateResult = "N";
                        var DeleteResult = "N";
                        // DELETE
                        if (ArrDelLst.Count > 0)
                        {
                            foreach (var DeleteItem in ArrDelLst)
                            {
                                string[] arrParams = new string[2];
                                arrParams[0] = "@Method";
                                arrParams[1] = "@ProductLineCode";
                             
                                object[] arrParamsValue = new object[2];
                                arrParamsValue[0] = "DELETE";
                                arrParamsValue[1] = DeleteItem.ProductLineCode;
                               
                                DeleteResult = conn.ExecuteScalar<string>(SP_MES_PRODUCTION_LINE, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                            }

                        }
                        else
                        {
                            DeleteResult = "Y";
                        }
                        // UPDATE
                        if (ArrUpdLst.Count > 0)
                        {
                            foreach (var UpadteItem in ArrUpdLst)
                            {
                                string[] arrParams = new string[11];
                                arrParams[0] = "@Method";
                                arrParams[1] = "@ProductLineCode";
                                arrParams[2] = "@ProductLineName";
                                arrParams[3] = "@ProductLineNameEng";
                                arrParams[4] = "@MaterialWarehouseCode";
                                arrParams[5] = "@FinishWarehouseCode";
                                arrParams[6] = "@InternalExternal";
                                arrParams[7] = "@Manager";
                                arrParams[8] = "@Status";
                                arrParams[9] = "@PartnerCode";
                                arrParams[10] = "@Update_By";

                                object[] arrParamsValue = new object[11];
                                arrParamsValue[0] = "UPDATE";
                                arrParamsValue[1] = UpadteItem.ProductLineCode;
                                arrParamsValue[2] = UpadteItem.ProductLineName;
                                arrParamsValue[3] = UpadteItem.ProductLineNameEng;
                                arrParamsValue[4] = UpadteItem.MaterialWarehouseCode;
                                arrParamsValue[5] = UpadteItem.FinishWarehouseCode;
                                arrParamsValue[6] = UpadteItem.InternalExternal;
                                arrParamsValue[7] = UpadteItem.Manager;
                                arrParamsValue[8] = UpadteItem.Status;
                                arrParamsValue[9] = UpadteItem.PartnerCode;
                                arrParamsValue[10] = curUser;
                                UpdateResult = conn.ExecuteScalar<string>(SP_MES_PRODUCTION_LINE, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                            }

                        }
                        else
                        {
                            UpdateResult = "Y";
                        }
                        // INSERT
                        if (ArrInsLst.Count > 0)
                        {
                            foreach (var InsertItem in ArrInsLst)
                            {
                                string[] arrParams = new string[11];
                                arrParams[0] = "@Method";
                                arrParams[1] = "@ProductLineCode";
                                arrParams[2] = "@ProductLineName";
                                arrParams[3] = "@ProductLineNameEng";
                                arrParams[4] = "@MaterialWarehouseCode";
                                arrParams[5] = "@FinishWarehouseCode";
                                arrParams[6] = "@InternalExternal";
                                arrParams[7] = "@Manager";
                                arrParams[8] = "@Status";
                                arrParams[9] = "@PartnerCode";
                                arrParams[10] = "@Created_By";

                                object[] arrParamsValue = new object[11];
                                arrParamsValue[0] = "INSERT";
                                arrParamsValue[1] = InsertItem.ProductLineCode;
                                arrParamsValue[2] = InsertItem.ProductLineName;
                                arrParamsValue[3] = InsertItem.ProductLineNameEng;
                                arrParamsValue[4] = InsertItem.MaterialWarehouseCode;
                                arrParamsValue[5] = InsertItem.FinishWarehouseCode;
                                arrParamsValue[6] = InsertItem.InternalExternal;
                                arrParamsValue[7] = InsertItem.Manager;
                                arrParamsValue[8] = InsertItem.Status;
                                arrParamsValue[9] = InsertItem.PartnerCode;
                                arrParamsValue[10] = curUser;
                                InsertResult = conn.ExecuteScalar<string>(SP_MES_PRODUCTION_LINE, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                            }

                        }
                        else
                        {
                            InsertResult = "Y";
                        }


                        if ((InsertResult == "Y" || InsertResult == "INSERT") && (UpdateResult == "Y" || UpdateResult == "UPDATE") && (DeleteResult == "Y" || DeleteResult == "DELETE"))
                        {
                            // save success
                            transaction.Commit();
                            if (DeleteResult == "DELETE" && InsertResult == "Y" && UpdateResult == "Y")
                            {
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0008
                                };
                            }
                            else if (DeleteResult == "Y" && InsertResult == "Y" && UpdateResult == "UPDATE")
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
                                    Success = true,
                                    Message = MessageCode.MD0004
                                };
                            }


                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                // Save fail
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
                            Message = "Save data not success! + Exception: " + ex.Message,
                        };
                    }
                }
            }
        }
    }
}

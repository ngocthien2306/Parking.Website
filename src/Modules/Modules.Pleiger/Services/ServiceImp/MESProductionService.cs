using InfrastructureCore;
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
    public class MESProductionService : IMESProductionService
    {
        private const string SP_Name_Plan = "SP_MES_PRODUCTIONPLAN";
        private const string SP_Name_ItemSlipMaster = "SP_MES_ITEM_SLIP_MASTER";
        private const string SP_Name_ItemSlipDetail = "SP_MES_ITEM_SLIP_DETAIL";
        private const string SP_NAME_PRODUCTIONLINE = "SP_NAME_PRODUCTIONLINE";

        #region Get list production planning
        public List<MES_SaleProject> GetListData(string ProjectStatus, string projectCode, string prodcnCode, string itemCode, string itemName, string customer)
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[6];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectStatus";
                arrParams[2] = "@ProjectCode";
                arrParams[3] = "@ItemCode";
                arrParams[4] = "@ItemName";
                arrParams[5] = "@ProdcnCode";
                object[] arrParamsValue = new object[6];
                arrParamsValue[0] = "GetListData";
                arrParamsValue[1] = ProjectStatus;
                arrParamsValue[2] = projectCode;
                arrParamsValue[3] = itemCode;
                arrParamsValue[4] = itemName;
                arrParamsValue[5] = prodcnCode;
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_Name_Plan, arrParams, arrParamsValue);
                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }
        #endregion
        #region Get detail production planning
        public MES_SaleProject GetDetailData(string ProjectCode, string ProjectStatus)
        {
            var result = new MES_SaleProject();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@ProjectStatus";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetDetailData";
                arrParamsValue[1] = ProjectCode;
                arrParamsValue[2] = ProjectStatus;
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_Name_Plan, arrParams, arrParamsValue);
                result = data.FirstOrDefault();
            }
            return result;
        }
        #endregion
        #region Get prod plain lines master
        public List<MES_ProductLine> GetListProdLinesMaster()
        {
            var result = new List<MES_ProductLine>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                //arrParams[1] = "@ProjectCode";
                //arrParams[2] = "@ProjectStatus";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListProdLinesMaster";
                //arrParamsValue[1] = ProjectCode;
                //arrParamsValue[2] = ProjectStatus;
                var data = conn.ExecuteQuery<MES_ProductLine>(SP_Name_Plan, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
        #endregion
        #region Get List ProjectProdcnLines
        public List<MES_ProjectProdcnLines> GetListProjectProdcnLines(string projectCode)
        {
            var result = new List<MES_ProjectProdcnLines>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListProjectProdcnLines";
                arrParamsValue[1] = projectCode;
                var data = conn.ExecuteQuery<MES_ProjectProdcnLines>(SP_Name_Plan, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
        #endregion
        #region OnSaveProductPlainLines
        public Result SaveProductPlainLines(string ProjectCode, string ProdcnCode,
            List<MES_ProjectProdcnLines> lstAdd, List<MES_ProjectProdcnLines> lstEdit,
            List<MES_ProjectProdcnLines> lstDelete,
            string MaterWHCode, string ProdcnMessage, DateTime PlanDoneDate)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string[] arrParams = new string[6];
                        arrParams[0] = "@Method";
                        arrParams[1] = "@ProjectCode";
                        arrParams[2] = "@ProdcnCode";
                        arrParams[3] = "@MaterWHCode";
                        arrParams[4] = "@ProdcnMessage";
                        arrParams[5] = "@PlanDoneDate";
                        object[] arrParamsValue = new object[6];
                        arrParamsValue[0] = "UpdateWHnMess";
                        arrParamsValue[1] = ProjectCode;
                        arrParamsValue[2] = ProdcnCode;
                        arrParamsValue[3] = MaterWHCode;
                        arrParamsValue[4] = ProdcnMessage;
                        arrParamsValue[5] = PlanDoneDate;
                        var rs = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);

                        foreach (var item in lstAdd)
                        {
                            string[] arrParamsAdd = new string[7];
                            arrParamsAdd[0] = "@Method";
                            arrParamsAdd[1] = "@ProjectCode";
                            arrParamsAdd[2] = "@ProdcnCode";
                            arrParamsAdd[3] = "@ProdcnLineCode";
                            arrParamsAdd[4] = "@AssignedQty";
                            arrParamsAdd[5] = "@LineManager";
                            arrParamsAdd[6] = "@ProdcnLineState";
                            object[] arrParamsAddValue = new object[7];
                            arrParamsAddValue[0] = "InsertLine";
                            arrParamsAddValue[1] = ProjectCode;
                            arrParamsAddValue[2] = ProdcnCode;
                            arrParamsAddValue[3] = item.ProdcnLineCode;
                            arrParamsAddValue[4] = item.AssignedQty;
                            arrParamsAddValue[5] = item.LineManager;
                            arrParamsAddValue[6] = item.ProdcnLineState;
                            var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                        }
                        foreach (var item in lstEdit)
                        {
                            string[] arrParamsAdd = new string[7];
                            arrParamsAdd[0] = "@Method";
                            arrParamsAdd[1] = "@ProjectCode";
                            arrParamsAdd[2] = "@ProdcnCode";
                            arrParamsAdd[3] = "@ProdcnLineCode";
                            arrParamsAdd[4] = "@AssignedQty";
                            arrParamsAdd[5] = "@LineManager";
                            arrParamsAdd[6] = "@ProdcnLineState";
                            object[] arrParamsAddValue = new object[7];
                            arrParamsAddValue[0] = "UpdateLine";
                            arrParamsAddValue[1] = item.ProjectCode;
                            arrParamsAddValue[2] = ProdcnCode;
                            arrParamsAddValue[3] = item.ProdcnLineCode;
                            arrParamsAddValue[4] = item.AssignedQty;
                            arrParamsAddValue[5] = item.LineManager;
                            arrParamsAddValue[6] = item.ProdcnLineState;
                            var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                        }
                        foreach (var item in lstDelete)
                        {
                            string[] arrParamsAdd = new string[4];
                            arrParamsAdd[0] = "@Method";
                            arrParamsAdd[1] = "@ProjectCode";
                            arrParamsAdd[2] = "@ProdcnCode";
                            arrParamsAdd[3] = "@ProdcnLineCode";
                            object[] arrParamsAddValue = new object[4];
                            arrParamsAddValue[0] = "DeleteLine";
                            arrParamsAddValue[1] = ProjectCode;
                            arrParamsAddValue[2] = ProdcnCode;
                            arrParamsAddValue[3] = item.ProdcnLineCode;
                            var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                        }
                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }

            return result;
        }
        #endregion
        #region OnUpdateWorkPlan
        public Result OnUpdateWorkPlan(string ProjectCode, string ProdcnCode, string ProjectStatus, string RequestCode,
            string MaterialWarehouse, int OrderQty, string UserID)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string[] arrParamsAdd = new string[4];
                        arrParamsAdd[0] = "@Method";
                        arrParamsAdd[1] = "@ProjectCode";
                        arrParamsAdd[2] = "@ProdcnCode";
                        arrParamsAdd[3] = "@ProjectStatus";

                        object[] arrParamsAddValue = new object[4];
                        arrParamsAddValue[0] = "UpdateStatusProject";
                        arrParamsAddValue[1] = ProjectCode;
                        arrParamsAddValue[2] = ProdcnCode;
                        arrParamsAddValue[3] = ProjectStatus;
                        var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);

                        // Get ReqQty from MES_RequestPartList by Request code and item code
                        string[] arrParamsGetReqQty = new string[2];
                        arrParamsGetReqQty[0] = "@Method";
                        arrParamsGetReqQty[1] = "@RequestCode";

                        object[] arrParamsAddValueGetReqQty = new object[2];
                        arrParamsAddValueGetReqQty[0] = "GetReqQtyInRequestPartList";
                        arrParamsAddValueGetReqQty[1] = RequestCode;
                        var requestPartList = conn.ExecuteQuery<MES_RequestPartList>(SP_Name_Plan, arrParamsGetReqQty, arrParamsAddValueGetReqQty, transaction).ToList();

                        // 2020-08-20: Minh add
                        // Make the transaction slip to subtract material when click Start
                        // SlipType = 1 
                        // ReqQty from MES_RequestPartList
                        // RelNumber = Request code in table MES_RequestPartList
                        // Warehouse to = Finish warehouse
                        //======Insert item slip masert================
                        string[] arrParamsITM = new string[9];
                        arrParamsITM[0] = "@DIV";
                        arrParamsITM[1] = "@SlipType";
                        arrParamsITM[2] = "@SlipNumber";
                        arrParamsITM[3] = "@WHFromCode";
                        arrParamsITM[4] = "@WHToCode";
                        arrParamsITM[5] = "@UserCreated";
                        arrParamsITM[6] = "@RelNumber";
                        arrParamsITM[7] = "@Created_By";
                        arrParamsITM[8] = "@InventoryClosed";

                        object[] arrParamsITMValue = new object[9];
                        arrParamsITMValue[0] = "SaveMasterItemSlip";
                        arrParamsITMValue[1] = "1";
                        arrParamsITMValue[2] = "";
                        arrParamsITMValue[3] = MaterialWarehouse;// sai warehouse
                        arrParamsITMValue[4] = null;
                        arrParamsITMValue[5] = UserID;
                        arrParamsITMValue[6] = ProdcnCode;
                        arrParamsITMValue[7] = UserID;
                        arrParamsITMValue[8] = false;
                        var SlipNumber = conn.ExecuteScalar<string>(SP_Name_ItemSlipMaster, CommandType.StoredProcedure, arrParamsITM, arrParamsITMValue, transaction);

                        // check list requestPartList have item code and ReqQty
                        if (requestPartList != null)
                        {
                            int seq = 1;
                            foreach (var itemPartlist in requestPartList)
                            {
                                //insert item detail
                                string[] arrParamsITD = new string[7];
                                arrParamsITD[0] = "@DIV";
                                arrParamsITD[1] = "@SlipNumber";
                                arrParamsITD[2] = "@Seq";
                                arrParamsITD[3] = "@ItemCode";
                                arrParamsITD[4] = "@Qty";
                                arrParamsITD[5] = "@RelNumber";
                                arrParamsITD[6] = "@Created_By";
                                // insert slip detail, Minh change for update MesItemStock
                                object[] arrParamsITDValue = new object[7];
                                arrParamsITDValue[0] = "SaveDetailItemSlip";
                                arrParamsITDValue[1] = SlipNumber;
                                arrParamsITDValue[2] = seq++;
                                arrParamsITDValue[3] = itemPartlist.ItemCode;
                                arrParamsITDValue[4] = itemPartlist.ReqQty * OrderQty; // 20201019: Minh add calculate total raw material need
                                arrParamsITDValue[5] = ProdcnCode;
                                arrParamsITDValue[6] = UserID;
                                //  arrParamsITDValue[5] = "1";
                                var rsITD = conn.ExecuteNonQuery(SP_Name_ItemSlipDetail, CommandType.StoredProcedure, arrParamsITD, arrParamsITDValue, transaction);
                            }
                        }

                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }
            return result;
        }
        #endregion

        #region OnUpdateWorkCompleted
        public Result OnUpdateWorkCompleted(string ProjectCode, string ProdcnCode, string ProjectStatus)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string[] arrParamsAdd = new string[4];
                        arrParamsAdd[0] = "@Method";
                        arrParamsAdd[1] = "@ProjectCode";
                        arrParamsAdd[2] = "@ProdcnCode";
                        arrParamsAdd[3] = "@ProjectStatus";

                        object[] arrParamsAddValue = new object[4];
                        //arrParamsAddValue[0] = "UpdateStatusProject";
                        arrParamsAddValue[0] = "UpdateProjectWorkCompleted";
                        arrParamsAddValue[1] = ProjectCode;
                        arrParamsAddValue[2] = ProdcnCode;
                        arrParamsAddValue[3] = ProjectStatus;
                        var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }
            return result;
        }
        #endregion

        #region OnUpdateProdLineStatus
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectCode"></param>
        /// <param name="ProdcnCode"></param>
        /// <param name="ProdcnLineCode"></param>
        /// <param name="Status"></param>
        /// <param name="ItemCode"></param>
        /// <param name="FNWarehouse"></param>
        /// <returns></returns>
        public Result OnUpdateProdLineStatus(string ProjectCode, string ProdcnCode, string ProdcnLineCode, string Status, string ItemCode,
                    string FNWarehouse, string RequestCode, string FinishWarehouseCodeSlt, string UserID)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update ProjectLine status
                        string[] arrParamsAdd = new string[5];
                        arrParamsAdd[0] = "@Method";
                        arrParamsAdd[1] = "@ProjectCode";
                        arrParamsAdd[2] = "@ProdcnCode";
                        arrParamsAdd[3] = "@ProdcnLineCode";
                        arrParamsAdd[4] = "@ProdcnLineState";

                        object[] arrParamsAddValue = new object[5];
                        arrParamsAddValue[0] = "UpdateProdLineStatus";
                        arrParamsAddValue[1] = ProjectCode;
                        arrParamsAddValue[2] = ProdcnCode;
                        arrParamsAddValue[3] = ProdcnLineCode;
                        arrParamsAddValue[4] = Status;
                        var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);

                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = ex.Message;
                    }
                }
            }
            return result;
        }
        #endregion
        #region OnUpdateProdLineDoneQty
        public Result OnUpdateProdLineDoneQty(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string UserID, string UserName, string ItemCode, string WareHouseCode, string MasterWHCode)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //==========update and change status production lines===========
                        string[] arrParamsLines = new string[5];
                        arrParamsLines[0] = "@Method";
                        arrParamsLines[1] = "@ProjectCode";
                        arrParamsLines[2] = "@ProdcnCode";
                        arrParamsLines[3] = "@ProdcnLineCode";
                        arrParamsLines[4] = "@ProdDoneQty";

                        object[] arrParamsLinesValue = new object[5];
                        arrParamsLinesValue[0] = "UpdateProdLineDoneQty";
                        arrParamsLinesValue[1] = ProjectCode;
                        arrParamsLinesValue[2] = ProdcnCode;
                        arrParamsLinesValue[3] = ProdcnLineCode;
                        arrParamsLinesValue[4] = ProductionQuantity;
                        var rs = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsLines, arrParamsLinesValue, transaction);
                        //======Insert item slip master================
                        string[] arrParamsITM = new string[8];
                        arrParamsITM[0] = "@DIV";
                        arrParamsITM[1] = "@SlipType";
                        arrParamsITM[2] = "@SlipNumber";
                        arrParamsITM[3] = "@WHFromCode";
                        arrParamsITM[4] = "@WHToCode";
                        arrParamsITM[5] = "@UserCreated";
                        arrParamsITM[6] = "@RelNumber";
                        arrParamsITM[7] = "@InventoryClosed";

                        object[] arrParamsITMValue = new object[8];
                        arrParamsITMValue[0] = "SaveMasterItemSlip";
                        arrParamsITMValue[1] = "0";
                        arrParamsITMValue[2] = "";
                        arrParamsITMValue[3] = null; // 2020-08-18: change MasterWHCode to null, need check with Mr.Loc
                        arrParamsITMValue[4] = WareHouseCode;
                        arrParamsITMValue[5] = UserID;
                        arrParamsITMValue[6] = ProdcnCode;
                        arrParamsITMValue[7] = false;
                        var SlipNumber = conn.ExecuteScalar<string>(SP_Name_ItemSlipMaster, CommandType.StoredProcedure, arrParamsITM, arrParamsITMValue, transaction);

                        //=======insert production work result first===========
                        string[] arrParamsAdd = new string[10];
                        arrParamsAdd[0] = "@Method";
                        arrParamsAdd[1] = "@ProjectCode";
                        arrParamsAdd[2] = "@ProdcnCode";
                        arrParamsAdd[3] = "@ProdcnLineCode";
                        arrParamsAdd[4] = "@WorkDoneQty";
                        arrParamsAdd[5] = "@WorkPersonId";
                        arrParamsAdd[6] = "@WorkPersonName";
                        arrParamsAdd[7] = "@FProdStockInYN";
                        arrParamsAdd[8] = "@StkInSlipNumber";
                        arrParamsAdd[9] = "@Created_By";
                        object[] arrParamsAddValue = new object[10];
                        arrParamsAddValue[0] = "InsertProdWorkResult";
                        arrParamsAddValue[1] = ProjectCode;
                        arrParamsAddValue[2] = ProdcnCode;
                        arrParamsAddValue[3] = ProdcnLineCode;
                        arrParamsAddValue[4] = ProductionQuantity;
                        arrParamsAddValue[5] = UserID;
                        arrParamsAddValue[6] = UserName;
                        arrParamsAddValue[7] = true;
                        arrParamsAddValue[8] = SlipNumber;
                        arrParamsAddValue[9] = UserID;
                        var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);

                        // insert slip detail, Minh change for update MesItemStock
                        string[] arrParamsITD = new string[6];
                        arrParamsITD[0] = "@DIV";
                        arrParamsITD[1] = "@SlipNumber";
                        arrParamsITD[2] = "@Seq";
                        arrParamsITD[3] = "@ItemCode";
                        arrParamsITD[4] = "@Qty";
                        arrParamsITD[5] = "@RelNumber";
                        //insert item detail
                        object[] arrParamsITDValue = new object[6];
                        arrParamsITDValue[0] = "SaveDetailItemSlip";
                        arrParamsITDValue[1] = SlipNumber;
                        arrParamsITDValue[2] = 1;
                        arrParamsITDValue[3] = ItemCode;
                        arrParamsITDValue[4] = ProductionQuantity;
                        arrParamsITDValue[5] = ProdcnCode;
                        //  arrParamsITDValue[5] = "1";

                        var rsITD = conn.ExecuteNonQuery(SP_Name_ItemSlipDetail, CommandType.StoredProcedure, arrParamsITD, arrParamsITDValue, transaction);


                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }

            return result;
        }
        #endregion
        #region OnUpdateProdLineDoneQtyAndState
        public Result OnUpdateProdLineDoneQtyAndState(string ProjectCode, string ProdcnCode, string ProdcnLineCode, int ProductionQuantity, string Status, string UserID, string UserName, string ItemCode, string WareHouseCode, string MasterWHCode)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //==========update and change status production lines===========
                        string[] arrParamsLines = new string[6];
                        arrParamsLines[0] = "@Method";
                        arrParamsLines[1] = "@ProjectCode";
                        arrParamsLines[2] = "@ProdcnCode";
                        arrParamsLines[3] = "@ProdcnLineCode";
                        arrParamsLines[4] = "@ProdDoneQty";
                        arrParamsLines[5] = "@ProdcnLineState";

                        object[] arrParamsLinesValue = new object[6];
                        arrParamsLinesValue[0] = "UpdateProdLineDoneQtyAndState";
                        arrParamsLinesValue[1] = ProjectCode;
                        arrParamsLinesValue[2] = ProdcnCode;
                        arrParamsLinesValue[3] = ProdcnLineCode;
                        arrParamsLinesValue[4] = ProductionQuantity;
                        arrParamsLinesValue[5] = Status;
                        var rs = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsLines, arrParamsLinesValue, transaction);

                        //======Insert item slip master================
                        string[] arrParamsITM = new string[8];
                        arrParamsITM[0] = "@DIV";
                        arrParamsITM[1] = "@SlipType";
                        arrParamsITM[2] = "@SlipNumber";
                        arrParamsITM[3] = "@WHFromCode";
                        arrParamsITM[4] = "@WHToCode";
                        arrParamsITM[5] = "@UserCreated";
                        arrParamsITM[6] = "@RelNumber";
                        arrParamsITM[7] = "@InventoryClosed";

                        object[] arrParamsITMValue = new object[8];
                        arrParamsITMValue[0] = "SaveMasterItemSlip";
                        arrParamsITMValue[1] = "0";
                        arrParamsITMValue[2] = "";
                        arrParamsITMValue[3] = null; // 2020-08-18: change MasterWHCode to null, need check with Mr.Loc
                        arrParamsITMValue[4] = WareHouseCode;
                        arrParamsITMValue[5] = UserID;
                        arrParamsITMValue[6] = ProdcnCode;
                        arrParamsITMValue[7] = false;
                        var SlipNumber = conn.ExecuteScalar<string>(SP_Name_ItemSlipMaster, CommandType.StoredProcedure, arrParamsITM, arrParamsITMValue, transaction);

                        //=======insert production work result===========
                        string[] arrParamsAdd = new string[10];
                        arrParamsAdd[0] = "@Method";
                        arrParamsAdd[1] = "@ProjectCode";
                        arrParamsAdd[2] = "@ProdcnCode";
                        arrParamsAdd[3] = "@ProdcnLineCode";
                        arrParamsAdd[4] = "@WorkDoneQty";
                        arrParamsAdd[5] = "@WorkPersonId";
                        arrParamsAdd[6] = "@WorkPersonName";
                        arrParamsAdd[7] = "@FProdStockInYN";
                        arrParamsAdd[8] = "@StkInSlipNumber";
                        arrParamsAdd[9] = "@Created_By";
                        object[] arrParamsAddValue = new object[10];
                        arrParamsAddValue[0] = "InsertProdWorkResult";
                        arrParamsAddValue[1] = ProjectCode;
                        arrParamsAddValue[2] = ProdcnCode;
                        arrParamsAddValue[3] = ProdcnLineCode;
                        arrParamsAddValue[4] = ProductionQuantity;
                        arrParamsAddValue[5] = UserID;
                        arrParamsAddValue[6] = UserName;
                        arrParamsAddValue[7] = true;
                        arrParamsAddValue[8] = SlipNumber;
                        arrParamsAddValue[9] = UserID;
                        var rsAdd = conn.ExecuteNonQuery(SP_Name_Plan, CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);

                        //insert item detail
                        string[] arrParamsITD = new string[6];
                        arrParamsITD[0] = "@DIV";
                        arrParamsITD[1] = "@SlipNumber";
                        arrParamsITD[2] = "@Seq";
                        arrParamsITD[3] = "@ItemCode";
                        arrParamsITD[4] = "@Qty";
                        arrParamsITD[5] = "@RelNumber";

                        object[] arrParamsITDValue = new object[6];
                        arrParamsITDValue[0] = "SaveDetailItemSlip";
                        arrParamsITDValue[1] = SlipNumber;
                        arrParamsITDValue[2] = 1;
                        arrParamsITDValue[3] = ItemCode;
                        arrParamsITDValue[4] = ProductionQuantity;
                        arrParamsITDValue[5] = ProdcnCode;
                        var rsITD = conn.ExecuteNonQuery(SP_Name_ItemSlipDetail, CommandType.StoredProcedure, arrParamsITD, arrParamsITDValue, transaction);


                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }

            return result;
        }
        #endregion


        public List<MES_ProductLine> GetProductLine()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetProductLine";
                var result = conn.ExecuteQuery<MES_ProductLine>(SP_NAME_PRODUCTIONLINE, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        public List<MES_SaleProject> GetProjectName(string ProjectStatus)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectStatus";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetProjectName";
                arrParamsValue[1] = ProjectStatus;
                var result = conn.ExecuteQuery<MES_SaleProject>(SP_NAME_PRODUCTIONLINE, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        public List<MES_Item> GetItemName(string ProjectStatus)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectStatus";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetItemName";
                arrParamsValue[1] = ProjectStatus;
                var result = conn.ExecuteQuery<MES_Item>(SP_NAME_PRODUCTIONLINE, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        public List<MES_SaleProject> GetListDataNew(string ProductLineName, string ProjectName, string itemName)
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProductLineName";
                arrParams[2] = "@ProjectName";
                arrParams[3] = "@ItemName";
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetListData";
                arrParamsValue[1] = ProductLineName;
                arrParamsValue[2] = ProjectName;
                arrParamsValue[3] = itemName;
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_NAME_PRODUCTIONLINE, arrParams, arrParamsValue);
                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        #region OnUpdateWorkPlan
        public Result CheckQtyOfEachItemIsEnoughInWarehouse(string ProjectCode, string ProdcnCode, string RequestCode,
            string MaterialWarehouse, int OrderQty, string UserID)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        // Get ReqQty from MES_RequestPartList by Request code and item code
                        string[] arrParamsGetReqQty = new string[2];
                        arrParamsGetReqQty[0] = "@Method";
                        arrParamsGetReqQty[1] = "@RequestCode";

                        object[] arrParamsAddValueGetReqQty = new object[2];
                        arrParamsAddValueGetReqQty[0] = "GetReqQtyInRequestPartList";
                        arrParamsAddValueGetReqQty[1] = RequestCode;
                        var requestPartList = conn.ExecuteQuery<MES_RequestPartList>(SP_Name_Plan, arrParamsGetReqQty, arrParamsAddValueGetReqQty, transaction).ToList();

                        //List<string> itemCodeLst = new List<string>();
                        //Dictionary<string, string> itemCodeLst1 = new Dictionary<string, string>();
                        //// check list requestPartList have item code and ReqQty
                        //if (requestPartList != null)
                        //{
                        //    foreach (var itemPartlist in requestPartList)
                        //    {
                        //        itemCodeLst1.Keys = "ItemCode";
                        //        itemCodeLst1.Values = itemPartlist.ItemCode;
                        //        itemCodeLst.Add(itemCodeLst1);
                        //    }
                        //}

                        List<string> messsage = new List<string>();
                        if (requestPartList != null && requestPartList.Count > 0)
                        {
                            // Get ReqQty from MES_RequestPartList by Request code and item code
                            string[] arrParamsItemCodeLst = new string[3];
                            arrParamsItemCodeLst[0] = "@Method";
                            arrParamsItemCodeLst[1] = "@ItemCodeLst";
                            arrParamsItemCodeLst[2] = "@MaterWHCode";

                            object[] arrParamsAddValueItemCodeLst = new object[3];
                            arrParamsAddValueItemCodeLst[0] = "CheckQtyOfEachItemIsEnoughInWarehouse";
                            arrParamsAddValueItemCodeLst[1] = JsonConvert.SerializeObject(requestPartList);
                            arrParamsAddValueItemCodeLst[2] = MaterialWarehouse;
                            var checkWHItemList = conn.ExecuteQuery<MES_WHItemStock>(SP_Name_Plan, arrParamsItemCodeLst, arrParamsAddValueItemCodeLst, transaction).ToList();


                            for (int i = 0; i < requestPartList.Count; i++)
                            {
                                int sumQtyEachItem = 0;
                                if (checkWHItemList.Count == 0)
                                {
                                    messsage.Add(requestPartList[i].ItemCode);
                                }
                                else 
                                {
                                    for (int j = 0; j < checkWHItemList.Count; j++)
                                    {
                                        sumQtyEachItem = requestPartList[j].ReqQty * OrderQty;
                                        if (sumQtyEachItem > checkWHItemList[j].StockQty)
                                        {
                                            messsage.Add(requestPartList[j].ItemCode);
                                        }
                                    }
                                }

                                var item = checkWHItemList.Where(m => m.ItemCode == requestPartList[i].ItemCode).FirstOrDefault();
                                if (item == null)
                                {
                                    if(!messsage.Contains(requestPartList[i].ItemCode))
                                    {
                                        messsage.Add(requestPartList[i].ItemCode);
                                    }
                                }

                            }

                            //var query = from reqPartlst in requestPartList
                            //            join checkWHlst in checkWHItemList
                            //            on reqPartlst.ItemCode equals checkWHlst.ItemCode into newReqPartLst
                            //            from subquery in newReqPartLst.DefaultIfEmpty()
                            //            //where (reqPartlst.ReqQty * OrderQty) > checkWHlst.StockQty
                            //            //where reqPartlst.ItemCode != checkWHlst.ItemCode
                            //            select new MES_WHItemStock
                            //            {
                            //                ItemCode = reqPartlst.ItemCode
                            //            };
                            //int count = query.Count();
                        }
                     


                        Console.Write(messsage);
                        transaction.Commit();
                        result.Success = messsage.Count > 0 ? true : false;
                        result.Data = messsage;
                        result.Message = MessageCode.MD0004;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        result.Success = false;
                        result.Message = MessageCode.MD0005 + ex.Message;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}

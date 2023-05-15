using Modules.Pleiger.Services.IService;
using InfrastructureCore;
using Modules.Pleiger.Models;
using System.Collections.Generic;
using InfrastructureCore.DAL;
using System.Linq;
using Modules.Admin.Models;
using System.Data;
using System;
using Modules.Common.Models;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESItemSlipService : IMESItemSlipService
    {
        private const string SP_MES_ITEM_SLIP_MASTER = "SP_MES_ITEM_SLIP_MASTER";
        private const string SP_MES_ITEM_SLIP_MASTER_DELIVERY = "SP_MES_ITEM_SLIP_MASTER_DELIVERY";
        private const string SP_MES_ITEM_SLIP_DETAIL = "SP_MES_ITEM_SLIP_DETAIL";
        private const string SP_MES_NUMBER_UNIQUE = "SP_MES_NUMBER_UNIQUE";
        private const string SP_MES_MOVING_STOCK_MASTER = "SP_MES_MOVING_STOCK_MASTER";
        private const string SP_MES_MOVING_STOCK_DETAIL = "SP_MES_MOVING_STOCK_DETAIL";
        private const string SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN = "SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN";

        #region "ItemSlip Master"
        public List<MES_ItemSlipMaster> GetListMESItemSlipMaster(string startDate, string endDate, string status, string userProjectNo, string userPoNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@UserProjectCode", "@UserPONumber" },
                    new object[] { "SelectMasterStockInPO", startDate, endDate, status, userProjectNo, userPoNumber }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        #endregion

        #region "ItemSlip detail"
        public List<MES_ItemSlipDetail> GetListMESItemSlipDetail(string slipNumber, string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_DETAIL,
                    new string[] { "@DIV", "@SlipNumber", "@PONumber" },
                    new object[] { "SelectDetailStockInPO", slipNumber, poNumber }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public List<Combobox> GetPONumberForRelease()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<Combobox>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV" },
                    new object[] { "SelectPONumberStockInPO" }).ToList();

                return result;
            }
        }
        public List<MES_ItemSlipMaster> GetPOAllNumberSearch(string UserPONumber,string PartnerName)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV", "@UserPONumber", "@PartnerName" },
                    new object[] { "GetPOAllNumberSearch", UserPONumber, PartnerName }).ToList();
                    return result;
            }
        }
        public List<MES_ItemSlipMaster> GetPOAllNumberForRelease()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV" },
                    new object[] { "SelectAllPONumberStockInPO" }).ToList();

                return result;
            }
        }
        public MES_PORequest GetDataReferByPONumberForRelease(string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuerySingle<MES_PORequest>(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                    new string[] { "@DIV", "@PONumber" },
                    new object[] { "SelectDataReferByPONumber", poNumber });

                return result;
            }
        }

        #endregion

        #region Get master key slip type
        public string GetItemSlipMasterKey()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var now = DateTime.Now.ToString("yyyyMMdd"); // case sensitive
                var result = conn.ExecuteScalar<string>(SP_MES_NUMBER_UNIQUE, CommandType.StoredProcedure,
                    new string[] { "@MasterType", "@Date" },
                    new object[] { "Slip", now });

                return result;
            }
        }

        public List<MES_ItemSlipDetail> CreateGridItemSlipDtlByPONumber(string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                List<MES_ItemSlipDetail> slipDetailList = new List<MES_ItemSlipDetail>();
                var result = conn.ExecuteQuery<MES_PurchaseDetail>(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                    new string[] { "@DIV", "@PONumber" },
                    new object[] { "CreateStockInPO", poNumber }).ToList();

                // cast to Item Slip detail
                if (result != null && result.Count > 0)
                {

                    foreach (var item in result)
                    {
                        MES_ItemSlipDetail slipDetail = new MES_ItemSlipDetail
                        {
                            No = item.No,
                            ItemCode = item.ItemCode,
                            ItemName = item.ItemName,
                            Unit = item.Unit,
                            Qty = 1,// init default 1 item.Qty
                            POQty = item.POQty,
                            Cost = item.ItemPrice,
                            Amt = item.Amt,
                            Tax = item.Tax,
                            TaxAmt = item.TotalPrice,
                            TotalPOQty = item.TotalPOQty

                        };
                        slipDetailList.Add(slipDetail);
                    }
                }

                return slipDetailList;
            }
        }


        #endregion

        #region "Insert - Update - Delete - release item from Partner to Pleiger"
        public Result SaveDataMaterialInStock(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // calculate TotalAmt, taxAmt, TotaltaxAmt
                        decimal? TotalAmt = 0;
                        decimal? TaxAmt = 0;
                        decimal? TotalTaxAmt = 0;

                        foreach (var item in itemSlipDetail)
                        {
                            TotalAmt += item.Amt;
                            TaxAmt += item.Tax;
                            TotalTaxAmt += item.TaxAmt;
                        }
                        // insert new
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Insert"))
                        {

                            // return a SlipNumberNew in DB
                            var SlipNumberNew = conn.ExecuteScalar<string>(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", "@PartnerCode",
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark", "@InventoryClosed" ,"@CheckedGoodsReturn"},
                                new object[] { "SaveMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, itemSlipMaster.SlipType, itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark, false ,0},
                                transaction);
                            if (SlipNumberNew == null)
                            {
                                // Save fail
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                            else
                            {
                                // Save detail
                                var resultInsDtl = 0;
                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                            "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                        new object[] { "SaveDetailItemSlip", itemSlipMaster.RelNumber, SlipNumberNew, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                            item.Cost, item.Amt, item.Tax, item.TaxAmt, item.Remark, itemSlipMaster.RelNumber, item.RelSeq},
                                transaction);
                                }
                                transaction.Commit();
                                if (resultInsDtl > 0)
                                {
                                    // save success
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
                                        // Save fail
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                            }


                        }
                        // Edit Data
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Edit"))
                        {
                            // update Master
                            var updateMaster = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                    new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", "@PartnerCode",
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark" },
                                    new object[] { "UpdateMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, itemSlipMaster.SlipType, itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark },
                                    transaction);

                            if (updateMaster > 0)
                            {
                                // save success master
                                // update detail
                                var resultUpdateDtl = 0;
                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                            "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                        new object[] { "UpdateDetailItemSlip", itemSlipMaster.RelNumber, itemSlipMaster.SlipNumber, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                            item.Cost, item.Amt, item.Tax, item.TaxAmt, item.Remark, itemSlipMaster.RelNumber, item.RelSeq},
                                        transaction);
                                }
                                transaction.Commit();
                                if (resultUpdateDtl > 0)
                                {
                                    // save success
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
                                        // Save fail
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                            }
                            else
                            {
                                return new Result
                                {
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }


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

        public Result DeleteItemSlip(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // delete master
                        if (dataMst != null && dataMst.Count > 0)
                        {
                            var resultDelMst = 0;
                            foreach (var item in dataMst)
                            {
                                resultDelMst += conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber", "@PONumber" },
                                            new object[] { "DeleteMasterItemSlip", item.SlipNumber, item.RelNumber }, transaction);
                            }
                            transaction.Commit();
                            if (resultDelMst > 0)
                            {
                                // save success
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
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                        }
                        // delete detail
                        if (dataDtl != null && dataDtl.Count > 0)
                        {
                            var resultDelDtl = 0;
                            foreach (var item in dataDtl)
                            {
                                resultDelDtl += conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber", "@ItemCode", "@PONumber" },
                                            new object[] { "DeleteDetailItemSlip", item.SlipNumber, item.ItemCode, item.RelNumber }, transaction);
                            }
                            transaction.Commit();
                            if (resultDelDtl > 0)
                            {
                                // save success
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
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
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

        #endregion

        #region  Get Data ItemSlip Project have production complete  - prepare item deliver from Pleiger to Partner

        public List<MES_SaleProject> GetListProjectPrepareDelivery(string startDate, string endDate, string status, string projectNo, string itemCodeSearch, string itemNameSearch, string prodcnCodeSearch)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_SaleProject>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@ProjectNo", "@ItemCode", "@ItemName", "@ProdcnCode" },
                    new object[] { "GetListProjectPrepareDelivery", startDate, endDate, status, projectNo, itemCodeSearch, itemNameSearch, prodcnCodeSearch }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public List<MES_SaleProject> ProjectPrepareDeliveryDataGridMasterDetailView(string projectNo)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_SaleProject>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@ProjectNo" },
                    new object[] { "GetProjectProduction", projectNo }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        /// <summary>
        /// GET CUSTOMER LIST FROM MES_PARTNER
        /// </summary>
        /// <returns></returns>
        public List<Combobox> GetCustomerPartnerCode(string projectNo)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<Combobox>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@ProjectNo" },
                    new object[] { "GetCustomerPartnerCode", projectNo }).ToList();

                return result;
            }
        }

        /// <summary>
        /// GET PLEIGER WH CODE CONTAIN ITEM CODE IN MES_WHItemStock
        /// </summary>
        /// <returns></returns>
        public List<Combobox> GetWarehousePGItem(string ItemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<Combobox>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@ItemCode" },
                    new object[] { "GetWarehousePGItem", ItemCode }).ToList();

                return result;
            }
        }
        public List<MES_ItemSlipDetail> GetQtyWarehousePGItem(string ItemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@ItemCode" },
                    new object[] { "GetWarehousePGItem", ItemCode }).ToList();
                return result;
            }
        }
     


        /// <summary>
        /// GET partner warehouse by partner code
        /// </summary>
        /// <returns></returns>
        public List<MES_Warehouse> GetWarehouseOfPartner()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Warehouse>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV" },
                    new object[] { "GetWarehouseOfPartner" }).ToList();

                return result;
            }
        }


        public List<MES_ItemSlipDelivery> GetListSlipDelivery(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDelivery>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@ProjectCode" },
                    new object[] { "GetListSlipDelivery", projectCode }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        #endregion

        #region "Insert - Update - Delete - item deliver from Pleiger to Partner"
        public Result SaveDataDeliveryOutStock(string flag, List<MES_ItemSlipDelivery> slipDetails, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // insert new
                        //if (!string.IsNullOrEmpty(flag) && flag.Equals("Insert"))
                        //{

                        foreach (var item in slipDetails)
                        {
                            // check State of row inserted
                            if (item.State != null && item.State == "INSERTED")
                            {
                                // return a SlipNumberNew in DB
                                // RelNumber is ProjectCode
                                var SlipNumberNew = conn.ExecuteScalar<string>(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                    new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", "@PartnerCode",
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark", "@InventoryClosed" },
                                    new object[] { "SaveMasterItemSlip", "", item.SlipYMD, "6", item.PartnerCode,
                                            item.WHFromCode, item.WHToCode, item.ProjectCode, userModify, item.Amt, item.Tax,
                                            item.TaxAmtDtl, item.Remark, false },
                                    transaction);

                                if (SlipNumberNew == null)
                                {
                                    // Save fail
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                                else
                                {
                                    // Save detail
                                    // RelNumber is ProjectCode
                                    var resultInsDtl = 0;
                                    int seq = 1;
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                                    "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                            new object[] { "SaveDetailItemSlip", "", SlipNumberNew, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                                    item.Cost, item.Amt, item.Tax, item.TaxAmtDtl, item.RemarkDtl, item.ProjectCode, item.RelSeq},
                                            transaction);
                                }
                                // update project Status = PJST06

                                string[] arrParamsAdd = new string[2];
                                arrParamsAdd[0] = "@Method";
                                arrParamsAdd[1] = "@ProjectCode";

                                object[] arrParamsAddValue = new object[2];
                                //arrParamsAddValue[0] = "UpdateStatusProject";
                                arrParamsAddValue[0] = "UpdateProjectDelivery";
                                arrParamsAddValue[1] = item.ProjectCode;
                                var rsAdd = conn.ExecuteNonQuery("SP_MES_PRODUCTIONPLAN", CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
                            }
                            // check State of row modified
                            if (item.State != null && item.State == "UPDATED")
                            {
                                // update Master
                                var updateMaster = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", "@PartnerCode",
                                                    "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark" },
                                        new object[] { "UpdateMasterItemSlip", item.SlipNumber, item.SlipYMD, "6", item.PartnerCode,
                                                    item.WHFromCode, item.WHToCode, item.ProjectCode, userModify, item.Amt, item.Tax,
                                                    item.TaxAmtDtl, item.Remark },
                                        transaction);

                                if (updateMaster > 0)
                                {
                                    // save success master
                                    // update detail
                                    var resultUpdateDtl = 0;
                                    int seq = 1;
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                                    "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                            new object[] { "UpdateDetailItemSlip", "", item.SlipNumber, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                                    item.Cost, item.Amt, item.Tax, item.TaxAmt, item.Remark, item.ProjectCode, item.RelSeq},
                                            transaction);
                                }
                                else
                                {
                                    return new Result
                                    {
                                        // Save fail
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                            }
                        }
                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
                        //}
                        // Edit Data
                        //if (!string.IsNullOrEmpty(flag) && flag.Equals("Edit"))
                        //{

                        //}
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

        public Result DeleteItemSlipDelivery(List<MES_ItemSlipDelivery> data, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (data != null && data.Count > 0)
                        {
                            foreach (var item in data)
                            {
                                // delete master and detail
                                var resultDelMst = 0;
                                resultDelMst += conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER_DELIVERY, CommandType.StoredProcedure,
                                                new string[] { "@DIV", "@SlipNumber", "@ItemCode" },
                                                new object[] { "DeleteItemSlipDelivery", item.SlipNumber, item.ItemCode }, transaction);
                            }
                            // lúc delete phải check state của project code: chưa làm nè
                            transaction.Commit();
                            result.Success = true;
                            result.Message = MessageCode.MD0008;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Please choice data which need to deleted.";
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

        #endregion


        #region Moving Stock in Pleiger

        
        public List<MES_ItemSlipMaster> GetListMovingStockItem(string startDate, string endDate, string status, string fromWH, string toWH)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_MOVING_STOCK_MASTER,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@FromWH", "@ToWH" },
                    new object[] { "SelectMovingStockMaster", startDate, endDate, status, fromWH, toWH }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        public List<MES_ItemSlipDetail> GetListMovingStockItemDetail(string slipNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_DETAIL,
                    new string[] { "@DIV", "@SlipNumber" },
                    new object[] { "SelectDetailStockInPO", slipNumber }).ToList();//SelectMovingStockDetail

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public Result SaveMovingStockItem(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // insert new
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Insert"))
                        {

                            // return a SlipNumberNew in DB
                            var SlipNumberNew = conn.ExecuteScalar<string>(SP_MES_MOVING_STOCK_MASTER, CommandType.StoredProcedure,
                                new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", 
                                            "@WHFromCode", "@WHToCode", "@UserCreated", "@Remark" },
                                new object[] { "SaveMasterMovingStock", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, 11,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, userModify, itemSlipMaster.Remark },
                                transaction);
                            if (SlipNumberNew == null)
                            {
                                // Save fail
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                            else
                            {
                                // Save detail
                                var resultInsDtl = 0;
                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_MOVING_STOCK_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty", "@Remark"},
                                        new object[] { "SaveMovingStockDetail", SlipNumberNew, seq++ ,item.ItemCode, item.Unit, item.Qty, item.Remark},
                                transaction);
                                }
                                transaction.Commit();
                                if (resultInsDtl > 0)
                                {
                                    // save success
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
                                        // Save fail
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                            }


                        }
                        // Edit Data
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Edit"))
                        {
                            // update Master
                            var updateMaster = conn.ExecuteNonQuery(SP_MES_MOVING_STOCK_MASTER, CommandType.StoredProcedure,
                                    new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", 
                                            "@WHFromCode", "@WHToCode",  "@UserCreated", "@Remark" },
                                    new object[] { "UpdateMasterMovingStock", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, 11,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, userModify, itemSlipMaster.Remark },
                                    transaction);

                            if (updateMaster > 0)
                            {
                                // save success master
                                // update detail
                                var resultUpdateDtl = 0;
                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_MOVING_STOCK_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty", "@Remark" },
                                        new object[] { "UpdateMovingStockDetail", itemSlipMaster.SlipNumber, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                            item.Remark},
                                        transaction);
                                }
                                transaction.Commit();
                                if (resultUpdateDtl > 0)
                                {
                                    // save success
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
                                        // Save fail
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                            }
                            else
                            {
                                return new Result
                                {
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }


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

        public Result DeleteMovingStockItem(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // delete master
                        if (dataMst != null && dataMst.Count > 0)
                        {
                            var resultDelMst = 0;
                            foreach (var item in dataMst)
                            {
                                resultDelMst += conn.ExecuteNonQuery(SP_MES_MOVING_STOCK_MASTER, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber"},
                                            new object[] { "DeleteMasterMovingStock", item.SlipNumber }, transaction);
                            }
                            transaction.Commit();
                            if (resultDelMst > 0)
                            {
                                // save success
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
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                        }
                        // delete detail
                        if (dataDtl != null && dataDtl.Count > 0)
                        {
                            var resultDelDtl = 0;
                            foreach (var item in dataDtl)
                            {
                                resultDelDtl += conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber", "@ItemCode" },
                                            new object[] { "DeleteMovingStockDetail", item.SlipNumber, item.ItemCode}, transaction);
                            }
                            transaction.Commit();
                            if (resultDelDtl > 0)
                            {
                                // save success
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
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
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


        #endregion

        #region Form Search View
        public List<MES_SaleProject> GetProjectNameInProduction()
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@DIV";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetProjectNameInProduction";
                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_ITEM_SLIP_MASTER_DELIVERY, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }
        #endregion

        #region Goods Return PO
        public List<MES_ItemSlipMaster> GetItemSlipMasterGoodsReturnPO(string startDate, string endDate, string status, string partnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@PartnerCode" },
                    new object[] { "GetMasterGoodsReturnPO", startDate, endDate, status, partnerCode}).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public List<Combobox> GetPONumberHaveReceipt()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<Combobox>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN,
                    new string[] { "@DIV" },
                    new object[] { "GetPONumberHaveReceipt" }).ToList();

                return result;
            }
        }
        // Quan add 2020/09/29
        // GetPONumberHaveReceipt Popup Search
        public List<MES_ItemSlipMaster> GetPONumberHaveReceiptSearch(string UserPONumber, string PartnerName)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN,
                    new string[] { "@DIV", "@UserPONumber", "@PartnerName" },
                    new object[] { "GetPONumberHaveReceiptSearch", UserPONumber, PartnerName }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        public List<MES_ItemSlipDetail> CreateGridItemSlipDtlByPONumberInGoodsReturn(string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                List<MES_ItemSlipDetail> slipDetailList = new List<MES_ItemSlipDetail>();
                var result = conn.ExecuteQuery<MES_PurchaseDetail>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN, CommandType.StoredProcedure,
                    new string[] { "@DIV", "@PONumber" },
                    new object[] { "CreateGoodsReturnPO", poNumber }).ToList();

                // cast to Item Slip detail
                if (result != null && result.Count > 0)
                {

                    foreach (var item in result)
                    {
                        MES_ItemSlipDetail slipDetail = new MES_ItemSlipDetail
                        {
                            No = item.No,
                            ItemCode = item.ItemCode,
                            ItemName = item.ItemName,
                            Unit = item.Unit,
                            Qty = 0,// init default 0 item.Qty
                            POQty = item.POQty,
                            DeliverQty = item.DeliverQty, // số lượng đã dc giao
                            Cost = item.ItemPrice,
                            Amt = item.Amt,
                            Tax = item.Tax,
                            TaxAmt = item.TotalPrice

                        };
                        slipDetailList.Add(slipDetail);
                    }
                }

                return slipDetailList;
            }
        }

        /// <summary>
        /// Goods Return 
        /// Sliptype = 3:반품출고	Xuat hang bi tra lai
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="itemSlipMaster"></param>
        /// <param name="itemSlipDetail"></param>
        /// <param name="userModify"></param>
        /// <returns></returns>
        public Result SaveDataGoodsReturn(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // calculate TotalAmt, taxAmt, TotaltaxAmt
                        decimal? TotalAmt = 0;
                        decimal? TaxAmt = 0;
                        decimal? TotalTaxAmt = 0;

                        foreach (var item in itemSlipDetail)
                        {
                            TotalAmt += item.Amt;
                            TaxAmt += item.Tax;
                            TotalTaxAmt += item.TaxAmt;
                        }
                        // insert new
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Insert"))
                        {

                            // return a SlipNumberNew in DB
                            var SlipNumberNew = conn.ExecuteScalar<string>(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", "@PartnerCode",
                                               "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", 
                                               "@TotalTaxAmt", "@Remark", "@InventoryClosed","@CheckedGoodsReturn","@SlipNumberGoodReciep"},
                                new object[] { "SaveMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, "3", itemSlipMaster.PartnerCode,
                                               itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                               TotalTaxAmt, itemSlipMaster.Remark, false ,1,itemSlipMaster.SlipNumberGoodReciep},
                                transaction);
                            if (SlipNumberNew == null)
                            {
                                // Save fail
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                            else
                            {
                                // Save detail
                                var resultInsDtl = 0;
                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                                       "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                        new object[] { "SaveDetailItemSlip", itemSlipMaster.RelNumber, SlipNumberNew, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                                       item.Cost, item.Amt, item.Tax, item.TaxAmt, item.Remark, itemSlipMaster.RelNumber, item.RelSeq},
                                transaction);
                                }
                                transaction.Commit();
                                if (resultInsDtl > 0)
                                {
                                    // save success
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
                                        // Save fail
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                            }


                        }
                        // Edit Data
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Edit"))
                        {
                            // update Master
                            var updateMaster = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                    new string[] { "@DIV", "@SlipNumber", "@SlipYMD", "@SlipType", "@PartnerCode",
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark" },
                                    new object[] { "UpdateMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, "3", itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark },
                                    transaction);

                            if (updateMaster > 0)
                            {
                                // save success master
                                // update detail
                                var resultUpdateDtl = 0;
                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                            "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                        new object[] { "UpdateDetailItemSlip", itemSlipMaster.RelNumber, itemSlipMaster.SlipNumber, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                            item.Cost, item.Amt, item.Tax, item.TaxAmt, item.Remark, itemSlipMaster.RelNumber, item.RelSeq},
                                        transaction);
                                }
                                transaction.Commit();
                                if (resultUpdateDtl > 0)
                                {
                                    // save success
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
                                        // Save fail
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                            }
                            else
                            {
                                return new Result
                                {
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }


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

        public Result DeleteDataGoodsReturn(List<MES_ItemSlipMaster> dataMst, List<MES_ItemSlipDetail> dataDtl, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // delete master
                        if (dataMst != null && dataMst.Count > 0)
                        {
                            var resultDelMst = 0;
                            foreach (var item in dataMst)
                            {
                                resultDelMst += conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber", "@PONumber", "@SlipNumberGoodReciep" },
                                            new object[] { "DeleteMasterItemSlip", item.SlipNumber, item.RelNumber,item.SlipNumberGoodReciep }, transaction);
                            }
                            transaction.Commit();
                            if (resultDelMst > 0)
                            {
                                // save success
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
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                        }
                        // delete detail
                        if (dataDtl != null && dataDtl.Count > 0)
                        {
                            var resultDelDtl = 0;
                            foreach (var item in dataDtl)
                            {
                                resultDelDtl += conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber", "@ItemCode", "@PONumber" },
                                            new object[] { "DeleteDetailItemSlip", item.SlipNumber, item.ItemCode, item.RelNumber }, transaction);
                            }
                            transaction.Commit();
                            if (resultDelDtl > 0)
                            {
                                // save success
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
                                    // Save fail
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
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

        public List<MES_ItemSlipDetail> GetListMESItemSlipDetailGoodsReturn(string slipNumber, string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN,
                    new string[] { "@DIV", "@SlipNumber", "@PONumber" },
                    new object[] { "GoodsReturnDetail", slipNumber, poNumber }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        #endregion
    }
}

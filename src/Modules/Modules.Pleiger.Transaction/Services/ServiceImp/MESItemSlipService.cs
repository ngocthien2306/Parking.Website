using InfrastructureCore;
using System.Collections.Generic;
using InfrastructureCore.DAL;
using System.Linq;
using Modules.Admin.Models;
using System.Data;
using System;
using Modules.Common.Models;
using Modules.Pleiger.Transaction.Services.IService;
using Modules.Pleiger.CommonModels;
using System.Threading;
using Newtonsoft.Json;

namespace Modules.Pleiger.Transaction.Services.ServiceImp
{
    public class MESItemSlipService : IMESItemSlipService
    {
        private const string SP_MES_ITEM_SLIP_MASTER = "SP_MES_ITEM_SLIP_MASTER";
        private const string SP_MES_ITEM_SLIP_MASTER_DELIVERY = "SP_MES_ITEM_SLIP_MASTER_DELIVERY";
        private const string SP_MES_ITEM_SLIP_DETAIL = "SP_MES_ITEM_SLIP_DETAIL";
        private const string SP_MES_NUMBER_UNIQUE = "SP_MES_NUMBER_UNIQUE";
        private const string SP_MES_MOVING_STOCK_MASTER = "SP_MES_MOVING_STOCK_MASTER";
        private const string SP_MES_MOVING_STOCK_DETAIL = "SP_MES_MOVING_STOCK_DETAIL";

        // Return Goods PO
        private const string SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN = "SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN";
        // Return Goods Delivery
        private const string SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN_DELIVERY = "SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN_DELIVERY";

        #region "ItemSlip Master"
        public List<MES_ItemSlipMaster> GetListMESItemSlipMaster(string startDate, string endDate, string status, string userProjectNo, string userPoNumber, string goodReceipt)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@UserProjectCode", "@UserPONumber", "@GoodReceiptNumber" },
                    new object[] { "SelectMasterStockInPO", startDate, endDate, status, userProjectNo, userPoNumber, goodReceipt }).ToList();

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
        public List<MES_ItemSlipDetail> GetListMESItemSlipDetailForReleasePartner(string slipNumber, string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_DETAIL,
                    new string[] { "@DIV", "@SlipNumber", "@PONumber" },
                    new object[] { "GetListMESItemSlipDetailForReleasePartner", slipNumber, poNumber }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        
        public List<MES_ItemSlipDetail> GetItemSlipDetailInPopup(string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_DETAIL,
                    new string[] { "@DIV", "@PONumber" },
                    new object[] { "GetItemSlipDetailInPopup", poNumber }).ToList();

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
        public List<MES_ItemSlipMaster> GetPOAllNumberSearch(string UserPONumber, string PartnerName)
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
        public List<MES_ItemPO> GetItemSlipDetailInPopupSearchPO(string poNumber)
        {
            var result = new List<MES_ItemPO>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemPORequest";
                arrParamsValue[1] = poNumber;
                var data = conn.ExecuteQuery<MES_ItemPO>("SP_MES_POREQUEST", arrParams, arrParamsValue);
                result = data.ToList();
            }
            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;
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
                            TotalPOQty = item.TotalPOQty,
                            PleigerRemark = item.PleigerRemark,
                            PleigerRemark2 = item.PleigerRemark2


                        };
                        slipDetailList.Add(slipDetail);
                    }
                }

                return slipDetailList;
            }
        }
        public List<MES_ItemSlipDetail> CreateGridItemSlipDtlByPONumberPartner(string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                List<MES_ItemSlipDetail> slipDetailList = new List<MES_ItemSlipDetail>();
                var result = conn.ExecuteQuery<MES_PurchaseDetail>(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                    new string[] { "@DIV", "@PONumber" },
                    new object[] { "CreateStockInPOPartner", poNumber }).ToList();

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
                            TotalPOQty = item.TotalPOQty,
                            PleigerRemark = item.PleigerRemark,
                            PleigerRemark2 = item.PleigerRemark2,
                            DeliveryDate = item.DeliveryDate



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
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark", "@InventoryClosed" ,"@CheckedGoodsReturn","@Confirm"},
                                new object[] { "SaveMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, itemSlipMaster.SlipType, itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark, false ,0,1},
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
                                var resultUpdateStatus = 0;
                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                            "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                        new object[] { "SaveDetailItemSlip_GoodsReciept", itemSlipMaster.RelNumber, SlipNumberNew, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                            item.Cost, item.Amt, item.Tax, item.TaxAmt, item.Remark, itemSlipMaster.RelNumber, item.RelSeq},
                                transaction);
                                }
                                resultUpdateStatus = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                  new string[] { "@DIV", "@PONumber" },
                                  new object[] { "UpdateStatusWhenGoodsRecieptConfirmedAll", itemSlipMaster.RelNumber },
                                  transaction);
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
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark","@Confirm" },
                                    new object[] { "UpdateMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, itemSlipMaster.SlipType, itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark, 1 },
                                    transaction);

                            if (updateMaster > 0)
                            {
                                // save success master
                                // update detail
                                var resultUpdateDtl = 0;
                                var resultUpdateStatus = 0;

                                int seq = 1;
                                foreach (var item in itemSlipDetail)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@PONumber", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty",
                                            "@Cost", "@Amt", "@Tax", "@TaxAmt", "@Remark", "@RelNumber", "@RelSeq" },
                                        new object[] { "UpdateDetailItemSlip_GoodsReciept", itemSlipMaster.RelNumber, itemSlipMaster.SlipNumber, seq++ ,item.ItemCode, item.Unit, item.Qty,
                                            item.Cost, item.Amt, item.Tax, item.TaxAmt, item.Remark, itemSlipMaster.RelNumber, item.RelSeq},
                                        transaction);
                                }

                                resultUpdateStatus = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                   new string[] { "@DIV", "@PONumber" },
                                   new object[] { "UpdateStatusWhenGoodsRecieptConfirmedAll", itemSlipMaster.RelNumber },
                                   transaction);
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

                            //transaction.Commit();
                            //if (resultDelMst > 0)
                            //{
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

                                if (resultDelDtl >= 0)
                                {
                                    // save success
                                    foreach (var item in dataMst)
                                    {
                                        resultDelMst += conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                                    new string[] { "@DIV", "@SlipNumber", "@PONumber" },
                                                    new object[] { "DeleteMasterItemSlip", item.SlipNumber, item.RelNumber }, transaction);
                                    }
                                    transaction.Commit();
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004
                                    };
                                }

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

        public Result DeleteItemSlipDetail(MES_ItemSlipDetail dataDtl)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var resultDelDtl = 0;

                        resultDelDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                                 new string[] { "@DIV", "@SlipNumber", "@ItemCode", "@PONumber" },
                                                 new object[] { "DeleteDetailItemSlip", dataDtl.SlipNumber, dataDtl.ItemCode, dataDtl.RelNumber }, transaction);

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

        public Result UpdateSlipDate(string SlipNumber, DateTime? SlipDate)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var result = 0;

                        result = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                                 new string[] { "@DIV", "@SlipNumber", "@SlipYMD" },
                                                 new object[] { "UpdateSlipDate", SlipNumber, SlipDate }, transaction);

                        transaction.Commit();

                        if (result > 0)
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

        #region  Get Data ItemSlip Project have production complete  - prepare item deliver from Pleiger to Partner

        public List<MES_SaleProject> GetListProjectPrepareDelivery(string startDate, string endDate, string status, string projectNo, string itemCodeSearch, string itemNameSearch, string prodcnCodeSearch, string projectOrderType, string saleOrderProjectName, string userProjectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_SaleProject>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@ProjectNo", "@ItemCode", "@ItemName", "@ProdcnCode", "@SalesOrderProjectName", "@ProjectOrderType", "@UserProjectCode" },
                    new object[] { "GetListProjectPrepareDelivery", startDate, endDate, status, projectNo, itemCodeSearch, itemNameSearch, prodcnCodeSearch, saleOrderProjectName, projectOrderType, userProjectCode }).ToList();

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
                    //if (x.No == 1)
                    //{
                    //    x.QtyInStockFake = int.Parse(x.Qty.ToString()) + x.QtyInStockFake;
                    //}

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
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark", "@InventoryClosed", "@ProjectCode" },
                                    new object[] { "SaveMasterItemSlip", "", item.SlipYMD, "6", item.PartnerCode,
                                            item.WHFromCode, item.WHToCode, item.ProjectCode, userModify, item.Amt, item.Tax,
                                            item.TaxAmtDtl, item.Remark, false, item.ProjectCode },
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
                                // Quanchange update project Status = PJST07
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

                                    // Quan add 2020-12-03
                                    // update project Status = PJST06
                                    // Quanchange update project Status = PJST07

                                    string[] arrParamsAdd = new string[2];
                                    arrParamsAdd[0] = "@Method";
                                    arrParamsAdd[1] = "@ProjectCode";
                                    object[] arrParamsAddValue = new object[2];
                                    //arrParamsAddValue[0] = "UpdateStatusProject";
                                    arrParamsAddValue[0] = "UpdateProjectDelivery";
                                    arrParamsAddValue[1] = item.ProjectCode;
                                    var rsAdd = conn.ExecuteNonQuery("SP_MES_PRODUCTIONPLAN", CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);

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
                                        new string[] { "@DIV", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty", "@Remark" },
                                        new object[] { "SaveMovingStockDetail", SlipNumberNew, seq++, item.ItemCode, item.Unit, item.Qty, item.Remark },
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
                                            new string[] { "@DIV", "@SlipNumber" },
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
                                            new object[] { "DeleteMovingStockDetail", item.SlipNumber, item.ItemCode }, transaction);
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
        public List<MES_ItemSlipMaster> GetItemSlipMasterGoodsReturnPO(string startDate, string endDate, string status, string partnerCode, string userProjectCode, string userPoNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@PartnerCode", "@UserProjectCode", "@UserPONumber" },
                    new object[] { "GetMasterGoodsReturnPO", startDate, endDate, status, partnerCode, userProjectCode, userPoNumber }).ToList();

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
                            TaxAmt = item.TotalPrice,
                            MaximumreturnQty = item.MaximumreturnQty

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
                                               "@TotalTaxAmt", "@Remark", "@InventoryClosed","@CheckedGoodsReturn","@SlipNumberGoodReceipt"},
                                new object[] { "SaveMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, "3", itemSlipMaster.PartnerCode,
                                               itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                               TotalTaxAmt, itemSlipMaster.Remark, false ,1,itemSlipMaster.SlipNumberGoodReceipt},
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
                                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
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
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark",
                                            "@InventoryClosed","@CheckedGoodsReturn","@SlipNumberGoodReceipt"
                                    },
                                    new object[] { "UpdateMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, "3", itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark , false, 1, itemSlipMaster.SlipNumberGoodReceipt},
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
                                            new string[] { "@DIV", "@SlipNumber", "@PONumber", "@SlipNumberGoodReceipt" },
                                            new object[] { "DeleteMasterItemSlip", item.SlipNumber, item.RelNumber, item.SlipNumberGoodReceipt }, transaction);
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

        #region Goods Return Delivery
        public List<MES_ItemSlipDetail> GetListMESItemSlipDetailGoodsReturnDelivery(string slipNumber, string poNumber, string slipNumberGoodReceipt)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN_DELIVERY,
                    new string[] { "@DIV", "@SlipNumber", "@PONumber", "@SlipNumberGoodReceipt" },
                    new object[] { "GoodsReturnDeliveryDetail", slipNumber, poNumber, slipNumberGoodReceipt }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        public List<MES_ItemSlipMaster> GetItemSlipMasterGoodsReturnDelivery(string startDate, string endDate, string status,
                        string partnerCode, string userProjectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN_DELIVERY,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@PartnerCode", "@UserProjectCode" },
                    new object[] { "GetMasterGoodsReturnDelivery", startDate, endDate, status, partnerCode, userProjectCode }).ToList();
                return result;
            }
        }

        public List<MES_ItemSlipMaster> GetProjectHaveGoodsIssuesSearch(string UserProjectCode, string PartnerName)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN_DELIVERY,
                    new string[] { "@DIV", "@UserProjectCode", "@PartnerName" },
                    new object[] { "GetProjectHaveGoodsIssuesSearch", UserProjectCode, PartnerName }).ToList();
                return result;
            }
        }
        public List<MES_ItemSlipMaster> GetListGoodsDeliveries(string UserProjectCode, string PartnerName, string ProjectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN_DELIVERY,
                    new string[] { "@DIV", "@UserProjectCode", "@PartnerName", "@ProjectNo" },
                    new object[] { "GetListGoodsDeliveries", UserProjectCode, PartnerName, ProjectCode }).ToList();
                return result;
            }
        }

        public List<MES_ItemSlipDetail> CreateGridItemSlipDtlByProjectInGoodsReturnDelivery(string projectCode, string slipNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                List<MES_ItemSlipDetail> slipDetailList = new List<MES_ItemSlipDetail>();
                var result = conn.ExecuteQuery<MES_PurchaseDetail>(SP_MES_ITEM_SLIP_MASTER_GOODS_RETURN_DELIVERY, CommandType.StoredProcedure,
                    new string[] { "@DIV", "@ProjectNo", "@SlipNumber" },
                    new object[] { "CreateGoodsReturnDelivery", projectCode, slipNumber }).ToList();

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
                            //POQty = item.POQty,
                            DeliverQty = item.DeliverQty, // số lượng đã dc giao
                            Cost = 0,
                            Amt = 0,
                            Tax = 0,
                            TaxAmt = 0,
                            MaximumreturnQty = 0

                        };
                        slipDetailList.Add(slipDetail);
                    }
                }

                return slipDetailList;
            }
        }

        /// <summary>
        /// Giao hàng cho customer bị trả lại
        /// SlipType = 2
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="itemSlipMaster"></param>
        /// <param name="itemSlipDetail"></param>
        /// <param name="userModify"></param>
        /// <returns></returns>
        public Result SaveDataGoodsReturnDelivery(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify, string transSlipNumber)
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
                                               "@TotalTaxAmt", "@Remark", "@InventoryClosed","@CheckedGoodsReturn","@SlipNumberGoodReceipt"},
                                new object[] { "SaveMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, "2", itemSlipMaster.PartnerCode,
                                               itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                               TotalTaxAmt, itemSlipMaster.Remark, false ,1,itemSlipMaster.SlipNumberGoodIssues},
                                transaction);

                            // Quan add 2020-12-03                            
                            // Quan update project Status = PJST07
                            // return a SlipNumberNew in DB                          
                            string[] arrParamsAdd = new string[2];
                            arrParamsAdd[0] = "@Method";
                            arrParamsAdd[1] = "@ProjectCode";
                            object[] arrParamsAddValue = new object[2];
                            //arrParamsAddValue[0] = "UpdateStatusProject";
                            arrParamsAddValue[0] = "UpdateStatusProjectReturn";
                            arrParamsAddValue[1] = itemSlipMaster.RelNumber;
                            var rsAdd = conn.ExecuteNonQuery("SP_MES_PRODUCTIONPLAN", CommandType.StoredProcedure, arrParamsAdd, arrParamsAddValue, transaction);
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
                                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
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
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark",
                                            "@InventoryClosed","@CheckedGoodsReturn","@SlipNumberGoodReceipt"
                                    },
                                    new object[] { "UpdateMasterItemSlip", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, "2", itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark , false, 1, itemSlipMaster.SlipNumberGoodReceipt},
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
        #endregion

        public List<MES_ItemSlipDetail> getStockQtyByItemCode(string ItemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipDetail>(SP_MES_ITEM_SLIP_MASTER_DELIVERY,
                    new string[] { "@DIV", "@ItemCode" },
                    new object[] { "getStockQtyByItemCode", ItemCode }).ToList();
                return result;
            }
        }

        // Slide 40    New menu, This menu is for partner use only.
        // Purchase Order Delivery
        public List<MES_ItemSlipMaster> GetListPODeliverybyPartner(string startDate, string endDate, string status, string userProjectNo, string userPoNumber, string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@UserProjectCode", "@UserPONumber" },
                    new object[] { "GetListPODeliverybyPartner", startDate, endDate, status, userProjectNo, userPoNumber, UserCode }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        public List<MES_ItemSlipMaster> GetPOAllNumberSearchByPartner(string UserPONumber, string PartnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV", "@UserPONumber", "@PartnerCode" },
                    new object[] { "GetPOAllNumberSearchByPartner", UserPONumber, PartnerCode }).ToList();
                return result;
            }
        }
        public List<MES_ItemSlipMaster> GetListMESItemSlipMasterForReleasePartner(string startDate, string endDate, string status, string userProjectNo, string userPoNumber, string loginUser)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemSlipMaster>(SP_MES_ITEM_SLIP_MASTER,
                    new string[] { "@DIV", "@StartDate", "@EndDate", "@Status", "@UserProjectCode", "@UserPONumber", "@loginUser" },
                    new object[] { "SelectMasterStockInPOPartner", startDate, endDate, status, userProjectNo, userPoNumber, loginUser }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public Result SavePartnerPODelivery(string flag, MES_ItemSlipMaster itemSlipMaster, List<MES_ItemSlipDetail> itemSlipDetail, string userModify)
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
                                            "@WHFromCode", "@WHToCode", "@RelNumber", "@UserCreated", "@TotalAmt", "@TaxAmt", "@TotalTaxAmt", "@Remark", "@InventoryClosed" ,"@CheckedGoodsReturn","@PartnerPlanDeliveryDate","@PartnerDeliveryDate"},
                                new object[] { "SavePartnerPODelivery", itemSlipMaster.SlipNumber, itemSlipMaster.SlipYMD, itemSlipMaster.SlipType, itemSlipMaster.PartnerCode,
                                            itemSlipMaster.WHFromCode, itemSlipMaster.WHToCode, itemSlipMaster.RelNumber, userModify, TotalAmt, TaxAmt,
                                            TotalTaxAmt, itemSlipMaster.Remark, false ,0,itemSlipMaster.PartnerPlanDeliveryDate,itemSlipMaster.PartnerDeliveryDate},
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
                                var resultUpdateStatus = 0;
                                
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

                                resultUpdateStatus = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                  new string[] { "@DIV", "@PONumber" },
                                  new object[] { "UpdateStatusWhenGoodsRecieptConfirmedAll", itemSlipMaster.RelNumber },
                                  transaction);

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
        public List<MES_SaleProject> getDetailByProjectCodeList(string listProjectCode)
        {
            object routes_list = JsonConvert.DeserializeObject(listProjectCode);
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@listProjectCode";
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "getDetailByProjectCodeList";
                arrParamsValue[1] = listProjectCode;
                var data = conn.ExecuteQuery<MES_SaleProject>(
                    SP_MES_ITEM_SLIP_MASTER_DELIVERY, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MES_ItemSlipMaster> getCreatedPO(string userId)
        {
            var result = new List<MES_ItemSlipMaster>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@UserCode";
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "getCreatedPO";
                arrParamsValue[1] = userId;
                var data = conn.ExecuteQuery<MES_ItemSlipMaster>(
                    SP_MES_ITEM_SLIP_MASTER, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
        public Result PONotified(string PONumber)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "PONotified";
                arrParamsValue[1] = PONumber;
                var rs = conn.ExecuteNonQuery(
                    SP_MES_ITEM_SLIP_MASTER, arrParams, arrParamsValue);
                if(rs == 1)
                {
                    result.Success = true;
                }
                else
                {
                    result.Message = MessageCode.MD00017;
                }
            }
            return result;
        }
    }
}

using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Extensions;
using InfrastructureCore.Helpers;
using InfrastructureCore.Utils;
using Microsoft.AspNetCore.Http;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Inventory.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Modules.Pleiger.Inventory.Services.ServiceImp
{
    public class MESInventoryService : IMESInventoryService
    {
        private const string SP_MES_TRANS_CLOSING_MASTER = "SP_MES_TRANS_CLOSING_MASTER";
        private const string SP_MES_TRANS_CLOSING_DETAIL = "SP_MES_TRANS_CLOSING_DETAIL";
        private const string SP_MES_TRANS_CLOSING_ITEMS = "SP_MES_TRANS_CLOSING_ITEMS";

        private const string SP_MES_ITEM_SLIP_MASTER = "SP_MES_ITEM_SLIP_MASTER";
        private const string SP_MES_ITEM_SLIP_DETAIL = "SP_MES_ITEM_SLIP_DETAIL";

        private const string SP_MES_INVENTORY_CURRENT_STOCK = "SP_MES_INVENTORY_CURRENT_STOCK";
        private const string EXCEL_EXPORT_TEMPLATE_PATH = @"excelTemplate/InventoryCheckTemplate.xlsx";
        private const string EXCEL_WORKSHEETS_NAME = "InventoryCheck";
        private const string EXCEL_DATE_FORMAT = "yyyy-MM-dd HH:mm:sss";
        private const string EXCEL_EXPORT_FOLDER = @"RenderExcel/";
        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "yyyyMMddhhmmss";

        #region "Trans Closing Mst"
        public List<MES_TransClosingMst> GetTransClosingMst(string startDate, string endDate)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingMst>(SP_MES_TRANS_CLOSING_MASTER,
                    new string[] { "@DIV", "@StartDate", "@EndDate" },
                    new object[] { "SelectTransClosingMst", startDate, endDate }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public Result CloseMonth(MES_TransClosingMst data, string userModify)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var update = conn.ExecuteNonQuery(SP_MES_TRANS_CLOSING_MASTER, CommandType.StoredProcedure,
                            new string[] { "@DIV", "@TransMonth", "@POTrans", "@PJTrans", "@ItemSlipTrans", "@InventoryClosedYN", "@Created_By" },
                            new object[] { "InventoryCloseMonth", data.TransMonth, true, true, true, true, userModify },
                            transaction);
                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
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
        
        public Result UnCloseMonth(List<MES_TransClosingMst> data, string userModify)
        {
            Result result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in data)
                        {
                            var update = conn.ExecuteNonQuery(SP_MES_TRANS_CLOSING_MASTER, CommandType.StoredProcedure,
                            new string[] { "@DIV", "@TransMonth", "@POTrans", "@PJTrans", "@ItemSlipTrans", "@InventoryClosedYN", "@Created_By" },
                            new object[] { "InventoryCloseMonth", item.TransMonth, item.POTrans, item.PJTrans, item.ItemSlipTrans, item.InventoryClosedYN, userModify },
                            transaction);
                        }
                        
                        transaction.Commit();
                        result.Success = true;
                        result.Message = MessageCode.MD0004;
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
        #region "Trans Closing detail"
        public List<MES_TransClosingDtls> GetTransClosingDtl(string TransMonth)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingDtls>(SP_MES_TRANS_CLOSING_DETAIL,
                    new string[] { "@DIV", "@TransMonth" },
                    new object[] { "SelectTransClosingDetail", TransMonth }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        #endregion

        #region "Duy TransClosingDetail from Items  TransCloseNo"
        public List<MES_TransClosingDtls> GetTransClosingDtlTransCloseNo(string TransCloseNo)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingDtls>(SP_MES_TRANS_CLOSING_DETAIL,
                    new string[] { "@DIV", "@TransCloseNo" },
                    new object[] { "SelectTransClosingDetailTransNo", TransCloseNo }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        #endregion

        #region "Duy TransClosingMaster from Deatil TransMonth"
        public List<MES_TransClosingMst> GetTransClosingMstTransMonth(string TransMonth)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingMst>(SP_MES_TRANS_CLOSING_MASTER,
                    new string[] { "@DIV", "@TransMonth" },
                    new object[] { "SelectTransClosingMstFrTransMonth", TransMonth }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        #endregion

        #region "Trans Closing items"
        public List<MES_TransClosingItems> GetTransClosingItems(string TransCloseNo)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingItems>(SP_MES_TRANS_CLOSING_ITEMS,
                    new string[] { "@DIV", "@TransCloseNo" },
                    new object[] { "SelectTransClosingItems", TransCloseNo }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public List<MES_Warehouse> GetWareHouseByCategory(string CategoryCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Warehouse>(SP_MES_INVENTORY_CURRENT_STOCK,
                    new string[] { "@DIV", "@Category" },
                    new object[] { "GetWareHouseByCategory", CategoryCode }).ToList();

                return result;
            }
        }
        public List<MES_TransClosingItems> GetTransClosingMstSearch(string startDate, string endDate, string Category, string ItemClass, string ItemCode, string ItemName)
       {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingItems>(SP_MES_TRANS_CLOSING_ITEMS,   
                    new string[] { "@DIV","@startDate", "@endDate", "@Category", "@ItemClass", "@ItemCode", "@ItemName" },
                    new object[] { "GetTransClosingMstSearch", startDate, endDate , Category, ItemClass, ItemCode, ItemName }).ToList();

            int no = 1;
            result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
        #endregion
        #region Inventory check
        public bool IsTransmonthHaveInventoryClosed(string data)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingMst>(SP_MES_TRANS_CLOSING_MASTER,
                    new string[] { "@DIV", "@TransMonth"},
                    new object[] { "IsTransmonthHaveInventoryClosed", data }).FirstOrDefault();

                return result != null && result.InventoryClosedYN ? true : false;
            }
        }

        private bool IsTransmonthHaveData(string data)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_TransClosingMst>(SP_MES_TRANS_CLOSING_MASTER,
                    new string[] { "@DIV", "@TransMonth"},
                    new object[] { "IsTransmonthHaveInventoryClosed", data }).FirstOrDefault();

                return result != null ? true : false;
            }
        }

        public List<MES_InventoryCheckVO> SelectAllItemCurrentStock(string warehouseCode, string category,
            string itemCode, string itemName, DateTime? closeMonth)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_InventoryCheckVO>(SP_MES_INVENTORY_CURRENT_STOCK,
                    new string[] { "@DIV", "@Category", "@ItemCode", "@ItemName", "@WarehouseCode", "@CloseMonth" },
                    new object[] { "SelectAllItemCurrentStock", category, itemCode, itemName, warehouseCode, closeMonth }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }
         public List<MES_InventoryCheckVO> GetInventoryCurrentStock(string warehouseCode, string category,
            string itemCode, string itemName, DateTime? closeMonth)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_InventoryCheckVO>(SP_MES_INVENTORY_CURRENT_STOCK,
                    new string[] { "@DIV", "@Category", "@ItemCode", "@ItemName", "@WarehouseCode", "@CloseMonth" },
                    new object[] { "SelectInventoryCurrentStock", category, itemCode, itemName, warehouseCode, closeMonth }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public List<MES_InventoryCheckVO> GetInventoryCurrentStockNew(string warehouseCode, string category,
            string itemCode, string itemName, DateTime? closeMonth, string Lang)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[7];
                arrParams[0] = "@DIV";
                arrParams[1] = "@Category";
                arrParams[2] = "@ItemCode";
                arrParams[3] = "@ItemName";
                arrParams[4] = "@WarehouseCode";
                arrParams[5] = "@CloseMonth";
                arrParams[6] = "@Lang";
                object[] arrParamsValue = new object[7];
                arrParamsValue[0] = "SelectInventoryCurrentStockNew";
                arrParamsValue[1] = category;
                arrParamsValue[2] = itemCode;
                arrParamsValue[3] = itemName;
                arrParamsValue[4] = warehouseCode;
                arrParamsValue[5] = closeMonth;
                arrParamsValue[6] = Lang;
                var result = conn.ExecuteQuery<MES_InventoryCheckVO>(SP_MES_INVENTORY_CURRENT_STOCK,
                    arrParams, arrParamsValue).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                });

                return result;
            }
        }

        public List<MES_InventoryCheckExcelVO> GetItemInventoryByWHCodeAndItemCode(string jsonObj)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@ListItemWH";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetItemInventoryByWHCodeAndItemCode";
                arrParamsValue[1] = jsonObj;
                var result = conn.ExecuteQuery<MES_InventoryCheckExcelVO>(SP_MES_INVENTORY_CURRENT_STOCK,
                    arrParams, arrParamsValue);

                //int no = 1;
                //result.ForEach(x =>
                //{
                //    x.No = no++;
                //});

                return result.ToList();
            }
        }



        public List<MES_InventoryCheckVO> GetInventoryCurrentStockDownload(string warehouseCode, string category,
            string itemCode, string itemName)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_InventoryCheckVO>(SP_MES_INVENTORY_CURRENT_STOCK,
                    new string[] { "@DIV", "@Category", "@ItemCode", "@ItemName", "@WarehouseCode" },
                    new object[] { "SelectInventoryCurrentStock", category, itemCode, itemName, warehouseCode }).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.No = no++;
                    x.StockDate = DateTime.Now;//.ToString("yyyy-MM-dd");
                });

                return result;
            }
        }

        public string ExportInventoryExcelFile(DataTable dt)
        {
            var log = new LogWriter("DownloadFileInventoryCurrentStock");
            log.LogWrite("DownloadFileInventoryCurrentStock Start");
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            string excelFileName = $"{EXCEL_WORKSHEETS_NAME}{DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT)}.xlsx";
            FileInfo newExcelFile;
            FileInfo excelTemplate = new FileInfo(EXCEL_EXPORT_TEMPLATE_PATH);

            string curDate = DateTime.Today.ToString("yyyyMM");
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", curDate);
            var tempFilePath = Path.Combine(tempPath, EXCEL_EXPORT_FOLDER);
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);

            log.LogWrite("tempFilePath tempFilePath : " + tempFilePath);
            using (var excel = new ExcelPackage(excelTemplate))
            {
                try
                {
                    ExcelWorksheet templateSheet = excel.Workbook.Worksheets[EXCEL_WORKSHEETS_NAME];

                    templateSheet.Cells.LoadFromDataTable(dt, true);

                    if (dt.Rows.Count != 0)
                    {

                        int colNumber = 1;

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.DataType == typeof(DateTime))
                            {
                                //workSheet.Column(colNumber).Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss AM/PM";
                                templateSheet.Column(colNumber).Style.Numberformat.Format = EXCEL_DATE_FORMAT;
                            }
                            if (/*col.DataType == typeof(Int64) ||*/ col.DataType == typeof(Double) || col.DataType == typeof(Decimal)/* || col.DataType == typeof(Int32)*/)
                            {
                                templateSheet.Column(colNumber).Style.Numberformat.Format = "#,###";
                            }
                            colNumber++;
                        }
                        

                        templateSheet.Cells[1, 1, dt.Rows.Count + 1, dt.Columns.Count].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        templateSheet.Cells[1, 1, dt.Rows.Count + 1, dt.Columns.Count].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        templateSheet.Cells[1, 1, dt.Rows.Count + 1, dt.Columns.Count].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        templateSheet.Cells[1, 1, dt.Rows.Count + 1, dt.Columns.Count].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        templateSheet.Cells[1, 1, dt.Rows.Count + 1, dt.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        //templateSheet.Row(1).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        templateSheet.Cells[1, 1, 1, dt.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        templateSheet.Cells[1, 1, 1, dt.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        templateSheet.Cells[1, 1, 1, dt.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                        //templateSheet.Cells.AutoFitColumns();
                    }

                    newExcelFile = new FileInfo(tempFilePath + excelFileName);
                    excel.SaveAs(newExcelFile);
                    log.LogWrite("tempFilePath + excelFileName : " + tempFilePath + excelFileName);
                }
                catch (Exception ex)
                {
                    log.LogWrite("tempFilePath + excelFileName : ex" + ex.ToString());
                    throw;
                }
            }
            return tempFilePath + excelFileName;
        }

        public Result UploadFileInventoryCurrentStock(IFormFile myFile, string chunkMetadata, Type type, string Lang)
        {
            Result result = new Result();
            var tempPath = "";
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

                var filename = metaDataObject.FileGuid + metaDataObject.FileName;

                ExcelHelperTest excelHelperTest = new ExcelHelperTest();
                // Get data from excecl to DataTable
                var dt = excelHelperTest.ReadFromExcelfile(tempPath + "//" + filename, "InventoryCheck", type);
                // Add DataTable to Dataset
                DataSet dts = new DataSet();
                dts.Tables.Add(dt);
                // Convert DataSetType to ModelType
                //var dataFromFile = dts.parseCellDataByTypefromExcel<MES_InventoryCheckVO>();// data from file upload
                var dataFromFile = dts.parseCellDataByTypefromExcel<MES_InventoryCheckExcelVO>();// data from file upload

                bool IsDuplicate = false;

                foreach (var item in dataFromFile)
                {
                    var count = dataFromFile.Where(m => m.ItemCode == item.ItemCode && m.WHCode == item.WHCode && m.CategoryCode == item.CategoryCode).Count();

                    if(count >= 2)
                    {
                        IsDuplicate = true;
                    }    
                }

                if (IsDuplicate)
                {
                    result.Success = false;

                    if(Lang == "en")
                    {
                        result.Message = MessageCode.MD00013_EN;
                    }   
                    else
                    {
                        result.Message = MessageCode.MD00013_KR;
                    }    
                    
                    result.Data = null;
                    return result;
                }

                //Trong khoảng thời gian upload phải tracking dc itemcode nào dc thay doi trong stock, show data detail cho user xác nhận.
                // Get current stock data
                List<MES_InventoryCheckVO> listNewDataDB = SelectAllItemCurrentStock(null, null, null, null, null); // data from DB

                int seq = 1;
                var query = from dtfromFile in dataFromFile
                            join dtfromDB in listNewDataDB
                            on new { dtfromFile.WHCode, dtfromFile.CategoryCode, dtfromFile.ItemCode } equals
                            new { dtfromDB.WHCode, dtfromDB.CategoryCode, dtfromDB.ItemCode }
                            where dtfromFile.OfflineCheckQty != dtfromDB.CurrentStockQty
                            select new MES_InventoryCheckVO
                            {
                                No = seq++,
                                WHCode = dtfromDB.WHCode,
                                WHName = dtfromDB.WHName,
                                CategoryCode = dtfromDB.CategoryCode,
                                CategoryName = dtfromDB.CategoryName,
                                ItemCode = dtfromDB.ItemCode,
                                ItemName = dtfromDB.ItemName,
                                StockQtyUploaded = dtfromFile.MESSystemQty,
                                CurrentStockQty = dtfromDB.CurrentStockQty,
                                CheckQty = dtfromFile.OfflineCheckQty,
                                DiffQty = dtfromDB.CurrentStockQty - dtfromFile.OfflineCheckQty,
                                DiffQtyDisplay = dtfromFile.OfflineCheckQty - dtfromDB.CurrentStockQty,
                                Remark = dtfromFile.Remark,
                                StockDate = dtfromFile.StockDate,
                                CheckDate = dtfromFile.CheckDate != null ? dtfromFile.CheckDate : DateTime.Now
                            };

                List<MES_InventoryCheckVO> resultData = query.ToList<MES_InventoryCheckVO>();

                result.Success = true;
                result.Message = MessageCode.MD0004;
                result.Data = resultData;
            }

            return result;
        }

        public Result SaveInventoryCheck(List<MES_InventoryCheckVO> data, string userModify, string detailRemark)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Get current transmonth
                        var curTransMonth = DateTime.Now.ToString("yyyyMM");

                        string TransCloseNoNew = null;
                        var transCloseMonthNew = 0;
                        // check month have data or no

                        // if have data, insert detail
                        if (IsTransmonthHaveData(curTransMonth))
                        {
                            //first row
                            for (int i = 0; i < 1; i++)
                            {
                                // insert data transClosingDtls
                                //TransCloseNoNew = conn.ExecuteScalar<string>(SP_MES_TRANS_CLOSING_DETAIL, CommandType.StoredProcedure,
                                //        new string[] { "@DIV", "@TransCloseNo", "@TransMonth", "@ItemCount", "@TransCloseDate", "@DownloadDate", "@UploadDate",
                                //            "@SearchConds", "@Remark", "@Created_By" },
                                //        new object[] { "SaveDetailTransClosing", "", curTransMonth, data.Count ,"", data[0].StockDate, DateTime.Now,
                                //            "", data[0].Remark, userModify},
                                //        transaction);
                                TransCloseNoNew = conn.ExecuteScalar<string>(SP_MES_TRANS_CLOSING_DETAIL, CommandType.StoredProcedure,
                                        new string[] { "@DIV", "@TransCloseNo", "@TransMonth", "@ItemCount", "@TransCloseDate", "@DownloadDate", "@UploadDate",
                                            "@SearchConds", "@Remark", "@Created_By" },
                                        new object[] { "SaveDetailTransClosing", "", curTransMonth, data.Count ,"", data[0].StockDate, DateTime.Now,
                                            "", detailRemark, userModify},
                                        transaction);
                            }

                            int SeqNo = 1;
                            foreach (var item in data)
                            {

                                if (TransCloseNoNew == null)
                                {
                                    // Save fail
                                    transaction.Rollback();
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                                else
                                {
                                    // insert data TransClosingItems
                                    var transClosingItems = 0;
                                    transClosingItems = conn.ExecuteNonQuery(SP_MES_TRANS_CLOSING_ITEMS, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@TransCloseNo", "@SeqNo", "@Category", "@ItemCode", "@WHCode", "@StockQty",
                                            "@StockDate", "@CheckQty", "@CheckDate", "@Created_By", "@Remark" },
                                            new object[] { "SaveTransClosingItems", TransCloseNoNew, SeqNo++, item.CategoryCode, item.ItemCode ,item.WHCode, item.CurrentStockQty,
                                            item.StockDate, item.CheckQty, item.CheckDate, userModify, item.Remark},
                                            transaction);
                                }
                            }
                        }
                        // if dont have data, insert master
                        else
                        {
                            transCloseMonthNew = conn.ExecuteNonQuery(SP_MES_TRANS_CLOSING_MASTER, CommandType.StoredProcedure,
                                new string[] { "@DIV", "@TransMonth", "@POTrans", "@PJTrans", "@ItemSlipTrans", "@InventoryClosedYN", "@Created_By" },
                                new object[] { "SaveMasterTransClosing", curTransMonth, false, false, false, false, userModify },
                                transaction);
                            if (transCloseMonthNew == 0)
                            {
                                transaction.Rollback();
                                // Save fail
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                            else
                            {
                                //first row
                                for (int i = 0; i < 1; i++)
                                {
                                    // insert data transClosingDtls
                                    //TransCloseNoNew = conn.ExecuteScalar<string>(SP_MES_TRANS_CLOSING_DETAIL, CommandType.StoredProcedure,
                                    //        new string[] { "@DIV", "@TransCloseNo", "@TransMonth", "@ItemCount", "@TransCloseDate", "@DownloadDate", "@UploadDate",
                                    //        "@SearchConds", "@Remark", "@Created_By" },
                                    //        new object[] { "SaveDetailTransClosing", "", curTransMonth, data.Count ,"", data[0].StockDate, DateTime.Now,
                                    //        "", data[0].Remark, userModify},
                                    //        transaction);

                                    TransCloseNoNew = conn.ExecuteScalar<string>(SP_MES_TRANS_CLOSING_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@TransCloseNo", "@TransMonth", "@ItemCount", "@TransCloseDate", "@DownloadDate", "@UploadDate",
                                            "@SearchConds", "@Remark", "@Created_By" },
                                            new object[] { "SaveDetailTransClosing", "", curTransMonth, data.Count ,"", data[0].StockDate, DateTime.Now,
                                            "", detailRemark, userModify},
                                            transaction);
                                }

                                int SeqNo = 1;
                                foreach (var item in data)
                                {

                                    if (TransCloseNoNew == null)
                                    {
                                        // Save fail
                                        transaction.Rollback();
                                        return new Result
                                        {
                                            Success = false,
                                            Message = MessageCode.MD0005
                                        };
                                    }
                                    else
                                    {
                                        // insert data TransClosingItems
                                        var transClosingItems = 0;
                                        transClosingItems = conn.ExecuteNonQuery(SP_MES_TRANS_CLOSING_ITEMS, CommandType.StoredProcedure,
                                                new string[] { "@DIV", "@TransCloseNo", "@SeqNo", "@Category", "@ItemCode", "@WHCode", "@StockQty",
                                            "@StockDate", "@CheckQty", "@CheckDate", "@Created_By", "@Remark" },
                                                new object[] { "SaveTransClosingItems", TransCloseNoNew, SeqNo++, item.CategoryCode, item.ItemCode ,item.WHCode, item.CurrentStockQty,
                                            item.StockDate, item.CheckQty, item.CheckDate, userModify, item.Remark},
                                                transaction);
                                    }
                                }
                            }
                        }
                        
                        // insert slip data
                        foreach (var item in data)
                        {
                            // check Diff Qty > 0, SlipType=4	out	 - stock
                            // exam: Diff.= 10, tức > 0, tức là lost mất hàng, SlipType=4 là trừ stock đi 10
                            if (item.DiffQty > 0)// 20201102 : minh modify item.DiffQty > 0 => item.DiffQty < 0
                            {
                                // return a SlipNumberNew in DB
                                // RelNumber is is null// need 
                                var SlipNumberNew = conn.ExecuteScalar<string>(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                    new string[] { "@DIV", "@SlipNumber", "@SlipType", "@WHFromCode", "@RelNumber", "@UserCreated", "@Remark", "@InventoryClosed", "@Created_By" },
                                    new object[] { "SaveMasterItemSlip", "", "4", item.WHCode, TransCloseNoNew, userModify, item.Remark, true, userModify },
                                    transaction);

                                if (SlipNumberNew == null)
                                {
                                    // Save fail
                                    transaction.Rollback();
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                                else
                                {
                                    // Save detail
                                    // RelNumber is null// need 
                                    var resultInsDtl = 0;
                                    int seq = 1;
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty", "@Remark", "@RelNumber", "@RelSeq", "@Created_By" },
                                            new object[] { "SaveDetailItemSlip", SlipNumberNew, seq++, item.ItemCode, "", item.DiffQty, item.Remark, TransCloseNoNew, 1, userModify },
                                            transaction);
                                }
                            }
                            // check Diff Qty < 0 SlipType=0	in	+ stock
                            // exam: Diff.= -1, tức < 0, tức là thiếu mất hàng, SlipType=0 là cộng stock  0
                            else if (item.DiffQty < 0)// 20201102 : minh modify item.DiffQty < 0 => item.DiffQty > 0
                            {
                                // return a SlipNumberNew in DB
                                // RelNumber is is null// need 
                                var SlipNumberNew = conn.ExecuteScalar<string>(SP_MES_ITEM_SLIP_MASTER, CommandType.StoredProcedure,
                                    new string[] { "@DIV", "@SlipNumber", "@SlipType", "@WHToCode", "@RelNumber", "@UserCreated", "@Remark", "@InventoryClosed", "@Created_By" },
                                    new object[] { "SaveMasterItemSlip", "", "0", item.WHCode, TransCloseNoNew, userModify, item.Remark, true, userModify },
                                    transaction);

                                if (SlipNumberNew == null)
                                {
                                    // Save fail
                                    transaction.Rollback();
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD0005
                                    };
                                }
                                else
                                {
                                    // Save detail
                                    // RelNumber is null// need 
                                    var resultInsDtl = 0;
                                    int seq = 1;
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_ITEM_SLIP_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@DIV", "@SlipNumber", "@Seq", "@ItemCode", "@Unit", "@Qty", "@Remark", "@RelNumber", "@RelSeq", "@Created_By" },
                                            new object[] { "SaveDetailItemSlip", SlipNumberNew, seq++, item.ItemCode, "", item.DiffQty, item.Remark, TransCloseNoNew, 1, userModify },
                                            transaction);
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



        #endregion
    }
}

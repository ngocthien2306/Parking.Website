using Modules.Pleiger.Services.IService;
using InfrastructureCore;
using Modules.Pleiger.Models;
using System;
using System.Collections.Generic;
using InfrastructureCore.DAL;
using System.Linq;
using System.Text.RegularExpressions;
using Modules.Pleiger.Utils;
using System.Data;
using InfrastructureCore.Utils;
using OfficeOpenXml;
using System.IO;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESWHInventoryStatusService : IMESWHInventoryStatusService
    {
        private const string SP_MES_WAREHOUSE_INVENTORY_STATUS = "SP_MES_WAREHOUSE_INVENTORY_STATUS";
        private const string EXCEL_EXPORT_TEMPLATE_PATH = @"excelTemplate/WarehouseInventory.xlsx";
        private const string EXCEL_WORKSHEETS_NAME = "WarehouseInventory";
        private const string EXCEL_DATE_FORMAT = "yyyy-MM-dd HH:mm:sss";
        private const string EXCEL_EXPORT_FOLDER = @"RenderExcel/";
        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "yyyyMMddhhmmss";

        #region "Master"
        public List<MES_WHInventoryStatus> GetAllWHInventory(MES_WHInventoryStatus model, int pageSize, int itemSkip)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                if (!String.IsNullOrEmpty(model.ItemName))
                {
                    model.ItemName = Regex.Replace(model.ItemName, PleigerConstant.REGEX_REPLACE_DATA_SEARCH, "_");
                }
                var result = conn.ExecuteQuery<MES_WHInventoryStatus>(SP_MES_WAREHOUSE_INVENTORY_STATUS,
                    new string[] { "@DIV" , "@P_WarehouseCode", "@P_Category", "@P_ItemCode", "@P_ItemName", "@PageNumber", "@PageSize", "@P_WarehouseName" },
                    new object[] { "SelectMaster", model.WarehouseCode, model.Category, model.ItemCode, model.ItemName, itemSkip, pageSize, model.WarehouseName }).ToList();

                int no = itemSkip +1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        
        public int CountAllWHInventory(MES_WHInventoryStatus model)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                if (!String.IsNullOrEmpty(model.ItemName))
                {
                    model.ItemName = Regex.Replace(model.ItemName, PleigerConstant.REGEX_REPLACE_DATA_SEARCH, "_");
                }
                var result = conn.ExecuteScalar<int>(SP_MES_WAREHOUSE_INVENTORY_STATUS,
                    new string[] { "@DIV" , "@P_WarehouseCode", "@P_Category", "@P_ItemCode", "@P_ItemName", "@P_WarehouseName" },
                    new object[] { "Count", model.WarehouseCode, model.Category, model.ItemCode, model.ItemName, model.WarehouseName });
                return result;
            }
        }

        public List<MES_WHInventoryStatus> GetAllWHInventoryToExport(string WarehouseCode, string Category, string ItemName)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_WHInventoryStatus>(SP_MES_WAREHOUSE_INVENTORY_STATUS,
                    new string[] { "@DIV", "@P_WarehouseCode", "@P_Category", "@P_ItemName" },
                    new object[] { "SelectAllMaster",WarehouseCode,Category!= null ?  Category : null,ItemName}).ToList();

                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        #endregion
        //bao add

        #region Export excel
        public string ExportWHInventoryExcelFile(DataTable dt)
        {
            var log = new LogWriter("ExportWHInventoryExcelFile");
            log.LogWrite("ExportWHInventoryExcelFile Start");
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

                    //ExcelWorksheet newSheet = excel.Workbook.Worksheets.Add("Inventory", templateSheet);

                    //excel.Workbook.Worksheets.Delete(excel.Workbook.Worksheets.Where(m => m.Name == "InventoryCheck").FirstOrDefault());

                    templateSheet.Cells.LoadFromDataTable(dt, true);

                    if (dt.Rows.Count != 0)
                    {
                        var modelCells = templateSheet.Cells["A1"];
                        int modelRows = dt.Rows.Count + 1;
                        string modelRange = $"A1:G{modelRows}";
                        var modelTable = templateSheet.Cells[modelRange];

                        // Apply style to excel sheet
                        modelTable.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        modelTable.Style.Font.Bold = false;
                        modelTable.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    }
                    // Count the colums of data table for applying style


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
        #endregion

    }
}

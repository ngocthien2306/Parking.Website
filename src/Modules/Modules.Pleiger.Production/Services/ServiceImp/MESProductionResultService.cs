using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Utils;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Model;
using Modules.Pleiger.Production.Services.IService;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    class MESProductionResultService : IMESProductionResultService
    {
        private readonly string SP_MES_PRODUCTION_RESULT = "SP_MES_PRODUCTION_RESULT";
        private readonly string SP_MES_PRODUCTION_RESULT_EXPORT_EXCEL = "SP_MES_PRODUCTION_RESULT_EXPORT_EXCEL";
        private const string EXCEL_WORKSHEETS_NAME = "PRODUCTION_RESULT";
        private const string EXCEL_DATE_FORMAT = "yyyy-MM-dd HH:mm:sss";
        private const string EXCEL_EXPORT_FOLDER = @"RenderExcel/";
        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "yyyyMMddhhmmss";
        private const string EXCEL_EXPORT_TEMPLATE_PATH = @"excelTemplate/ProductionResultExportTemplate.xlsx";

        public List<MES_SaleProject> searchProductionResult(
            string userProjectCode,
            string productionCode,
            string itemCode,
            string itemName,
            string projectStatus,
            string salesClasification,
            string projectOrderType,
            string salesOrderProjectName,
            string salesOrderProjectCode)
        {
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[10];
                arrParams[0] = "@Method";
                arrParams[1] = "@UserProjectCode";
                arrParams[2] = "@ProductionCode";
                arrParams[3] = "@ItemCode";
                arrParams[4] = "@ItemName";
                arrParams[5] = "@ProjectStatus";
                arrParams[6] = "@SalesClassification";
                arrParams[7] = "@ProjectOrderType";
                arrParams[8] = "@SaleOrderProjectName";
                arrParams[9] = "@SaleOrderProjectCode";
                
                object[] arrParamsValue = new string[10];
                arrParamsValue[0] = "SearchProjectProductionResult";
                arrParamsValue[1] = userProjectCode;
                arrParamsValue[2] = productionCode;
                arrParamsValue[3] = itemCode;
                arrParamsValue[4] = itemName;
                arrParamsValue[5] = projectStatus;
                arrParamsValue[6] = salesClasification;
                arrParamsValue[7] = projectOrderType;
                arrParamsValue[8] = salesOrderProjectName;
                arrParamsValue[9] = salesOrderProjectCode;
                var data = conn.ExecuteQuery<MES_SaleProject>(
                    SP_MES_PRODUCTION_RESULT, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MES_ProjectProdcnLines> getProductionLineResult(string projectCode, string productionCode)
        {
            var result = new List<MES_ProjectProdcnLines>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@ProductionCode";
                object[] arrParamsValue = new string[3];
                arrParamsValue[0] = "GetProductionLineResult";
                arrParamsValue[1] = projectCode;
                arrParamsValue[2] = productionCode;
                var data = conn.ExecuteQuery<MES_ProjectProdcnLines>(
                    SP_MES_PRODUCTION_RESULT, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
        public List<MES_ProjectProdcnLines> getProductionLineResultDetail(string projectCode, string GroupLine)
        {
            var result = new List<MES_ProjectProdcnLines>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@GroupLine";
                object[] arrParamsValue = new string[3];
                arrParamsValue[0] = "getProductionLineResultDetail";
                arrParamsValue[1] = projectCode;
                arrParamsValue[2] = GroupLine;
                var data = conn.ExecuteQuery<MES_ProjectProdcnLines>(
                    SP_MES_PRODUCTION_RESULT, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
        
        public List<MESWorkHistory> getWorkHistory(string projectCode, string productionCode, string prodcnLineCode)
        {
            var result = new List<MESWorkHistory>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@ProductionCode";
                arrParams[3] = "@ProdcnLineCode";
                object[] arrParamsValue = new string[4];
                arrParamsValue[0] = "GetWorkHistory";
                arrParamsValue[1] = projectCode;
                arrParamsValue[2] = productionCode;
                arrParamsValue[3] = prodcnLineCode;
                var data = conn.ExecuteQuery<MESWorkHistory>(
                    SP_MES_PRODUCTION_RESULT, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public List<MESProductDelivery> getDeliveryInformation(string projectCode)
        {
            var result = new List<MESProductDelivery>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "GetDeliveryInformation";
                arrParamsValue[1] = projectCode;
                var data = conn.ExecuteQuery<MESProductDelivery>(
                    SP_MES_PRODUCTION_RESULT, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        #region "Export Excel" 
        public List<MESProductionResultExportTemplate> getExportExcelData(
            string userProjectCode,
            string productionCode,
            string itemCode,
            string itemName,
            string projectStatus,
            string salesClasification,
            string projectOrderType,
            string salesOrderProjectName,
             string salesOrderProjectCode)
        {
            var result = new List<MESProductionResultExportTemplate>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[9];
                arrParams[0] = "@UserProjectCode";
                arrParams[1] = "@ProductionCode";
                arrParams[2] = "@ItemCode";
                arrParams[3] = "@ItemName";
                arrParams[4] = "@ProjectStatus";
                arrParams[5] = "@SalesClassification";
                arrParams[6] = "@ProjectOrderType";
                arrParams[7] = "@SaleOrderProjectName";
                arrParams[8] = "@SaleOrderProjectCode";
                object[] arrParamsValue = new string[9];
                arrParamsValue[0] = userProjectCode;
                arrParamsValue[1] = productionCode;
                arrParamsValue[2] = itemCode;
                arrParamsValue[3] = itemName;
                arrParamsValue[4] = projectStatus;
                arrParamsValue[5] = salesClasification;
                arrParamsValue[6] = projectOrderType;
                arrParamsValue[7] = salesOrderProjectName;
                arrParamsValue[8] = salesOrderProjectCode;
                
                var data = conn.ExecuteQuery<MESProductionResultExportTemplate>(
                    SP_MES_PRODUCTION_RESULT_EXPORT_EXCEL, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public string ExportItemPartnerExcelFile(DataTable dt)
        {
            var log = new LogWriter("ExportItemPartnerExcelFile");
            log.LogWrite("ExportItemPartnerExcelFile Start");
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

                    int colNumber = 1;

                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.DataType == typeof(DateTime))
                        {
                            //workSheet.Column(colNumber).Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss AM/PM";
                            templateSheet.Column(colNumber).Style.Numberformat.Format = EXCEL_DATE_FORMAT;
                            templateSheet.Column(colNumber).Width = 20;
                        }
                        else if (col.DataType == typeof(Int32) || col.DataType == typeof(Double) || col.DataType == typeof(Decimal)/* || col.DataType == typeof(Int32)*/)
                        {
                            templateSheet.Column(colNumber).Style.Numberformat.Format = "#,##0";
                        }
                        else if (col.DataType == typeof(String))
                        {
                            templateSheet.Column(colNumber).AutoFit();
                        }
                        colNumber++;
                    }

                    templateSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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

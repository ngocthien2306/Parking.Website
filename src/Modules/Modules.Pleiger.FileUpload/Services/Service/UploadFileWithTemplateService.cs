using InfrastructureCore;
using InfrastructureCore.Configuration;
using InfrastructureCore.DAL;
using InfrastructureCore.DataAccess;
using InfrastructureCore.Helpers;
using InfrastructureCore.Utils;
using Microsoft.AspNetCore.Http;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.FileUpload.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Modules.Pleiger.FileUpload.Services.Service
{
    public class UploadFileWithTemplateService : IUploadFileWithTemplateService
    {
        #region Properties
        IDBContextConnection dbConnection;
        IDbConnection conn;
        #endregion


        public UploadFileWithTemplateService(IDBContextConnection dbConnection)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            this.dbConnection = dbConnection;
            conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);
        }

        public void ImportDataToTemplateExcelFile(string pageName, string filePath)
        {
            FileInfo excelTemplate = new FileInfo(filePath);
            switch (pageName)
            {
                case "ITEMS_MGT":
                    ItemMgtTemplateExcelFile(excelTemplate);
                    break;
                case "ITEM_PARTNER_INFO":
                    ItemPartnerTemplateExcelFile(excelTemplate);
                    break;
                case "SALE_PROJECT":
                    SaleProjectTemplateExcelFile(excelTemplate);
                    break;
                case "SALE_ORDER_PROJECT":
                    SaleOrderProjectTemplate(excelTemplate);
                    break;
                case "ITEM_MANAGEMENT":
                    ItemManagementTemplate(excelTemplate);
                    break;
                default: break;
            }
        }

        private void ItemMgtTemplateExcelFile(FileInfo pathFile)
        {
            using (var excel = new ExcelPackage(pathFile))
            {
                var category = GetComCodeDtls().Where(m => m.GROUP_CD == "IMTP00").ToList();
                var itemClassCode = GetItemClass();
                var monetaryUnit = GetComCodeDtls().Where(m => m.GROUP_CD == "MOUT00").ToList();
                var unit = GetComCodeDtls().Where(m => m.GROUP_CD == "ITUN00").ToList();
                var leadTimeType = GetComCodeDtls().Where(m => m.GROUP_CD == "LDTM00").ToList();
                var inspectionMethod = GetComCodeDtls().Where(m => m.GROUP_CD == "ICPM00").ToList();
                var partner = GetItemPartner();
                var warehouse = GetWarehouse();
                int row = 3;

                ExcelWorksheet templateSheet = excel.Workbook.Worksheets["Template"];
                ExcelWorksheet commonDataSheet = excel.Workbook.Worksheets["CommonDataSheet"];

                //Remove dropdown data from cell A2 to cell M2
                var AllDataValidation = templateSheet.DataValidations["A2:M2"];
                if (AllDataValidation != null)
                {
                    foreach (var item in templateSheet.DataValidations)
                    {
                        templateSheet.DataValidations.Remove(item);
                    }
                }

                #region ADD DATA TO CommonDataSheet
                //clear all data in excel file from cell P5 
                commonDataSheet.Cells[$"A3:W{row + partner.Count - 1}"].Clear();

                InsertCommonDataToExcel("A2", category, row, 1, 2, templateSheet, "B", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("B2", itemClassCode, row, 4, 5, templateSheet, "E", "ItemClassCode", "ClassNameKor", commonDataSheet);
                InsertCommonDataToExcel("E2", partner, row, 7, 8, templateSheet, "H", "PartnerCode", "PartnerName", commonDataSheet);
                InsertCommonDataToExcel("G2", monetaryUnit, row, 10, 11, templateSheet, "K", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("M2", unit, row, 13, 14, templateSheet, "N", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("H2", leadTimeType, row, 16, 17, templateSheet, "Q", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("J2", inspectionMethod, row, 19, 20, templateSheet, "T", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("K2", warehouse, row, 22, 23, templateSheet, "W", "WarehouseCode", "WarehouseName", commonDataSheet);

                commonDataSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

                #endregion

                excel.Save();
            }
        }

        private void ItemPartnerTemplateExcelFile(FileInfo pathFile)
        {
            using (var excel = new ExcelPackage(pathFile))
            {
                var partner = GetPartnerNew();
                var leadTimeType = GetComCodeDtls().Where(m => m.GROUP_CD == "LDTM00").ToList();
                var monetaryUnit = GetComCodeDtls().Where(m => m.GROUP_CD == "MOUT00").ToList();
                var itemCode = GetItems();
                int row = 3;

                ExcelWorksheet templateSheet = excel.Workbook.Worksheets["Template"];
                ExcelWorksheet commonDataSheet = excel.Workbook.Worksheets["CommonDataSheet"];

                //Remove dropdown data from cell A2 to cell M2
                var AllDataValidation = templateSheet.DataValidations["A2:M2"];
                if (AllDataValidation != null)
                {
                    foreach (var item in templateSheet.DataValidations)
                    {
                        templateSheet.DataValidations.Remove(item);
                    }
                }

                #region ADD DATA TO CommonDataSheet
                //clear all data in excel file from cell P5 
                commonDataSheet.Cells[$"A3:W{row + partner.Count - 1}"].Clear();

                InsertCommonDataToExcel("A2", itemCode, row, 1, 2, templateSheet, "B", "ItemCode", "ItemName", commonDataSheet);
                InsertCommonDataToExcel("B2", partner, row, 7, 8, templateSheet, "H", "PartnerCode", "PartnerName", commonDataSheet);
                InsertCommonDataToExcel("C2", leadTimeType, row, 16, 17, templateSheet, "Q", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("E2", monetaryUnit, row, 10, 11, templateSheet, "K", "BASE_CODE", "BASE_NAME", commonDataSheet);

                commonDataSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

                #endregion

                excel.Save();
            }
        }

        private void ItemManagementTemplate(FileInfo pathFile)
        {
            using (var excel = new ExcelPackage(pathFile))
            {
                var ProcessingClassification = GetComCodeDtls().Where(grCd => grCd.GROUP_CD == "PRCL00").ToList();
                var Unit = GetComCodeDtls().Where(grCd => grCd.GROUP_CD == "ITUN00").ToList();
                var InspectionMethod = GetComCodeDtls().Where(grCd => grCd.GROUP_CD == "ICPM00").ToList();
                var UseYN = GetComCodeDtls().Where(grCd => grCd.GROUP_CD == "USE000").ToList();

                var ItemClass = GetItemClassItemGrid();

                var customer = GetPartnerNew();

                int row = 3;

                ExcelWorksheet templateSheet = excel.Workbook.Worksheets["ItemInfo"];
                ExcelWorksheet commonDataSheet = excel.Workbook.Worksheets["CommonDataSheet"];
                var AllDataValidation = templateSheet.DataValidations["A2:I2"];
                if (AllDataValidation != null)
                {
                    foreach (var item in templateSheet.DataValidations)
                    {
                        templateSheet.DataValidations.Remove(item);
                    }
                }
                commonDataSheet.Cells[$"A3:I{row + 9}"].Clear();
                commonDataSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
                for (int i = 0; i < 29; i++)
                {
                    if (i > 1)
                    {
                        InsertCommonDataToExcel("A" + i.ToString(), ItemClass, row, 7, 8, templateSheet, "G", "ID", "Name", commonDataSheet);
                        InsertCommonDataToExcel("B" + i.ToString(), ProcessingClassification, row, 4, 5, templateSheet, "E", "BASE_CODE", "BASE_NAME", commonDataSheet);
                        InsertCommonDataToExcel("H" + i.ToString(), Unit, row, 1, 2, templateSheet, "B", "BASE_CODE", "BASE_NAME", commonDataSheet);
                        InsertCommonDataToExcel("I" + i.ToString(), InspectionMethod, row, 10, 11, templateSheet, "K", "BASE_CODE", "BASE_NAME", commonDataSheet);
                        ValidateInteger("F" + i.ToString(), templateSheet);

                    }
                }
                excel.Save();

            }
        }

        private void SaleOrderProjectTemplate(FileInfo pathFile)
        {
            using (var excel = new ExcelPackage(pathFile))
            {
                var orderProjectType = GetComCodeDtls().Where(m => m.GROUP_CD == "POT000").ToList();
                var orderTeamCode = GetComCodeDtls().Where(m => m.GROUP_CD == "ORG000").ToList();
                var customer = GetPartnerNew();
                int row = 3;

                ExcelWorksheet templateSheet = excel.Workbook.Worksheets["Template"];
                ExcelWorksheet commonDataSheet = excel.Workbook.Worksheets["CommonDataSheet"];

                var AllDataValidation = templateSheet.DataValidations["A2:H2"];
                if (AllDataValidation != null)
                {
                    foreach (var item in templateSheet.DataValidations)
                    {
                        templateSheet.DataValidations.Remove(item);
                    }
                }
                commonDataSheet.Cells[$"A3:H{row + 7}"].Clear();
                commonDataSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;
                for (int i = 0; i < 29; i++)
                {
                    if (i > 1)
                    {
                        InsertCommonDataToExcel("A" + i.ToString(), orderProjectType, row, 4, 5, templateSheet, "E", "BASE_CODE", "BASE_NAME", commonDataSheet);
                        InsertCommonDataToExcel("E" + i.ToString(), orderTeamCode, row, 19, 20, templateSheet, "T", "BASE_CODE", "BASE_NAME", commonDataSheet);
                        InsertCommonDataToExcel("F" + i.ToString(), customer, row, 7, 8, templateSheet, "H", "PartnerCode", "PartnerName", commonDataSheet);
                        ValidateStringLength("C" + i.ToString(), templateSheet, 200);
                        ValidateStringLength("D" + i.ToString(), templateSheet, 128);
                        ValidateStringLength("G" + i.ToString(), templateSheet, 100);
                        ValidateStringLength("H" + i.ToString(), templateSheet, 150);
                    }
                }

                excel.Save();
            }
        }

        private void SaleProjectTemplateExcelFile(FileInfo pathFile)
        {
            using (var excel = new ExcelPackage(pathFile))
            {
                var saleOrderProject = GetSaleOrderProjectComCode();
                var customer = GetPartnerNew();
                var salesClassification = GetComCodeDtls().Where(m => m.GROUP_CD == "SCS000").ToList();
                var domesticForeign = GetComCodeDtls().Where(m => m.GROUP_CD == "CTTP00").ToList();
                var monetaryUnit = GetComCodeDtls().Where(m => m.GROUP_CD == "MOUT00").ToList();
                var vatType = GetComCodeDtls().Where(m => m.GROUP_CD == "VAT000").ToList();
                var orderTeam = GetComCodeDtls().Where(m => m.GROUP_CD == "ORG000").ToList();
                var finishProducts = GetFinishProducts();

                int row = 3;

                ExcelWorksheet templateSheet = excel.Workbook.Worksheets["Template"];
                ExcelWorksheet commonDataSheet = excel.Workbook.Worksheets["CommonDataSheet"];
                ExcelWorksheet a = excel.Workbook.Worksheets[""];

                //Remove dropdown data from cell A2 to cell M2
                var AllDataValidation = templateSheet.DataValidations["A2:S2"];
                if (AllDataValidation != null)
                {
                    foreach (var item in templateSheet.DataValidations)
                    {
                        templateSheet.DataValidations.Remove(item);
                    }
                }
                #region ADD DATA TO CommonDataSheet
                //clear all data in excel file from cell P5 
                commonDataSheet.Cells[$"A3:X{row + finishProducts.Count - 1}"].Clear();

                InsertCommonDataToExcel("A2", saleOrderProject, row, 22, 23, templateSheet, "W", "ID", "Name", commonDataSheet);
                InsertCommonDataToExcel("B2", salesClassification, row, 4, 5, templateSheet, "E", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("G2", finishProducts, row, 1, 2, templateSheet, "B", "ItemCode", "ItemName", commonDataSheet);
                //InsertCommonDataToExcel("H2", customer, row, 7, 8, templateSheet, "H", "PartnerCode", "PartnerName", commonDataSheet);
                InsertCommonDataToExcel("I2", domesticForeign, row, 13, 14, templateSheet, "N", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("K2", monetaryUnit, row, 10, 11, templateSheet, "K", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("N2", vatType, row, 16, 17, templateSheet, "Q", "BASE_CODE", "BASE_NAME", commonDataSheet);
                InsertCommonDataToExcel("R2", orderTeam, row, 19, 20, templateSheet, "T", "BASE_CODE", "BASE_NAME", commonDataSheet);

                //-----------------------------VALIDATION------------------------------------
                //Date Style yyyy-MM-dd
                ValidateDateTimeCell("F2", templateSheet);//Plan Delivery Date
                ValidateDateTimeCell("Q2", templateSheet); //Exchange Rate Date

                //Integer Style
                //ValidateInteger("H2", templateSheet);
                ValidateInteger("J2", templateSheet);//Order Quantity

                //Decimal Style
                ValidateDecimal("L2", templateSheet);//Order Price
                ValidateDecimal("O2", templateSheet);//Vat Rate
                ValidateDecimal("P2", templateSheet);//Exchange Rate

                //String Length
                ValidateStringLength("C2", templateSheet, 80);//User Project Code
                ValidateStringLength("S2", templateSheet, 128);//ETC
                //-------------------------------------x------------------------------------

                //----------------------------SET DEFAULT VALUE-----------------------------
                //SetDefaultValueToCell("H2", templateSheet, typeof(int), "0");
                SetDefaultValueToCell("L2", templateSheet, typeof(int), "0");//Order Price
                SetDefaultValueToCell("M2", templateSheet, typeof(int), "0");//Item Price
                SetDefaultValueToCell("J2", templateSheet, typeof(int), "0");//Order Quantity
                SetDefaultValueToCell("O2", templateSheet, typeof(int), "0");//Vat Rate
                SetDefaultValueToCell("P2", templateSheet, typeof(int), "0");//Exchange Rate
                //-----------------------------------x--------------------------------------

                commonDataSheet.Hidden = OfficeOpenXml.eWorkSheetHidden.Hidden;

                #endregion

                excel.Save();
            }
        }

        private void InsertSingleDataToExcel<T>(string colDropdownName,
            IList<T> listData, int startRow, int startCol, int endCol,
            ExcelWorksheet templateSheet, string dataColName, string propCode, string propName, ExcelWorksheet dataSheet)
        {
            try
            {
                //CommonDataSheet!$B$3:$B$6
                var dropdown = templateSheet.Cells[colDropdownName].DataValidation.AddListDataValidation();
                for (int i = 0; i < listData.Count; i++)
                {
                    dataSheet.Cells[startRow + i, endCol].Value = listData[i].GetType().GetProperty(propName).GetValue(listData[i], null);
                }
                dropdown.Formula.ExcelFormula = $"{dataSheet.Name}!${dataColName}${startRow}:${dataColName}${(startRow + listData.Count - 1)}";
            }
            catch (Exception ex)
            {
                LogWriter log = new LogWriter(ex.Message);
            }
        }

        /// <summary>
        /// Added By PVN
        /// Date Added: 2020-11-20
        /// Description: Write Common Data To Excel File And Create Dropdown List Base On It
        /// </summary>
        private void InsertCommonDataToExcel<T>(string colDropdownName,
            IList<T> listData, int startRow, int startCol, int endCol,
            ExcelWorksheet templateSheet, string dataColName, string propCode, string propName, ExcelWorksheet dataSheet)
        {
            try
            {
                //CommonDataSheet!$B$3:$B$6
                var dropdown = templateSheet.Cells[colDropdownName].DataValidation.AddListDataValidation();
                for (int i = 0; i < listData.Count; i++)
                {
                    dataSheet.Cells[startRow + i, startCol].Value = listData[i].GetType().GetProperty(propCode).GetValue(listData[i], null);
                    dataSheet.Cells[startRow + i, endCol].Value = listData[i].GetType().GetProperty(propName).GetValue(listData[i], null);
                }
                dropdown.Formula.ExcelFormula = $"{dataSheet.Name}!${dataColName}${startRow}:${dataColName}${(startRow + listData.Count - 1)}";
            }
            catch (Exception ex)
            {
                LogWriter log = new LogWriter(ex.Message);
            }

        }

        #region GET COMMON DATA FOR EXCEL TEMPLATE

        private const string SP_MES_GET_COMMON_DATA = "SP_MES_GET_COMMON_DATA";

        private List<MES_ComCodeDtls> GetComCodeDtls()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeDtls>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetComCodeDtlsNew" }).ToList();

                return result;
            }
        }

        private List<MES_ItemClass> GetItemClass()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemClass>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetItemClass" }).ToList();

                return result;
            }
        }

        public List<DynamicCombobox> GetItemClassItemGrid()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[0];

                object[] arrParamsValue = new object[0];

                var result = conn.ExecuteQuery<DynamicCombobox>("SP_GET_COMBOBOX_VALUE_DYNAMIC_ITEMCLASSCODE_IN_GRID", arrParams, arrParamsValue);

                return result.ToList();
            }
        }

        private List<MES_Partner> GetItemPartner()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetItemPartner" }).ToList();

                return result;
            }
        }

        private List<MES_Warehouse> GetWarehouse()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Warehouse>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetWarehouse" }).ToList();

                return result;
            }
        }

        private List<MES_Item> GetItems()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetItem" }).ToList();

                return result;
            }
        }

        private List<MES_Partner> GetPartnerNew()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetSalePJPartner" }).ToList();

                return result;
            }
        }

        private List<MES_Item> GetFinishProducts()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetFinishProducts" }).ToList();

                return result;
            }
        }

        private List<Combobox> GetSaleOrderProjectComCode()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<Combobox>(SP_MES_GET_COMMON_DATA,
                    new string[] { "@Method" },
                    new object[] { "GetSaleOrderProjectCode" }).ToList();

                return result;
            }
        }
        #endregion

        #region VALIDATIONS

        private void ValidateDateTimeCell(string cell, ExcelWorksheet ws)
        {
            var validation = ws.DataValidations.AddDateTimeValidation(cell);
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = "Wrong Format!";
            validation.Error = "Enter Date Here!";
            validation.Formula.Value = DateTime.Now.Date;
            //validation.Formula2.Value = DateTime.Now;
            validation.Operator = ExcelDataValidationOperator.greaterThanOrEqual;
        }

        private void ValidateInteger(string cell, ExcelWorksheet ws)
        {
            var validation = ws.DataValidations.AddIntegerValidation(cell);
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = "Wrong Format!";
            validation.Error = "Enter An Integer Here!";
            validation.Formula.Value = 0;
            validation.Operator = ExcelDataValidationOperator.greaterThanOrEqual;
        }

        private void ValidateDecimal(string cell, ExcelWorksheet ws)
        {
            var validation = ws.DataValidations.AddDecimalValidation(cell);
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = "Wrong Format!";
            validation.Error = "Enter Decimal Here!";
            validation.Formula.Value = 0.0;
            validation.Operator = ExcelDataValidationOperator.greaterThanOrEqual;
        }

        private void ValidateStringLength(string cell, ExcelWorksheet ws, int length)
        {
            //var validation = ws.DataValidations.AddTextLengthValidation($"{cell}:{cell}");
            var validation = ws.DataValidations.AddTextLengthValidation(cell);
            validation.ShowErrorMessage = true;
            validation.ErrorTitle = "Length Validation!";
            validation.Error = "Length must be less than or equal " + length;
            validation.Formula.Value = length;
            validation.Operator = ExcelDataValidationOperator.lessThanOrEqual;
        }


        #endregion

        #region DEFAULT CELL VALUE
        private void SetDefaultValueToCell(string cell, ExcelWorksheet ws, Type type, string value)
        {
            var converter = TypeDescriptor.GetConverter(type);
            ws.Cells[$"{cell}"].Value = converter.ConvertFrom(value);
        }
        #endregion

        public string UploadFileDynamicForImportFromPopup(IFormFile myFile, string chunkMetadata, Type type)
        {
            var tempPath = "";
            string fileName = "";
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

                fileName = metaDataObject.FileGuid + metaDataObject.FileName;


            }

            return tempPath + "\\" + fileName;
        }

        //public Result SaveToDB_SaleProject_Excel(string filePath, string SPName, string UserID)
        //{
        //    try
        //    {
        //        Result result = new Result();
        //        ExcelHelperTest excelHelperTest = new ExcelHelperTest();
        //        // Get data from excecl to DataTable
        //        DataTable dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_SaleProjectExcelTempalte));
        //        // Add DataTable to Dataset

        //        int sendingTimes = (dt.Rows.Count / 10) + 1;
        //        for (int i = 0; i < sendingTimes; i++)
        //        {
        //            string jsonObjTest;
        //            DataTable dtInsert;
        //            if (i == 0)
        //            {
        //                dtInsert = dt.AsEnumerable().Skip(0).Take(10).CopyToDataTable();
        //                jsonObjTest = JsonConvert.SerializeObject(dtInsert);
        //            }
        //            else
        //            {
        //                dtInsert = dt.AsEnumerable().Skip(10 * i).Take(10).CopyToDataTable();
        //                jsonObjTest = JsonConvert.SerializeObject(dtInsert);
        //            }

        //            if (!string.IsNullOrEmpty(jsonObjTest))
        //            {
        //                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
        //                {
        //                    using (var transaction = conn.BeginTransaction())
        //                    {
        //                        try
        //                        {
        //                            var resultInsDtl = 0;
        //                            resultInsDtl = conn.ExecuteNonQuery(SPName, CommandType.StoredProcedure,
        //                                    new string[] { "@ListObj", "@UserID" },
        //                                    new object[] { jsonObjTest, UserID }, transaction
        //                                    );
        //                            transaction.Commit();
        //                            result.Success = true;
        //                            result.Message = MessageCode.MD0004;

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            transaction.Rollback();
        //                            LogWriter log = new LogWriter("Insert batch data failed");
        //                            log.LogWrite(ex.Message);
        //                            return new Result
        //                            {
        //                                Success = false,
        //                                Message = MessageCode.MD0005 + ex.ToString(),
        //                            };
        //                        }

        //                    }
        //                }
        //            }
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public Result SaveToDB_Model_Excel(string filePath, string SPName, string UserID, string checkPage)
        {
            try
            {
                Result result = new Result();
                ExcelHelperTest excelHelperTest = new ExcelHelperTest();
                // Get data from excecl to DataTable
                DataTable dt = null;
                if (checkPage == "SaleOrderProject")
                {
                    dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_SaleOrderProjectExcelTemplate));
                }
                else if (checkPage == "ItemManagement")
                {
                    dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_ItemManagementExcelTemplate));
                }
                else if (checkPage == "SaleProject")
                {
                    dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_SaleProjectExcelTempalte));
                }
                else if (checkPage == "PODetail")
                {
                    dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_ItemPartListExcelTempalte));
                }
                // Add DataTable to Dataset

                int sendingTimes = (dt.Rows.Count / 10) + 1;
                for (int i = 0; i < sendingTimes; i++)
                {
                    string jsonObjTest;
                    DataTable dtInsert;
                    if (i == 0)
                    {
                        dtInsert = dt.AsEnumerable().Skip(0).Take(10).CopyToDataTable();
                        jsonObjTest = JsonConvert.SerializeObject(dtInsert);
                    }
                    else
                    {
                        dtInsert = dt.AsEnumerable().Skip(10 * i).Take(10).CopyToDataTable();
                        jsonObjTest = JsonConvert.SerializeObject(dtInsert);
                    }

                    if (!string.IsNullOrEmpty(jsonObjTest))
                    {
                        using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                        {
                            using (var transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    var resultInsDtl = 0;
                                    resultInsDtl = conn.ExecuteNonQuery(SPName, CommandType.StoredProcedure,
                                            new string[] { "@ListObj", "@UserID" },
                                            new object[] { jsonObjTest, UserID }, transaction
                                            );
                                    transaction.Commit();
                                    result.Success = true;
                                    result.Message = MessageCode.MD0004;

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    LogWriter log = new LogWriter("Insert batch data failed");
                                    log.LogWrite(ex.Message);
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD0005 + ex.ToString(),
                                    };
                                }

                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = MessageCode.MD0005 + ex.ToString()
                };
            }
        }

        public List<MES_ItemPO> Getdata_Model_Excel(string filePath, string SPName, string UserID, string checkPage, string PartnerCode)
        {
            try
            {
                DataTable dt = new DataTable();
                ExcelHelperTest excelHelperTest = new ExcelHelperTest();
                // Get data from excecl to DataTable
                if (checkPage == "PODetail")
                {
                    dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_ItemPartListExcelTempalte));
                }
                string jsonData =  JsonConvert.SerializeObject(dt);
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[3];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@jsonData";
                    arrParams[2] = "@PartnerCode";

                    object[] arrParamsValue = new object[3];
                    arrParamsValue[0] = "SearchListItemByPartnerFromExcel";
                    arrParamsValue[1] = jsonData;
                    arrParamsValue[2] = PartnerCode;
                    var result = conn.ExecuteQuery<MES_ItemPO>("SP_MES_ITEMPARTNER", arrParams, arrParamsValue);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}


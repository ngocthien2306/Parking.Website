using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Utils;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.SalesProject.Services.IService;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Modules.Pleiger.SalesProject.Services.ServiceImp
{
    public class MESSaleProjectService : IMESSaleProjectService
    {
        private string SP_MES_SALEPROJECT = "SP_MES_SALEPROJECT";
        private string SP_MES_SALEPROJECT_SAVE_DATA_GRID = "SP_MES_SALEPROJECT_SAVE_DATA_GRID";
        private string SP_MES_SALEPROJECTSEARCH = "SP_MES_SALEPROJECTSEARCH";
        private string SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP = "SP_GET_COMBOBOX_VALUE_DYNAMIC_BY_SP";
        private string SP_MES_SALEPROJECT_DELETEBYPROJECTSTATUS = "SP_MES_SALEPROJECT_DELETEBYPROJECTSTATUS";
        private string SP_GET_LIST_SALEPROJECT_STATUS = "SP_GET_LIST_SALEPROJECT_STATUS";
        private string SP_HLC_SYFileUpload = "SP_HLC_SYFileUpload";
        private string SP_MES_URL_FILE = "SP_MES_URL_FILE";

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
        public List<MES_UrlByUser> GetListFileUrlSaleProject(string SPCode)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@ProjectCode";
                    object[] arrParamsValue = new object[2];
                    arrParamsValue[0] = "GetUrlFile";
                    arrParamsValue[1] = SPCode;
                    var result = conn.ExecuteQuery<MES_UrlByUser>(SP_MES_URL_FILE, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region "Insert - Update - Delete"

        // Save link file 
        // Thien Add 2022-01-19

        public Result SaveListFile(List<MES_UrlByUser> files, string code)
        {
            var result = new Result();
            using (var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = connect.BeginTransaction())
                {
                    try
                    {
                        var resultSave = 0;
                        string[] arrP = new string[2];
                        arrP[0] = "@Method";
                        arrP[1] = "@ProjectCode";
                        object[] arrPV = new object[2];
                        arrPV[0] = "DeleteUrlFile";
                        arrPV[1] = code;
                        resultSave = connect.ExecuteNonQuery(SP_MES_URL_FILE, CommandType.StoredProcedure, arrP, arrPV, transaction);

                        if (files != null && files.Count > 0)
                        {

                            foreach (var item in files)
                            {
                                string[] arrParams = new string[3];
                                arrParams[0] = "@Method";
                                arrParams[1] = "@Url";
                                arrParams[2] = "@ProjectCode";

                                object[] arrParamsValue = new object[3];
                                arrParamsValue[0] = "SaveUrlFile";
                                arrParamsValue[1] = item.FileUrl;
                                arrParamsValue[2] = code;
                                resultSave += connect.ExecuteNonQuery(SP_MES_URL_FILE, CommandType.StoredProcedure, arrParams, arrParamsValue, transaction);
                            }
                        }
                        transaction.Commit();
                        if (resultSave > 0)
                        {
                            //save success
                            result.Success = true;
                            result.Message = MessageCode.MD0004;
                        }
                        else
                        {
                            //save fail
                            result.Success = false;
                            result.Message = MessageCode.MD0005;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = ex.ToString()
                        };
                    }
                }
            }
            return result;
        }

        // Save data Sale Project
        public Result SaveSalesProject(MES_SaleProject model, string userModify)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    var resultIns = "N";
                    try
                    {
                        string ProjectStatus = "PJST03";
                        if (model.SalesClassification != null && model.SalesClassification == "SCS001")
                        {
                            // Quan add 2021-02-23
                            // IF Project Create SalesClassification=SCS007(Project Sales)
                            // Insert Project Status = Delivery Request(PJST09)
                            ProjectStatus = "PJST09";
                        }
                        resultIns = conn.ExecuteScalar<string>(SP_MES_SALEPROJECT_SAVE_DATA_GRID, CommandType.StoredProcedure,
                            new string[] { "@DIV",
                                   "@ProjectCode","@UserProjectCode", "@ProjectName", "@InCharge","@PlanDeliveryDate", "@ProductType", "@ItemCode",
                                   "@PartnerCode", "@OrderNumber", "@DomesticForeign", "@OrderQuantity", "@MonetaryUnit", "@OrderPrice",
                                   "@ExchangeRate", "@VatType", "@VatRate","@UserModify", "@OrderTeamCode", "@ExchangeRateDate","@SalesClassification","@ETC","@FileID",
                                    "@ProjectStatus","@ItemPrice","@ConversionAmount", "@ProjectCodeMaster", "@SalesOrderProjectCode", "@InitialCode"
                            },
                            new object[] { "Insert",
                                    model.ProjectCode,model.UserProjectCode, model.ProjectName,model.InCharge,model.PlanDeliveryDate,model.ProductType, model.ItemCode,
                                    model.PartnerCode, model.OrderNumber, model.DomesticForeign, model.OrderQuantity,model.MonetaryUnit,model.OrderPrice,
                                    model.ExchangeRate,model.VatType,model.VatRate,userModify, model.OrderTeamCode, model.ExchangeRateDate, model.SalesClassification,
                                    model.ETC,model.FileID,ProjectStatus,model.ItemPrice,model.ConversionAmount, model.ProjectCodeMaster, model.SalesOrderProjectCode, model.InitialCode
                            }, transaction);
                        transaction.Commit();
                        if (resultIns != "N")
                        {
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004,
                                Data = resultIns
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                Success = false,
                                Message = MessageCode.MD0005,
                                Data = ""
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
                            Data = ""
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
                        };
                    }

                }

            }

        }
        #endregion

        #region Search 
        public object SearchSaleProject(MES_SaleProject model, string checkCode)
        {

            string check = "";
            if (checkCode == "Yes")
            {
                check = "1";
            }
            else if (checkCode == "No")
            {
                check = "0";
            }
            else
            {
                check = null;
            }
            var result = new List<MES_SaleProject>();
            var resultExcel = new List<MES_SaleProjectExCel>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[13];
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
                arrParams[10] = "@SalesOrderProjectName";
                arrParams[11] = "@ProjectOrderType";
                arrParams[12] = "@InitialCode";


                object[] arrParamsValue = new object[13];
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
                arrParamsValue[10] = model.SalesOrderProjectName;
                arrParamsValue[11] = model.ProjectOrderType;
                arrParamsValue[12] = check;


                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECTSEARCH, arrParams, arrParamsValue);
                arrParamsValue[0]= "SearchSaleProjectsExcel";
                var dataexcel = conn.ExecuteQuery<MES_SaleProjectExCel>(SP_MES_SALEPROJECTSEARCH, arrParams, arrParamsValue);

                result = data.ToList();
                resultExcel = dataexcel.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);
            return new { Data = result, DataExcel = resultExcel };

            //return result;
        }
        public List<MES_SaleProject> SearchSaleProjectsExcel(MES_SaleProject model, string checkCode)
        {
            string check = "";
            if (checkCode == "Yes")
            {
                check = "1";
            }
            else if (checkCode == "No")
            {
                check = "0";
            }
            else
            {
                check = null;
            }
            var result = new List<MES_SaleProject>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[13];
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
                arrParams[10] = "@SalesOrderProjectName";
                arrParams[11] = "@ProjectOrderType";
                arrParams[12] = "@InitialCode";

                object[] arrParamsValue = new object[13];
                arrParamsValue[0] = "SearchSaleProjectsExcel";
                arrParamsValue[1] = model.ProductType;
                arrParamsValue[2] = model.ProjectCode;
                arrParamsValue[3] = model.ProjectName;
                arrParamsValue[4] = model.ItemCode;
                arrParamsValue[5] = model.ItemName;
                arrParamsValue[6] = model.ProjectStatus != null && model.ProjectStatus.Equals("all") ? "" : model.ProjectStatus;
                arrParamsValue[7] = model.UserProjectCode;
                arrParamsValue[8] = model.OrderTeamCode;
                arrParamsValue[9] = model.UserCode;
                arrParamsValue[10] = model.SalesOrderProjectName;
                arrParamsValue[11] = model.ProjectOrderType;
                arrParamsValue[12] = check;

                var data = conn.ExecuteQuery<MES_SaleProject>(SP_MES_SALEPROJECTSEARCH, arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;
        }
        public object SearchSaleProjectsExcel1(MES_SaleProject model, string checkCode)
        {

            string check = "";
            if (checkCode == "Yes")
            {
                check = "1";
            }
            else if (checkCode == "No")
            {
                check = "0";
            }
            else
            {
                check = null;
            }
            Stopwatch w2 = Stopwatch.StartNew();
            w2.Start();
            var result = new List<MES_SaleProjectExCel>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[13];
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
                arrParams[10] = "@SalesOrderProjectName";
                arrParams[11] = "@ProjectOrderType";
                arrParams[12] = "@InitialCode";
                object[] arrParamsValue = new object[13];
                arrParamsValue[0] = "SearchSaleProjectsExcel";
                arrParamsValue[1] = model.ProductType;
                arrParamsValue[2] = model.ProjectCode;
                arrParamsValue[3] = model.ProjectName;
                arrParamsValue[4] = model.ItemCode;
                arrParamsValue[5] = model.ItemName;
                arrParamsValue[6] = model.ProjectStatus != null && model.ProjectStatus.Equals("all") ? "" : model.ProjectStatus;
                arrParamsValue[7] = model.UserProjectCode;
                arrParamsValue[8] = model.OrderTeamCode;
                arrParamsValue[9] = model.UserCode;
                arrParamsValue[10] = model.SalesOrderProjectName;
                arrParamsValue[11] = model.ProjectOrderType;
                arrParamsValue[12] = check;
                var data = conn.ExecuteQuery<MES_SaleProjectExCel>(SP_MES_SALEPROJECTSEARCH, arrParams, arrParamsValue);
                result = data.ToList();
            }

            //int i = 1;
            //result.ForEach(x => x.No = i++);
            w2.Stop();
            return new {Data = result, Time = w2.ElapsedMilliseconds };
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
        public CHECKRESULT CheckUserType(string UserId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@UserId";
                var arrParamValues = new object[2];
                arrParamValues[0] = "CheckUserType";
                arrParamValues[1] = UserId;
                var result = conn.ExecuteQuery<CHECKRESULT>("SP_MES_CHECK_USER_ROLE", arrParams, arrParamValues).ToList();
                if (result != null || result.Count != 0)
                {
                    return result.FirstOrDefault();
                }
                return new CHECKRESULT();
            }
        }


        // Quan add 
        private const string EXCEL_EXPORT_TEMPLATE_PATH = @"excelTemplate/SalesProjectTemplate.xlsx";
        private const string EXCEL_WORKSHEETS_NAME = "i-Cube Data_";
        private const string EXCEL_DATE_FORMAT = "yyyy-MM-dd HH:mm:sss";
        private const string EXCEL_EXPORT_FOLDER = @"RenderExcel/";
        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "yyyyMMdd";
        public string ExportExcelICube(DataTable dt)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            string excelFileName = $"{EXCEL_WORKSHEETS_NAME}{DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT)}.xlsx";
            string excelSheetName = $"{EXCEL_WORKSHEETS_NAME}{DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT)}";

            FileInfo newExcelFile;
            FileInfo excelTemplate = new FileInfo(EXCEL_EXPORT_TEMPLATE_PATH);

            string curDate = DateTime.Today.ToString("yyyyMM");
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", curDate);
            var tempFilePath = Path.Combine(tempPath, EXCEL_EXPORT_FOLDER);
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);

            //log.LogWrite("tempFilePath tempFilePath : " + tempFilePath);
            using (var excel = new ExcelPackage())
            {
                try
                {
                    //ExcelWorksheet templateSheet = excel.Workbook.Worksheets[EXCEL_WORKSHEETS_NAME];
                    ExcelWorksheet templateSheet = excel.Workbook.Worksheets.Add(excelSheetName);

                    templateSheet.Cells.LoadFromDataTable(dt, true);

                    if (dt.Rows.Count != 0)
                    {

                        int colNumber = 1;

                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.DataType == typeof(DateTime))
                            {
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
                        templateSheet.Cells[1, 1, 1, dt.Columns.Count].Style.WrapText = true;
                        templateSheet.Cells[1, 1, 1, dt.Columns.Count].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        templateSheet.Cells[1, 1, 1, dt.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        templateSheet.Cells[1, 1, 1, dt.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                        // Quan format teamplet
                        templateSheet.Column(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        templateSheet.Column(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        templateSheet.Column(1).Width = 30;
                        templateSheet.Column(2).Width = 12;
                        templateSheet.Column(3).Width = 12;
                        templateSheet.Column(4).Width = 12;
                        templateSheet.Column(5).Width = 15;
                        templateSheet.Column(6).Width = 12;
                        templateSheet.Column(7).Width = 12;
                        templateSheet.Column(8).Width = 12;
                        templateSheet.Column(9).Width = 12;
                        templateSheet.Column(10).Width = 10;
                        templateSheet.Column(11).Width = 15;
                        templateSheet.Column(12).Width = 12;
                        templateSheet.Column(13).Width = 12;
                        templateSheet.Column(14).Width = 12;
                        templateSheet.Column(14).Style.Numberformat.Format = "General";
                        templateSheet.Column(15).Width = 20;
                        templateSheet.Column(17).Width = 16;
                        templateSheet.Column(18).Width = 16;
                        templateSheet.Column(19).Width = 16;
                        templateSheet.Column(20).Width = 16;
                        templateSheet.Column(21).Width = 16;
                        templateSheet.Column(22).Width = 16;
                        templateSheet.Column(24).Width = 16;
                        templateSheet.Column(26).Width = 16;
                        templateSheet.Column(31).Width = 12;
                        templateSheet.Column(37).Width = 12;
                        templateSheet.Column(38).Width = 12;
                        templateSheet.Column(39).Width = 12;
                        templateSheet.Column(47).Width = 12;
                        templateSheet.Column(54).Width = 12;

                        templateSheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        templateSheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        templateSheet.Column(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        templateSheet.Column(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        templateSheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        templateSheet.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        templateSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                        int index = dt.Rows.Count / 3;
                        int fromrow = 2;
                        int torow = 4;
                        for (int i = 1; i <= index; i++)
                        {

                            templateSheet.Cells[fromrow, 1, torow, 1].Merge = true;
                            fromrow = fromrow + 3;
                            torow = torow + 3;

                        }



                        //templateSheet.Cells.AutoFitColumns();
                    }

                    newExcelFile = new FileInfo(tempFilePath + excelFileName);
                    excel.SaveAs(newExcelFile);
                    // log.LogWrite("tempFilePath + excelFileName : " + tempFilePath + excelFileName);
                }
                catch (Exception ex)
                {
                    //log.LogWrite("tempFilePath + excelFileName : ex" + ex.ToString());
                    throw;
                }
            }
            return tempFilePath + excelFileName;
        }

        /// <summary>
        /// Use for inmport excel to I-Cube FI
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        public List<MES_SaleProjectExcelInfor> GetDataExportExcelICube(string jsonObj)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@listProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "SalesProjectGetDataExportExcel";
                arrParamsValue[1] = jsonObj;
                var result = conn.ExecuteQuery<MES_SaleProjectExcelInfor>(SP_MES_SALEPROJECT, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        public List<SYFileUpload> GetSYFileUploadByID(string fileGuid)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@DIV";
                    arrParams[1] = "@FileGuid";

                    object[] arrParamsValue = new object[2];
                    arrParamsValue[0] = "SELECTBYFILEID";
                    arrParamsValue[1] = fileGuid;
                    var result = conn.ExecuteQuery<SYFileUpload>(SP_HLC_SYFileUpload, arrParams, arrParamsValue);

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public Result SaveUrlFile(int Id, string saleProjectID, string fileUrl, int flag)
        {
            var resultIns = 0;
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                       
                        resultIns = conn.ExecuteNonQuery(SP_MES_URL_FILE, CommandType.StoredProcedure,
                                        new string[] { "@Method", "@ProjectCode", "@Url", "@ID", "@Flag" },
                                        new object[] { "SaveUrlFile", saleProjectID, fileUrl, Id, flag }, transaction);
                        transaction.Commit();
                        if (resultIns > 0)
                        {
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004,
                                Data = ""
                            };
                        }
                        else
                        {
                            return new Result
                            {
                                Success = false,
                                Message = MessageCode.MD0005,
                                Data = ""
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


    }
}



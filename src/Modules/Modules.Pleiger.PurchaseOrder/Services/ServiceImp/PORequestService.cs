using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Modules.Pleiger.PurchaseOrder.Services.ServiceImp
{
    public class PORequestService : IPORequestService
    {

        private string SP_Name = "SP_MES_POREQUEST";
        private string SP_MES_POREQUEST_EXTENSION = "SP_MES_POREQUEST_EXTENSION";
        private string SP_MES_POREQUEST_MASTER = "SP_MES_POREQUEST_MASTER";
        private string SP_MES_POREQUEST_DETAIL = "SP_MES_POREQUEST_DETAIL";

        private const string SP_POREQ_DETAIL = "SP_MES_PORequest_DETAIL_PRINTPDF";

        private const string EXCEL_EXPORT_TEMPLATE_PATH = @"excelTemplate/POPrintTemplate.xlsx";
        private const string EXCEL_WORKSHEETS_HEADER_DETAIL = "PO";
        private const string EXCEL_EXPORT_FOLDER = @"PORequestPDF/";
        private const string EXCEL_EXPORT_NAME_DATE_FORMAT = "yyyyMMddhhmmss";
        private const string EXCEL_FILE_NAME = "PORequest";
        private const string EXCEL_WORKSHEETS_NAME = "PO";
        private const string EXCEL_WORKSHEETS_FOOTER_NAME = "Footer";
        private const string REGEX_REPLACE_DATA = @"[-/[%^*()!+\]]";


        #region "Get Data"

        // Get list Item Class
        public List<MES_PORequest> GetListData()
        {
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListData";
                var data = conn.ExecuteQuery<MES_PORequest>(SP_Name, arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        // Search list PO Request
        public List<MES_PORequest> SearchListPORequest(string projectCode, string poNumber, string UserPONumber,
            string projectName, string requestDateFrom, string requestDateTo, string partnerCode, string poStatus, string SalesClassification, bool isPartner)
        {
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var methodName = "";
                if (isPartner)
                {
                    methodName = "SearchListPORequestByPartner";
                }
                else
                {
                    methodName = "SearchListPORequest";
                }
                string[] arrParams = new string[10];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@PONumber";
                arrParams[3] = "@RequestDateFrom";
                arrParams[4] = "@RequestDateTo";
                arrParams[5] = "@UserPONumber";
                arrParams[6] = "@ProjectName";// Quan change task 2021-03-29 UserProjectCode => ProjectName
                arrParams[7] = "@PartnerCode";
                arrParams[8] = "@POStatus";
                arrParams[9] = "@SalesClassification";

                object[] arrParamsValue = new object[10];
                arrParamsValue[0] = methodName;
                arrParamsValue[1] = projectCode;
                arrParamsValue[2] = poNumber;
                arrParamsValue[3] = requestDateFrom;
                arrParamsValue[4] = requestDateTo;
                arrParamsValue[5] = UserPONumber;
                arrParamsValue[6] = projectName;
                arrParamsValue[7] = partnerCode;
                arrParamsValue[8] = poStatus;
                arrParamsValue[9] = SalesClassification;
                var data = conn.ExecuteQuery<MES_PORequest>(SP_Name, arrParams, arrParamsValue);//SP_MES_POREQUEST

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        // Get PO Detail
        public MES_PORequest GetPODetail(string poNumber)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[2];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@PONumber";
                    object[] arrParamsValue = new object[2];
                    arrParamsValue[0] = "GetPODetail";
                    arrParamsValue[1] = poNumber;
                    var data = conn.ExecuteQuery<MES_PORequest>(SP_Name, arrParams, arrParamsValue);
                    return data.FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        // Get list Item PO Request
        public List<MES_ItemPO> GetListItemPORequest(string poNumber)
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
                var data = conn.ExecuteQuery<MES_ItemPO>(SP_Name, arrParams, arrParamsValue);
                result = data.ToList();
            }
            int i = 1;
            result.ForEach(x => x.No = i++);
            return result;
        }
        public List<MES_ItemClass> GetListItemClassCode()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListItemClass";
                var data = conn.ExecuteQuery<MES_ItemClass>(SP_MES_POREQUEST_EXTENSION, arrParams, arrParamsValue);

                return data.ToList();
            }
        }
        public List<MES_HistoryDeliveryItem> GetListHistoryUpdateDeliveryItem(string Ponumber, string ItemCode, string UserID)
        {
            try
            {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    string[] arrParams = new string[4];
                    arrParams[0] = "@Method";
                    arrParams[1] = "@PONumber";
                    arrParams[2] = "@ItemCode";
                    arrParams[3] = "@UserEdit";
                    object[] arrParamsValue = new object[4];
                    arrParamsValue[0] = "GetListHistoryDelivery";
                    arrParamsValue[1] = Ponumber;
                    arrParamsValue[2] = ItemCode;
                    arrParamsValue[3] = UserID;
                    var data = conn.ExecuteQuery<MES_HistoryDeliveryItem>(SP_MES_POREQUEST_MASTER, arrParams, arrParamsValue);

                    return data.ToList();
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region "Insert - Update - Delete"

        // Save data PO Request
        public Result SaveDataPORequest(string projectCode, string saveDataType, string requestCode, string poNumber, string partnerCode, string arrivalRequestDate, string listItemPORequest, string userModify, string UserPONumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = -1;
                try
                {
                    result = conn.ExecuteNonQuery(SP_Name,
                        new string[] { "@Method", "@SaveDataType", "@ProjectCode", "@PONumber", "@PartnerCode", "@ArrivalRequestDate", "@ListItemPORequest", "@UserModify", "@UserPONumber" },
                        // new object[] { "SaveDataPORequest", saveDataType, projectCode, poNumber, partnerCode, arrivalRequestDate, listItemPORequest, userModify, UserPONumber });
                        new object[] { "SaveDataPORequest", saveDataType, projectCode, poNumber, partnerCode, arrivalRequestDate, listItemPORequest, userModify, UserPONumber });
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

        #endregion

        #region "Export PDF File"
        /// <summary>
        /// Added By PVN
        /// Date Added: 2020-08-31
        /// </summary>
        /// <param name="poNumber"></param>
        /// <returns></returns>
        private async Task<MES_PORequest> GetPOData(string poNumber)
        {
            Debug.WriteLine("PVN Test 1");
            Debug.WriteLine("GetPOData Run At " + DateTime.Now);
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetHeader";
                arrParamsValue[1] = poNumber;
                var data = conn.ExecuteQuery<MES_PORequest>(SP_POREQ_DETAIL, arrParams, arrParamsValue);

                result = data.ToList();

            }

            return result.FirstOrDefault();
        }

        private async Task<MES_PORequest> GetPOExportExcel(string poNumber)
        {
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetPOExportExcel";
                arrParamsValue[1] = poNumber;
                var data = conn.ExecuteQuery<MES_PORequest>("sp_MES_PORequest", arrParams, arrParamsValue);

                result = data.ToList();

            }

            return result.FirstOrDefault();
        }
        

        private async Task<List<MES_PORequest>> GetPODetailData(string poNumber)
        {
            Debug.WriteLine("PVN Test 2");
            Debug.WriteLine("GetPODetailData Run At " + DateTime.Now);
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetDetail";
                arrParamsValue[1] = poNumber;
                var data = conn.ExecuteQuery<MES_PORequest>(SP_POREQ_DETAIL, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }
        
        public List<MES_PORequest> GetPODataPrint(string poNumber)
        {
            Debug.WriteLine("PVN Test 1");
            Debug.WriteLine("GetPOData Run At " + DateTime.Now);
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetHeader";
                arrParamsValue[1] = poNumber;
                var data = conn.ExecuteQuery<MES_PORequest>(SP_POREQ_DETAIL, arrParams, arrParamsValue);

                result = data.ToList();

            }

            return result;
        }

        public List<MES_PORequest> GetPODetailDataPrint(string poNumber)
        {
            Debug.WriteLine("PVN Test 2");
            Debug.WriteLine("GetPODetailData Run At " + DateTime.Now);
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetDetailAndHeader";
                arrParamsValue[1] = poNumber;
                var data = conn.ExecuteQuery<MES_PORequest>(SP_POREQ_DETAIL, arrParams, arrParamsValue);

                result = data.ToList();
            }

            return result;
        }
        public async Task<string> CreatePOPrint(string poNumber)
        {
            var PDFHeader = await GetPOData(poNumber).ConfigureAwait(false);
            var PDFDetail = await GetPODetailData(poNumber).ConfigureAwait(false);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            string excelFileName = $"{EXCEL_FILE_NAME}{DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT)}.xlsx";
            string pdfFileName = "";
            FileInfo newExcelFile;
            FileInfo excelTemplate = new FileInfo(EXCEL_EXPORT_TEMPLATE_PATH);

            string curDate = DateTime.Today.ToString("yyyyMM");
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", curDate);
            var tempFilePath = Path.Combine(tempPath, EXCEL_EXPORT_FOLDER);
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);

            using (var excel = new ExcelPackage(excelTemplate))
            {
                ExcelWorksheet POSheet = excel.Workbook.Worksheets[EXCEL_WORKSHEETS_NAME];

                // Fill all the header of excel file
                POSheet.Cells["D7"].Value = PDFHeader.Supplier;
                POSheet.Cells["D8"].Value = PDFHeader.Address;
                POSheet.Cells["D9"].Value = PDFHeader.To;
                POSheet.Cells["M7"].Value = PDFHeader.OrderNumber;
                POSheet.Cells["M8"].Value = PDFHeader.Date;
                POSheet.Cells["M9"].Value = PDFHeader.YourRef;
                POSheet.Cells["M10"].Value = PDFHeader.ProjectInfo;
                POSheet.Cells["M11"].Value = PDFHeader.Responsible;
                POSheet.Cells["M12"].Value = PDFHeader.PhoneNo;

                POSheet.Cells["D8"].Style.WrapText = true;
                POSheet.Cells["D9"].Style.WrapText = true;

                int row = 16;
                Debug.WriteLine("PVN Test 3");
                Debug.WriteLine("GetPODetailData Run At " + DateTime.Now);
                try
                {
                    PDFDetail.ToList().ForEach(s =>
                    {
                        POSheet.Cells[$"A{row}"].Value = s.No.ToString();
                        POSheet.Cells[$"B{row}"].Value = s.ItemNO;

                        POSheet.Cells[$"H{row}"].Value = s.POQty;
                        POSheet.Cells[$"H{row}"].Style.Numberformat.Format = "#,###";
                        POSheet.Cells[$"H{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        POSheet.Cells[$"I{row}"].Value = s.Unit;
                        POSheet.Cells[$"I{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        POSheet.Cells[$"J{row}"].Value = s.Curency;
                        POSheet.Cells[$"J{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                        POSheet.Cells[$"K{row}"].Value = s.UnitPrice;
                        POSheet.Cells[$"M{row}"].Value = s.Total;

                        POSheet.Cells[$"B{row + 1}"].Value = s.ItemName;
                        //merge cell and wrap for display long text
                        POSheet.Cells[$"B{row + 1}" + ":" + $"G{row + 1}"].Merge = true;
                        POSheet.Cells[$"B{row + 1}"].Style.WrapText = true;
                        POSheet.Row(row + 1).Height = 40;


                        POSheet.Cells[$"B{row + 2}"].Value = "Remark 1: ";
                        POSheet.Row(row + 2).Height = 50;
                        POSheet.Cells[$"B{row + 3}"].Value = "Remark 2: ";
                        POSheet.Row(row + 3).Height = 50;
                        // Pleiger rmk
                        POSheet.Cells[$"D{row + 2}"].Value = s.PleigerRemark;
                        //merge cell and wrap for display long text
                        POSheet.Cells[$"D{row + 2}" + ":" + $"G{row + 2}"].Merge = true;
                        POSheet.Cells[$"D{row + 2}"].Style.WrapText = true;

                        // Partner rmk
                        POSheet.Cells[$"D{row + 3}"].Value = s.PORemark;
                        //merge cell and wrap for display long text
                        POSheet.Cells[$"D{row + 3}" + ":" + $"G{row + 3}"].Merge = true;
                        POSheet.Cells[$"D{row + 3}"].Style.WrapText = true;

                        POSheet.Cells[$"B{row + 4}"].Value = "Delivery Request: ";
                        POSheet.Cells[$"E{row + 4}"].Value = s.DeliveryRequestDate != null ?
                        DateTime.Parse(s.DeliveryRequestDate.ToString()).ToString("yyyy-MM-dd") : null;

                        POSheet.Cells[$"K{row}:M{row}"].Style.Numberformat.Format = "#,###.00";
                        POSheet.Cells[$"K{row}"].AutoFitColumns();

                        row += 6;

                        POSheet.Cells[$"A{row}:M{row}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    });
                }
                catch (Exception e)
                {

                }

                ExcelWorksheet footerSheet = excel.Workbook.Worksheets[EXCEL_WORKSHEETS_FOOTER_NAME];

                footerSheet.Cells["K1"].Value = PDFDetail.FirstOrDefault().Curency;
                footerSheet.Cells["M1"].Value = PDFDetail.Sum(m => m.Total);
                footerSheet.Cells["D3"].Value = PDFHeader.Packing;
                footerSheet.Cells["D4"].Value = PDFHeader.TermDelivery;
                footerSheet.Cells["D5"].Value = PDFHeader.TermPayment;
                footerSheet.Cells["D6"].Value = PDFHeader.RequestShipMode;

                //POSheet.Cells[$"A{row}:A{row+6}"].AutoFitColumns();

                footerSheet.Cells[1, 1, 17, 13].Copy(POSheet.Cells[row, 1, row + 17, 13]);

                POSheet.Row(row + 15).Height = 200;
                POSheet.Column(1).Width = 10;
                POSheet.Column(3).Width = 0;
                POSheet.Column(12).Width = 0;
                POSheet.Column(11).AutoFit();
                POSheet.Column(13).AutoFit();



                newExcelFile = new FileInfo(tempFilePath + excelFileName);

                excel.Workbook.Worksheets.Delete("Footer");
                excel.SaveAs(newExcelFile);

                Workbook workbook = new Workbook();
                //Load excel file  
                workbook.LoadFromFile(tempFilePath + excelFileName);

                pdfFileName = tempFilePath + excelFileName.Remove(excelFileName.LastIndexOf('.')) + ".pdf";
                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {
                    workbook.Worksheets[i].PageSetup.FitToPagesWide = 1;
                    workbook.Worksheets[i].PageSetup.FitToPagesTall = 0;
                    workbook.Worksheets[i].SaveToPdf(pdfFileName);


                }        

            }
            Debug.WriteLine("PVN Test 4");
            Debug.WriteLine("GetPODetailData Run At " + DateTime.Now);
            

            return pdfFileName;
        }

        private async Task<List<MES_ItemPO>> GetListItemPORequestExportExcel(string poNumber)
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
                var data = conn.ExecuteQuery<MES_ItemPO>(SP_Name, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }
        public async Task<string> ExportExcelPO(string poNumber)
        {

            string  EXCEL_EXPORT_PO_TEMPLATE_PATH = @"excelTemplate/POExportExcelTemplate.xlsx";

           // var DataHeader = await GetPOData(poNumber).ConfigureAwait(false);

            var DataHeader = await GetPOExportExcel(poNumber).ConfigureAwait(false);
            var PDFDetail = await GetListItemPORequestExportExcel(poNumber).ConfigureAwait(false);




            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            string excelFileName = $"{EXCEL_FILE_NAME}{DateTime.Now.ToString(EXCEL_EXPORT_NAME_DATE_FORMAT)}.xlsx";
            string pdfFileName = "";
            FileInfo newExcelFile;
            FileInfo excelTemplate = new FileInfo(EXCEL_EXPORT_PO_TEMPLATE_PATH);

            string curDate = DateTime.Today.ToString("yyyyMM");
            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", curDate);
            var tempFilePath = Path.Combine(tempPath, EXCEL_EXPORT_FOLDER);
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);

            using (var excel = new ExcelPackage(excelTemplate))
            {
                ExcelWorksheet POSheet = excel.Workbook.Worksheets["PODATA"];

                // Fill all the header of excel file
                POSheet.Cells["C3"].Value = DataHeader.UserProjectCode;
                POSheet.Cells["C5"].Value = DataHeader.BusinessType;
                POSheet.Cells["C7"].Value = DataHeader.FinalShipmentMode;
                POSheet.Cells["C9"].Value = DataHeader.Schedule;
                POSheet.Cells["C11"].Value = DataHeader.PartnerUser;

                POSheet.Cells["E3"].Value = DataHeader.PartnerName;
                POSheet.Cells["E5"].Value = DataHeader.RealArrivalReqDate;
                POSheet.Cells["E7"].Value = DataHeader.Packing;
                POSheet.Cells["E9"].Value = DataHeader.Mon;
                POSheet.Cells["E11"].Value = DataHeader.RefNumber;

                POSheet.Cells["G3"].Value = DataHeader.UserPONumber;
                POSheet.Cells["G5"].Value = DataHeader.InvoiceIssuedDate;
                POSheet.Cells["G7"].Value = DataHeader.TermDelivery;
                POSheet.Cells["G9"].Value = DataHeader.SPPR;

                POSheet.Cells["I3"].Value = DataHeader.RequestDate;
                POSheet.Cells["I5"].Value = DataHeader.ConnectionToDemand;
                POSheet.Cells["I7"].Value = DataHeader.TermPayment;
                POSheet.Cells["I9"].Value = DataHeader.SPPriceRef;

                POSheet.Cells["K3"].Value = DataHeader.UserRequest;
                POSheet.Cells["K5"].Value = DataHeader.Yard;
                POSheet.Cells["K7"].Value = DataHeader.OrderConfirmNumber;
                POSheet.Cells["K9"].Value = DataHeader.BLCode;

                POSheet.Cells["M3"].Value = DataHeader.AcceptDate;
                POSheet.Cells["M5"].Value = DataHeader.UserAccept;
                POSheet.Cells["M7"].Value = DataHeader.RequestShipMode;
                POSheet.Cells["M9"].Value = DataHeader.HullNo;
                POSheet.Cells["M11"].Value = DataHeader.Invoice;

                POSheet.Cells["O3"].Value = DataHeader.RejectDate;
                POSheet.Cells["O5"].Value = DataHeader.UserReject;


                //POSheet.Cells["M9"].Value = DataHeader.YourRef;
                //POSheet.Cells["M10"].Value = DataHeader.ProjectInfo;
                //POSheet.Cells["M11"].Value = DataHeader.Responsible;
                //POSheet.Cells["M12"].Value = DataHeader.PhoneNo;
                //POSheet.Cells["D8"].Style.WrapText = true;
                //POSheet.Cells["D9"].Style.WrapText = true;

                int row = 16;

                Debug.WriteLine("Q Test 3");
                Debug.WriteLine("Q Run At " + DateTime.Now);

                try
                {
                    PDFDetail.ToList().ForEach(s =>
                    {
                        POSheet.Cells[$"A{row}"].Value = s.No.ToString();
                        POSheet.Cells[$"B{row}"].Value = s.ItemCode;

                        POSheet.Cells[$"C{row}"].Value = s.ItemName;
                        POSheet.Cells[$"H{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        POSheet.Cells[$"D{row}"].Value = s.ItemPrice;
                        POSheet.Cells[$"D{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        POSheet.Cells[$"D{row}"].Style.Numberformat.Format = "#,###";

                        POSheet.Cells[$"E{row}"].Value = s.POQty;
                        POSheet.Cells[$"E{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        POSheet.Cells[$"E{row}"].Style.Numberformat.Format = "#,###";


                        POSheet.Cells[$"F{row}"].Value = s.TotalPrice;
                        POSheet.Cells[$"F{row}"].Style.Numberformat.Format = "#,###";
                        POSheet.Cells[$"F{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        POSheet.Cells[$"G{row}"].Value = s.MonetaryUnit;

                        POSheet.Cells[$"H{row}"].Value =  s.ArrivalRequestDate != null ?  DateTime.Parse(s.ArrivalRequestDate.ToString()).ToString("yyyy-MM-dd") : null;
                        POSheet.Cells[$"H{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        POSheet.Cells[$"I{row}"].Value =  s.DeliveryDate != null ? DateTime.Parse(s.DeliveryDate.ToString()).ToString("yyyy-MM-dd") : null;
                        POSheet.Cells[$"I{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        POSheet.Cells[$"J{row}"].Value =  s.PlanCompleteDate != null ? DateTime.Parse(s.PlanCompleteDate.ToString()).ToString("yyyy-MM-dd") : null;
                        POSheet.Cells[$"J{row}"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                        POSheet.Cells[$"K{row}"].Value = s.PleigerRemark;
                        POSheet.Cells[$"L{row}"].Value = s.PleigerRemark2;
                        POSheet.Cells[$"M{row}"].Value = s.PORemark;
                        POSheet.Cells[$"N{row}"].Value = s.ItemStatusName;

                        row += 1;
                        //POSheet.Cells[$"B{row + 1}"].Value = s.ItemName;
                        ////merge cell and wrap for display long text
                        //POSheet.Cells[$"B{row + 1}" + ":" + $"G{row + 1}"].Merge = true;
                        //POSheet.Cells[$"B{row + 1}"].Style.WrapText = true;
                        //POSheet.Row(row + 1).Height = 40;


                        //POSheet.Cells[$"B{row + 2}"].Value = "Remark 1: ";
                        //POSheet.Row(row + 2).Height = 50;
                        //POSheet.Cells[$"B{row + 3}"].Value = "Remark 2: ";
                        //POSheet.Row(row + 3).Height = 50;
                        //// Pleiger rmk
                        //POSheet.Cells[$"D{row + 2}"].Value = s.PleigerRemark;
                        ////merge cell and wrap for display long text
                        //POSheet.Cells[$"D{row + 2}" + ":" + $"G{row + 2}"].Merge = true;
                        //POSheet.Cells[$"D{row + 2}"].Style.WrapText = true;

                        //// Partner rmk
                        //POSheet.Cells[$"D{row + 3}"].Value = s.PORemark;
                        ////merge cell and wrap for display long text
                        //POSheet.Cells[$"D{row + 3}" + ":" + $"G{row + 3}"].Merge = true;
                        //POSheet.Cells[$"D{row + 3}"].Style.WrapText = true;

                        //POSheet.Cells[$"B{row + 4}"].Value = "Delivery Request: ";
                        //POSheet.Cells[$"E{row + 4}"].Value = s.DeliveryRequestDate != null ?
                        //DateTime.Parse(s.DeliveryRequestDate.ToString()).ToString("yyyy-MM-dd") : null;

                        //POSheet.Cells[$"K{row}:M{row}"].Style.Numberformat.Format = "#,###.00";
                        //POSheet.Cells[$"K{row}"].AutoFitColumns();



                        //POSheet.Cells[$"A{row}:M{row}"].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    });
                }
                catch (Exception e)
                {

                }

                newExcelFile = new FileInfo(tempFilePath + excelFileName);

                excel.SaveAs(newExcelFile);

                Workbook workbook = new Workbook();
                //Load excel file  
                workbook.LoadFromFile(tempFilePath + excelFileName);

                pdfFileName = tempFilePath + excelFileName.Remove(excelFileName.LastIndexOf('.')) + ".pdf";
                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {
                    workbook.Worksheets[i].PageSetup.FitToPagesWide = 1;
                    workbook.Worksheets[i].PageSetup.FitToPagesTall = 0;
                    workbook.Worksheets[i].SaveToPdf(pdfFileName);


                }

            }

            return newExcelFile.FullName;
        }

        
        public Result SavePO1(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId)
        {
            foreach (var item in listDetails)
            {
                PORequest.TotalPrice += item.TotalPrice;
            }
            // PORequest.TotalPrice = totalPrice;
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {   //Insert new PO
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Insert"))
                        {
                            PORequest.StatusCode = "ORST06";//created
                            if (string.IsNullOrEmpty(PORequest.PONumber))
                            {
                                PORequest.PONumber = String.Empty;
                            }
                            var PONumberNew = conn.ExecuteScalar<string>(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                                new string[] { "@Method", "@PONumber", "@PartnerCode", "@ProjectCode", "@UserPONumber", "@POStatus",
                                               "@TotalPrice","@PurchaseDate","@PurchaseUserId","@ArrivalRequestDate","@CreateBy",
                                               //purMaster over sea
                                               "@OrderConfirmNumber","@HullNo","@BusinessType","@ConnectionToDemand","@Yard","@Schedule","@Mon",
                                               "@SPPR","@SPPriceRef","@RequestShipMode","@FinalShipmentMode","@BLCode","@Invoice","@InvoiceIssuedDate",
                                               "@PartnerUser","@Packing","@RefNumber","@TermDelivery","@TermPayment","@RealArrivalReqDate"
                                },
                                new object[] {"InsertMasterPO",PORequest.PONumber,PORequest.PartnerCode,PORequest.ProjectCode,
                                               PORequest.UserPONumber,PORequest.StatusCode,PORequest.TotalPrice,DateTime.Now,UserId,PORequest.ArrivalRequestDate,UserId,
                                               //over sea
                                               PORequest.OrderConfirmNumber,PORequest.HullNo,PORequest.BusinessType,PORequest.ConnectionToDemand,
                                               PORequest.Yard,PORequest.Schedule,PORequest.Mon,PORequest.SPPR,PORequest.SPPriceRef,
                                               PORequest.RequestShipMode,PORequest.FinalShipmentMode,PORequest.BLCode,PORequest.Invoice,
                                               PORequest.InvoiceIssuedDate,PORequest.PartnerUser,PORequest.Packing,PORequest.RefNumber,
                                               PORequest.TermDelivery,PORequest.TermPayment,PORequest.RealArrivalReqDate
                                },
                                transaction);
                            if (PONumberNew == null)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0005
                                };
                            }
                            else
                            {
                                //print ting status => false => khi accept mới update cho là true
                                int seq = 1;
                                foreach (var item in listDetails)
                                {
                                    conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                        new string[] {"@Method","@Seq","@PONumber","@UserPONumber","@ProjectCode","@ItemCode","@POQty","@ItemPrice",
                                            "@Amt","@Tax","@TotalPrice","@DeliverQty","@LeadTimeType","@LeadTime","@PlanCompleteDate","@DeliveryDate",
                                            "@Status","@PleigerRemark","@PORemark","@PartnerAcceptor","@PrintingStatus","@Created_By","@Created_At"
                                       },
                                        new object[] {"SaveDetailPO",seq++,PONumberNew,PORequest.UserPONumber ,PORequest.ProjectCode,item.ItemCode,item.POQty,
                                        item.ItemPrice,item.Amt,item.Tax,item.TotalPrice,item.DeliverQty,item.LeadTimeType,item.LeadTime,
                                        DateTime.Now,DateTime.Now,PORequest.StatusCode,item.PleigerRemark,item.PORemark,
                                        item.PartnerAcceptor,false,UserId,DateTime.Now},
                                transaction);
                                }
                                transaction.Commit();
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0004,
                                    Data = PONumberNew != null ? PONumberNew : ""
                                };
                            }
                        }
                        //Edit PO 
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update"))
                        {
                            PORequest.StatusCode = "ORST06";//created
                                                            //when update or request
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                            new string[] { "@Method", "@PONumber","@ProjectCode","@PartnerCode", "@UserPONumber", "@POStatus",
                                                "@TotalPrice","@PurchaseDate","@PurchaseUserId","@ArrivalRequestDate","@UserModify",
                                                //purMaster over sea
                                               "@OrderConfirmNumber","@HullNo","@BusinessType","@ConnectionToDemand","@Yard","@Schedule","@Mon",
                                               "@SPPR","@SPPriceRef","@RequestShipMode","@FinalShipmentMode","@BLCode","@Invoice","@InvoiceIssuedDate",
                                               "@PartnerUser","@Packing","@RefNumber","@TermDelivery","@TermPayment","@RealArrivalReqDate"
                           },
                            new object[] {"UpdateMasterPO",PORequest.PONumber,PORequest.ProjectCode,PORequest.PartnerCode,
                                               PORequest.UserPONumber,PORequest.StatusCode,PORequest.TotalPrice,DateTime.Now,
                                               UserId,PORequest.ArrivalRequestDate,PORequest.UserModify ?? UserId,
                                               //over sea
                                               PORequest.OrderConfirmNumber,PORequest.HullNo,PORequest.BusinessType,PORequest.ConnectionToDemand,
                                               PORequest.Yard,PORequest.Schedule,PORequest.Mon,PORequest.SPPR,PORequest.SPPriceRef,
                                               PORequest.RequestShipMode,PORequest.FinalShipmentMode,PORequest.BLCode,PORequest.Invoice,
                                               PORequest.InvoiceIssuedDate,PORequest.PartnerUser,PORequest.Packing,PORequest.RefNumber,
                                               PORequest.TermDelivery,PORequest.TermPayment,PORequest.RealArrivalReqDate
                           }, transaction);
                            if (updateMST > 0)
                            {
                                var resultUpdateDtl = 0;
                                foreach (var item in listDetails)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                            new string[] {"@Method","@PONumber","@ProjectCode","@ItemCode","@POQty","@ItemPrice",
                                                          "@Amt","@Tax","@TotalPrice","@DeliverQty","@LeadTimeType","@LeadTime","@PlanCompleteDate",
                                                          "@Status","@PleigerRemark","@PORemark","@PartnerAcceptor","@PrintingStatus",
                                                          "@Updated_By","@Updated_At"},
                                            new object[] {"UpdateDetailPO",PORequest.PONumber,PORequest.ProjectCode,
                                                           item.ItemCode,item.POQty,item.ItemPrice,item.Amt,item.Tax,item.TotalPrice,item.DeliverQty,
                                                           item.LeadTimeType,item.LeadTime,DateTime.Now,PORequest.StatusCode,item.PleigerRemark,
                                                           item.PORemark,item.PartnerAcceptor,false,
                                                           UserId,DateTime.Now }, transaction);
                                }
                                transaction.Commit();
                                if (resultUpdateDtl > 0)
                                {
                                    // save success
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004,
                                        Data = PORequest.PONumber
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
                                    // Update fail
                                    Success = false,
                                    Message = "Update Fail !"
                                };
                            }
                        }
                        //when Request
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update-Request"))
                        {
                            PORequest.StatusCode = "ORST01";
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                               new string[] { "@Method", "@PONumber","@ProjectCode","@PartnerCode", "@UserPONumber", "@POStatus",
                                "@TotalPrice","@PurchaseDate","@PurchaseUserId"},
                               new object[] {"UpdateMasterPO-Request",PORequest.PONumber,PORequest.ProjectCode,PORequest.PartnerCode,
                                PORequest.UserPONumber,PORequest.StatusCode,PORequest.TotalPrice,DateTime.Now,UserId
                               }, transaction);
                            if (updateMST > 0)
                            {
                                var resultUpdateDtl = 0;
                                foreach (var item in listDetails)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                            new string[] {"@Method","@PONumber","@ProjectCode","@ItemCode","@POQty","@ItemPrice",
                                                "@Amt","@Tax","@TotalPrice","@DeliverQty","@LeadTimeType","@LeadTime",
                                                "@Status","@PleigerRemark","@PORemark","@PartnerAcceptor","@PrintingStatus"
                                                },
                                            new object[] {"UpdateDetailPO",PORequest.PONumber,PORequest.ProjectCode,
                                             item.ItemCode,item.POQty,item.ItemPrice,item.Amt,item.Tax,item.TotalPrice,item.DeliverQty,
                                             item.LeadTimeType,item.LeadTime,PORequest.StatusCode,item.PleigerRemark,
                                             item.PORemark,item.PartnerAcceptor,true
                                           }, transaction);
                                }
                                transaction.Commit();
                                if (resultUpdateDtl > 0)
                                {
                                    // save success
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004,
                                        Data = PORequest.PONumber
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
                                    // Update fail
                                    Success = false,
                                    Message = "Update Fail !"
                                };
                            }
                        }
                        //when accept
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update-Accept"))
                        {
                            PORequest.StatusCode = "ORST02";//accept
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                            new string[] { "@Method", "@PONumber","@ProjectCode", "@POStatus",
                                           "@AcceptDate","@UserIDAccepted"
                                    },
                            new object[] {"UpdateMasterPO-Accept",PORequest.PONumber,PORequest.ProjectCode,PORequest.StatusCode,DateTime.Now,UserId
                            }, transaction);
                            if (updateMST > 0)
                            {
                                var resultUpdateDtl = 0;
                                foreach (var item in listDetails)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                            new string[] {"@Method","@PONumber","@ProjectCode","@ItemCode",
                                                "@Status","@PrintingStatus","@DeliveryDate"
                                              },
                                            new object[] {"UpdateDetailPO-Accept",PORequest.PONumber,PORequest.ProjectCode,
                                             item.ItemCode,PORequest.StatusCode,false,item.DeliveryDate
                                            }, transaction);
                                }
                                transaction.Commit();
                                if (resultUpdateDtl > 0)
                                {
                                    // save success
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004,
                                        Data = PORequest.PONumber
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
                                    // Update fail
                                    Success = false,
                                    Message = "Accept Fail !"
                                };
                            }
                        }
                        //when reject
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update-Reject"))
                        {
                            PORequest.StatusCode = "ORST03";//reject
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                                 new string[] { "@Method", "@PONumber","@ProjectCode", "@POStatus",
                                           "@RejectDate","@UserIDReject"
                                         },
                                 new object[] {"UpdateMasterPO-Reject",PORequest.PONumber,PORequest.ProjectCode,PORequest.StatusCode,DateTime.Now,UserId
                                 }, transaction);
                            if (updateMST > 0)
                            {
                                var resultUpdateDtl = 0;
                                foreach (var item in listDetails)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                            new string[] {"@Method","@PONumber","@ProjectCode","@ItemCode","@Status",
                                                "@PrintingStatus","@DeliveryDate"
                                              },
                                            new object[] {"UpdateDetailPO-Reject",PORequest.PONumber,PORequest.ProjectCode,
                                             item.ItemCode,PORequest.StatusCode,false,item.DeliveryDate
                                            }, transaction);
                                }
                                transaction.Commit();
                                if (resultUpdateDtl > 0)
                                {
                                    // save success
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004,
                                        Data = PORequest.PONumber
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
                                    // Update fail
                                    Success = false,
                                    Message = "Reject Fail !"
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
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }

        // Thien add 2022-01-28
        public Result UpdateHistoryDeliveryDateItem(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId)
        {
            var result = new Result();
            using(var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = connect.BeginTransaction())
                {
                    try
                    {
                        var resultUpdateDtl = 0;
                        foreach (var item in listDetails)
                        {
                            if(item.DeliveryDate != null)
                            {
                                resultUpdateDtl = connect.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                                new string[] { "@Method", "@PONumber", "@ItemCode", "@PartnerEditDate", "@UserEdit", "@UserPONumber" },
                                new object[] {"UpdateHistoryDeliveryDate", PORequest.PONumber, item.ItemCode, item.DeliveryDate, UserId, PORequest.UserPONumber
                                }, transaction);
                            }

                        }
                        if (resultUpdateDtl > 0)
                        {
                            transaction.Commit();
                            result.Success = true;
                            result.Message = MessageCode.MD0004;
                            result.Data = PORequest.PONumber;
                            // save success
                        }
                        else
                        {
                            transaction.Rollback();
                            // Save fail
                            result.Success = false;
                            result.Message = MessageCode.MD0005;
                        }

                    }
                    
                    catch(Exception ex)
                    {
                        return new Result
                        {
                            Success = false,
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }
        // Quan add 2020/09/25        
        public Result SavePO(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId)
        {
            decimal totalPrice = 0;
            foreach (var item in listDetails)
            {
                // PORequest.TotalPrice += item.TotalPrice;
                totalPrice += item.TotalPrice;
            }
            // PORequest.TotalPrice = totalPrice;
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {   //Insert new PO
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Insert"))
                        {
                            PORequest.StatusCode = "ORST06";//created
                            if (string.IsNullOrEmpty(PORequest.PONumber))
                            {
                                PORequest.PONumber = String.Empty;
                            }
                            var PONumberNew = conn.ExecuteScalar<string>(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                                new string[] { "@Method", "@PONumber", "@PartnerCode", "@ProjectCode", "@UserPONumber", "@POStatus",
                                               "@TotalPrice","@PurchaseDate","@PurchaseUserId","@ArrivalRequestDate","@CreateBy",
                                               //purMaster over sea
                                               "@OrderConfirmNumber","@HullNo","@BusinessType","@ConnectionToDemand","@Yard","@Schedule","@Mon",
                                               "@SPPR","@SPPriceRef","@RequestShipMode","@FinalShipmentMode","@BLCode","@Invoice","@InvoiceIssuedDate",
                                               "@PartnerUser","@Packing","@RefNumber","@TermDelivery","@TermPayment","@RealArrivalReqDate"
                                },
                                new object[] {"InsertMasterPO",PORequest.PONumber,PORequest.PartnerCode,PORequest.ProjectCode,
                                               PORequest.UserPONumber,PORequest.StatusCode,totalPrice,DateTime.Now,UserId,PORequest.ArrivalRequestDate,UserId,
                                               //over sea
                                               PORequest.OrderConfirmNumber,PORequest.HullNo,PORequest.BusinessType,PORequest.ConnectionToDemand,
                                               PORequest.Yard,PORequest.Schedule,PORequest.Mon,PORequest.SPPR,PORequest.SPPriceRef,
                                               PORequest.RequestShipMode,PORequest.FinalShipmentMode,PORequest.BLCode,PORequest.Invoice,
                                               PORequest.InvoiceIssuedDate,PORequest.PartnerUser,PORequest.Packing,PORequest.RefNumber,
                                               PORequest.TermDelivery,PORequest.TermPayment,PORequest.RealArrivalReqDate
                                }, transaction);
                            if (PONumberNew == null)
                            {
                                return new Result
                                {
                                    Success = false,
                                    Message = MessageCode.MD0005,
                                    Data = PONumberNew

                                };
                            }
                            else
                            {
                                using (var conn1 = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                                {
                                    string listDataSaveJs = JsonConvert.SerializeObject(listDetails);
                                    var resultUpdateDtl = -1;
                                    resultUpdateDtl = conn1.ExecuteScalar<int>(SP_MES_POREQUEST_DETAIL,
                                                new string[] { "@Method", "@PONumber", "@ProjectCode", "@Status", "@UserId", "@UserPONumber", "@listDetails" },
                                                new object[] { "UpdateDetailPOChange", PONumberNew, PORequest.ProjectCode, PORequest.StatusCode, UserId, PORequest.UserPONumber, listDataSaveJs });
                                    if (resultUpdateDtl > 0)
                                    {
                                        // If save detail true
                                        // Commit Mst
                                        transaction.Commit();
                                        // save success
                                        return new Result
                                        {
                                            Success = true,
                                            Message = MessageCode.MD0004,
                                            Data = PONumberNew
                                        };
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
                            }
                        }
                        // Edit PO 
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update"))
                        {
                            PORequest.StatusCode = "ORST06";//created
                                                            //when update or request
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                            new string[] { "@Method", "@PONumber","@ProjectCode","@PartnerCode", "@UserPONumber", "@POStatus",
                                                "@TotalPrice","@PurchaseDate","@PurchaseUserId","@ArrivalRequestDate","@UserModify",
                                                //purMaster over sea
                                               "@OrderConfirmNumber","@HullNo","@BusinessType","@ConnectionToDemand","@Yard","@Schedule","@Mon",
                                               "@SPPR","@SPPriceRef","@RequestShipMode","@FinalShipmentMode","@BLCode","@Invoice","@InvoiceIssuedDate",
                                               "@PartnerUser","@Packing","@RefNumber","@TermDelivery","@TermPayment","@RealArrivalReqDate"
                           },
                            new object[] {"UpdateMasterPO",PORequest.PONumber,PORequest.ProjectCode,PORequest.PartnerCode,
                                               PORequest.UserPONumber,PORequest.StatusCode,totalPrice,DateTime.Now,
                                               UserId,PORequest.ArrivalRequestDate,PORequest.UserModify ?? UserId,
                                               //over sea
                                               PORequest.OrderConfirmNumber,PORequest.HullNo,PORequest.BusinessType,PORequest.ConnectionToDemand,
                                               PORequest.Yard,PORequest.Schedule,PORequest.Mon,PORequest.SPPR,PORequest.SPPriceRef,
                                               PORequest.RequestShipMode,PORequest.FinalShipmentMode,PORequest.BLCode,PORequest.Invoice,
                                               PORequest.InvoiceIssuedDate,PORequest.PartnerUser,PORequest.Packing,PORequest.RefNumber,
                                               PORequest.TermDelivery,PORequest.TermPayment,PORequest.RealArrivalReqDate
                           }, transaction);
                            // Quan add 2020/09/25                          
                            if (updateMST > 0)
                            {
                                using (var conn1 = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                                {
                                    string listDataSaveJs = JsonConvert.SerializeObject(listDetails);
                                    var resultUpdateDtl = -1;
                                    resultUpdateDtl = conn1.ExecuteScalar<int>(SP_MES_POREQUEST_DETAIL,
                                              new string[] { "@Method", "@PONumber", "@ProjectCode", "@Status", "@UserId", "@UserPONumber", "@listDetails" },
                                              new object[] { "UpdateDetailPOChange", PORequest.PONumber, PORequest.ProjectCode, PORequest.StatusCode, UserId, PORequest.UserPONumber, listDataSaveJs });
                                    if (resultUpdateDtl > 0)
                                    {
                                        transaction.Commit();
                                        // save success
                                        return new Result
                                        {
                                            Success = true,
                                            Message = MessageCode.MD0004,
                                            Data = PORequest.PONumber
                                        };
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
                            }
                        }
                        // when Request
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update-Request"))
                        {
                            PORequest.StatusCode = "ORST01";
                            string remark = PORequest.RemarkYN == "1" || PORequest.RemarkYN == "3" ? "2" : "0";
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                               new string[] { "@Method", "@PONumber", "@ProjectCode", "@PartnerCode", "@UserPONumber", "@POStatus", "@TotalPrice", "@PurchaseDate", "@PurchaseUserId", "@ArrivalRequestDate", "@Reject" },
                               new object[] {"UpdateMasterPO-Request",PORequest.PONumber,PORequest.ProjectCode,PORequest.PartnerCode,PORequest.UserPONumber,PORequest.StatusCode,totalPrice,DateTime.Now,UserId,PORequest.ArrivalRequestDate, remark = "0"
                               }, transaction);
                            if (updateMST > 0)
                            {
                                string listDataSaveJs = JsonConvert.SerializeObject(listDetails);

                                var resultUpdateDtl = 0;
                                foreach (var item in listDetails)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                            new string[]
                                            {
                                                "@Method","@PONumber","@ProjectCode","@ItemCode","@POQty","@ItemPrice","@Amt","@Tax","@TotalPrice",
                                                "@DeliverQty", "@LeadTimeType","@LeadTime","@Status","@PleigerRemark","@PleigerRemark2","@ArrivalRequestDate","@PORemark","@PartnerAcceptor","@PrintingStatus","@listDetails","@UserId"
                                            },
                                            new object[]
                                            {
                                                "UpdateDetailPOChangePO",PORequest.PONumber,PORequest.ProjectCode,item.ItemCode,item.POQty,item.ItemPrice,item.Amt,item.Tax,totalPrice,
                                                item.DeliverQty,item.LeadTimeType,item.LeadTime,PORequest.StatusCode,item.PleigerRemark,item.PleigerRemark2,item.ArrivalRequestDate,item.PORemark,item.PartnerAcceptor,true,listDataSaveJs,UserId
                                                //"UpdateDetailPO",PORequest.PONumber,PORequest.ProjectCode,item.ItemCode,item.POQty,item.ItemPrice,item.Amt,item.Tax,item.TotalPrice,
                                                //item.DeliverQty,item.LeadTimeType,item.LeadTime,PORequest.StatusCode,item.PleigerRemark,item.PORemark,item.PartnerAcceptor,true
                                           }, transaction);
                                }
                                if (resultUpdateDtl > 0)
                                {
                                    transaction.Commit();
                                    // save success
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004,
                                        Data = PORequest.PONumber
                                    };
                                }
                                else
                                {
                                    transaction.Rollback();
                                    // Save fail
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD0005

                                    };
                                }
                            }
                            else
                            {
                                return new Result
                                {
                                    // Update fail
                                    Success = false,
                                    Message = "Update Fail !"
                                };
                            }
                        }
                        //when accept
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update-Accept"))
                        {
                            PORequest.StatusCode = "ORST02";//accept
                          
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                                new string[] { "@Method", "@PONumber", "@ProjectCode", "@POStatus", "@AcceptDate", "@UserIDAccepted" },
                                new object[] {"UpdateMasterPO-Accept",PORequest.PONumber,PORequest.ProjectCode,PORequest.StatusCode,DateTime.Now,UserId
                                }, transaction);
                            if (updateMST > 0)
                            {
                                var resultUpdateDtl = 0;
                                foreach (var item in listDetails)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode", "@Status", "@PrintingStatus", "@DeliveryDate", "@PlanCompleteDate", "@PORemark" },
                                            new object[] {"UpdateDetailPO-Accept",PORequest.PONumber,PORequest.ProjectCode,item.ItemCode,PORequest.StatusCode,false,item.DeliveryDate,
                                                item.PlanCompleteDate,item.PORemark
                                            }, transaction);
                                }
                                if (resultUpdateDtl > 0)
                                {
                                    transaction.Commit();
                                    // save success
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004,
                                        Data = PORequest.PONumber
                                    };
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
                            else
                            {
                                transaction.Rollback();
                                return new Result
                                {
                                    // Update fail
                                    Success = false,
                                    Message = "Accept Fail !"
                                };
                            }
                        }
                        // when reject
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("Update-Reject"))
                        {
                            PORequest.StatusCode = "ORST03";//reject
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                                 new string[] { "@Method", "@PONumber", "@ProjectCode", "@POStatus", "@RejectDate", "@UserIDReject" },
                                 new object[] {"UpdateMasterPO-Reject",PORequest.PONumber,PORequest.ProjectCode,PORequest.StatusCode,DateTime.Now,UserId
                                 }, transaction);

                            if (updateMST > 0)
                            {
                                var resultUpdateDtl = 0;
                                foreach (var item in listDetails)
                                {
                                    resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                            new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode", "@Status", "@PrintingStatus", "@DeliveryDate", "@PlanCompleteDate", "@PORemark" },
                                            new object[] {"UpdateDetailPO-Reject",PORequest.PONumber,PORequest.ProjectCode,
                                             item.ItemCode,PORequest.StatusCode,false,item.DeliveryDate,item.PlanCompleteDate,item.PORemark
                                            }, transaction);
                                    int resultUpdateRejectDtl = conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                                        new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode", "@PORemark" },
                                        new object[] { "UpdateItemPartnerInPurchaseDetail",PORequest.PONumber,PORequest.ProjectCode,item.ItemCode,item.PORemark
                                        }, transaction);
                                }
                                if (resultUpdateDtl > 0)
                                {
                                    transaction.Commit();
                                    // save success
                                    return new Result
                                    {
                                        Success = true,
                                        Message = MessageCode.MD0004,
                                        Data = PORequest.PONumber
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
                            else
                            {
                                transaction.Rollback();
                                return new Result
                                {
                                    Success = false,
                                    Message = "Reject Fail !"
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
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }
        public List<MES_SaleProject> SearchProjectCodeByParams(string itemCode, string itemName, string productionProjectCode, string productionProjectName, string projectType)
        {

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[6];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemCode";
                arrParams[2] = "@ItemName";
                arrParams[3] = "@ProductionProjectCode";
                arrParams[4] = "@ProductionProjectName";
                arrParams[5] = "@ProjectType";
                object[] arrParamsValue = new object[6];
                arrParamsValue[0] = "GetListProjectCodeByStatus";
                arrParamsValue[1] = itemCode;
                if (!String.IsNullOrEmpty(itemName))
                {
                    arrParamsValue[2] = Regex.Replace(itemName, Utils.PleigerConstant.REGEX_REPLACE_DATA_SEARCH, "_");
                }
                else
                {
                    arrParamsValue[2] = itemName;
                }
                arrParamsValue[3] = productionProjectCode;
                arrParamsValue[4] = productionProjectName;
                arrParamsValue[5] = projectType;
                var data = conn.ExecuteQuery<MES_SaleProject>("SP_MES_SALEPROJECT", arrParams, arrParamsValue).ToList();
                int i = 1;
                data.ForEach(x => x.No = i++);
                return data;
            }
        }
        public Result UpdateStatusPartnerToClosePopup(string poNumber, string flag)
        {

            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        var resultUpdateDtl = 0;
                        if(flag == "remark")
                        {
                            resultUpdateDtl = conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                               new string[] { "@Method", "@PONumber", "@Reject" },
                               new object[] { "UpdateStatusPoCloseNotifyRemark", poNumber, "3" }, transaction);
                        }
                        else if(flag == "day")
                        {
                            resultUpdateDtl = conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                               new string[] { "@Method", "@PONumber", "@ChangedYN" },
                               new object[] { "UpdateStatusPoCloseNotifyDay", poNumber, "3" }, transaction);
                        }
                        transaction.Commit();
                        if (resultUpdateDtl > 0)
                        {
                            // save success
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004,
                                Data = poNumber
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
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }
        public Result UpdateStatusPoChangedDayItem(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId)
        {
            string remark = flag == "Popup_daychanged" ? "1" : "2";
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(flag))
                        {
                            var resultUpdateDtl = 0;

                            foreach (var item in listDetails)
                            {
                                resultUpdateDtl = conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                                    new string[] { "@Method", "@PONumber", "@ProjectCode", "@PORemark", "@ChangedYN", "@StatusPoItem", "@ItemCode" },
                                    new object[] { "UpdateStatusPoChangedDayItem", PORequest.PONumber, PORequest.ProjectCode, item.PORemark, remark, "ORST06", item.ItemCode }, transaction);
                            }
                            transaction.Commit();
                            if (resultUpdateDtl > 0)
                            {
                                // save success
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0004,
                                    Data = PORequest.PONumber
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
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }
        public Result UpdateStatusPoRemarkItem(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId)
        {
            string remark = flag == "Popup_remark" ? "1" : "2";
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(flag))
                        {
                            var resultUpdateDtl = 0;
            
                            foreach (var item in listDetails)
                            {
                                resultUpdateDtl = conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                                    new string[] { "@Method", "@PONumber", "@ProjectCode", "@PORemark", "@Reject", "@StatusPoItem", "@ItemCode"},
                                    new object[] { "UpdateStatusPoItem",PORequest.PONumber,PORequest.ProjectCode, item.PORemark, remark, "ORST06", item.ItemCode}, transaction);
                            }
                            transaction.Commit();
                            if (resultUpdateDtl > 0)
                            {
                                // save success
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0004,
                                    Data = PORequest.PONumber
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
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }
        public Result UpdatePODetailPartner(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //edit po detail by partner 
                        if (!string.IsNullOrEmpty(flag) && flag.StartsWith("Update"))
                        {
                          
                            var resultUpdateDtl = 0;
                            foreach (var item in listDetails)
                            {
                                resultUpdateDtl = conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                                    new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode", "@DeliveryDate", "@PlanCompleteDate", "@PORemark", "@StatusPoItem" },
                                    new object[] { "UpdateItemPartnerInPurchaseDetail",PORequest.PONumber,PORequest.ProjectCode,
                                        item.ItemCode,item.DeliveryDate,item.PlanCompleteDate,item.PORemark, "ORST06"}, transaction);
                            }
                            transaction.Commit();
                            if (resultUpdateDtl > 0)
                            {
                                // save success
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0004,
                                    Data = PORequest.PONumber
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
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }
        public Result DeletePOMst(string PONumber)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //edit po detail by partner 

                        var resultUpdateDtl = 0;
                        resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                            new string[] { "@Method", "@PONumber" },
                            new object[] { "DeletePOMst", PONumber }, transaction);
                        if (resultUpdateDtl > 0)
                        {
                            transaction.Commit();
                            // save success
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004,
                                //Data = PORequest.PONumber
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                // Save fail
                                Success = false,
                                //Message = MessageCode.MD0005
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
        }

        public MES_Partner getPartnerByPartnerCode(string partnerCode)
        {
            var result = new MES_Partner();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@PartnerCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetPartnerByPartnerCode";
                arrParamsValue[1] = partnerCode;
                var data = conn.ExecuteQuerySingle<MES_Partner>("SP_MES_PARTNER", CommandType.StoredProcedure, arrParams, arrParamsValue);

                result = data;
            }
            return result;
        }

        public List<MES_SaleProject> GetProjectCodeByStatus()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListProjectCodeByStatus";
                var data = conn.ExecuteQuery<MES_SaleProject>("SP_MES_SALEPROJECT", arrParams, arrParamsValue).ToList();
                return data;
            }
        }
        public Result POClose(string PONumber)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //edit po detail by partner 

                        var resultUpdateDtl = 0;
                        resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                            new string[] { "@Method", "@PONumber" },
                            new object[] { "POClose", PONumber }, transaction);
                        if (resultUpdateDtl > 0)
                        {
                            transaction.Commit();
                            // save success
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004,
                                //Data = PORequest.PONumber
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                // Save fail
                                Success = false,
                                //Message = MessageCode.MD0005
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "PO Close not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
        }

        public Result CheckStatusPOClose(string PONumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "CheckStatusPOClose";
                arrParamsValue[1] = PONumber;
                var data = conn.ExecuteQuery<MES_ItemPO>(SP_MES_POREQUEST_MASTER, arrParams, arrParamsValue).ToList();
                if (data.Count > 0)
                {
                    return new Result
                    {
                        Success = false
                    };
                }
                else
                {
                    return new Result
                    {
                        Success = true
                    };
                }
            }
        }
        public string GetPOStatus(string PONumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetPOStatus";
                arrParamsValue[1] = PONumber;
                var data = conn.ExecuteScalar<string>(SP_MES_POREQUEST_MASTER, arrParams, arrParamsValue);
                return data;
            }
        }
        

        #endregion

        public List<DynamicCombobox> GetListPartnerCombobox()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListPartnerCombobox";
                var data = conn.ExecuteQuery<DynamicCombobox>(SP_Name, arrParams, arrParamsValue).ToList();
                return data;
            }
        }

        public Result CoppyPO(string PoNumber, string userId)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //edit po detail by partner 

                        var resultUpdateDtl = 0;
                        resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                            new string[] { "@Method", "@PONumber" , "@UserID" },
                            new object[] { "CoppyPo", PoNumber, userId }, transaction);
                        if (resultUpdateDtl > 0)
                        {
                            transaction.Commit();
                            // save success
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD0004,
                                //Data = PORequest.PONumber
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                // Save fail
                                Success = false,
                                //Message = MessageCode.MD0005
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
        }

        public Result UpdateRemakAfterConfirmed(string PoNumber, string Remark)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var resultUpdateDtl = 0;
                        resultUpdateDtl = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                            new string[] { "@Method", "@PONumber" , "@RemarkAfterConfrimed" },
                            new object[] { "UpdateRemakAfterConfirmed", PoNumber, Remark }, transaction);
                        if (resultUpdateDtl > 0)
                        {
                            transaction.Commit();
                            // save success
                            return new Result
                            {
                                Success = true,
                                Message = MessageCode.MD00017,
                                
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new Result
                            {
                                // Save fail
                                Success = false,
                                Message = MessageCode.MD0009
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
        }


        
        public List<MES_PurchaseDetail> GetListPurcharDetail(string poNumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PONumber";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListPurcharDetail";
                arrParamsValue[1] = poNumber;
                var data = conn.ExecuteQuery<MES_PurchaseDetail>(SP_Name, arrParams, arrParamsValue);
                return data.ToList();
            }
        }

        public Result UpdatePODetailPartnerETA(MES_PORequest PORequest, List<MES_ItemPO> listDetails, string UserId)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                            var resultUpdateDtl = 0;
                            foreach (var item in listDetails)
                            {
                                resultUpdateDtl = conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                                    new string[] { "@Method", "@PONumber","@ItemCode", "@PlanCompleteDate"},
                                    new object[] { "UpdateItemPartnerInPurchaseDetailETA",PORequest.PONumber,item.ItemCode,item.PlanCompleteDate}, transaction);
                            }
                            transaction.Commit();
                            if (resultUpdateDtl > 0)
                            {
                                // save success
                                return new Result
                                {
                                    Success = true,
                                    Message = MessageCode.MD0004,
                                    Data = PORequest.PONumber
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
                            Message = "Request data not success! + Exception: " + ex.ToString(),
                        };
                    }
                }
            }
            return result;
        }

    }
}

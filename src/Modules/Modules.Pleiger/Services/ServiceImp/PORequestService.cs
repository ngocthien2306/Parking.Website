using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using Modules.Pleiger.Utils;
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

namespace Modules.Pleiger.Services.ServiceImp
{
    public class PORequestService : IPORequestService
    {
        //private string SP_Name = "sp_MES_PORequest";
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
            string UserProjectCode, string requestDateFrom, string requestDateTo, string partnerCode,string poStatus)
        {
            var result = new List<MES_PORequest>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var methodName = "";
                if (partnerCode != null && partnerCode != "")
                {
                    methodName = "SearchListPORequestByPartner";
                }
                else
                {
                    methodName = "SearchListPORequest";
                }
                string[] arrParams = new string[9];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@PONumber";
                arrParams[3] = "@RequestDateFrom";
                arrParams[4] = "@RequestDateTo";
                arrParams[5] = "@UserPONumber";
                arrParams[6] = "@UserProjectCode";
                arrParams[7] = "@PartnerCode";
                arrParams[8] = "@POStatus";
                object[] arrParamsValue = new object[9];
                arrParamsValue[0] = methodName;
                arrParamsValue[1] = projectCode;
                arrParamsValue[2] = poNumber;
                arrParamsValue[3] = requestDateFrom;
                arrParamsValue[4] = requestDateTo;
                arrParamsValue[5] = UserPONumber;
                arrParamsValue[6] = UserProjectCode;
                arrParamsValue[7] = partnerCode;
                arrParamsValue[8] = poStatus;
                var data = conn.ExecuteQuery<MES_PORequest>(SP_Name, arrParams, arrParamsValue);

                result = data.ToList();
            }

            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }

        // Get PO Detail
        public MES_PORequest GetPODetail(string poNumber)
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
                        POSheet.Cells[$"I{row}"].Value = s.Unit;
                        POSheet.Cells[$"J{row}"].Value = s.Curency;
                        POSheet.Cells[$"K{row}"].Value = s.UnitPrice;
                        POSheet.Cells[$"M{row}"].Value = s.Total;
                        POSheet.Cells[$"B{row + 1}"].Value = s.ItemName;
                        POSheet.Cells[$"B{row + 2}"].Value = "Remark 1: ";
                        POSheet.Cells[$"B{row + 3}"].Value = "Remark 2: ";
                        POSheet.Cells[$"D{row + 2}"].Value = s.PORemark;
                        POSheet.Cells[$"D{row + 3}"].Value = s.PleigerRemark;
                        POSheet.Cells[$"B{row + 4}"].Value = "Delivery Request : ";
                        POSheet.Cells[$"E{row + 4}"].Value = DateTime.Parse(s.DeliveryRequestDate.ToString()).ToString("yyyy-MM-dd");

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

                excel.Workbook.Worksheets.Delete("Footer");

                newExcelFile = new FileInfo(tempFilePath + excelFileName);
                excel.SaveAs(newExcelFile);

                Workbook workbook = new Workbook();
                //Load excel file  
                workbook.LoadFromFile(tempFilePath + excelFileName);

                pdfFileName = tempFilePath + excelFileName.Remove(excelFileName.LastIndexOf('.')) + ".pdf";
                //Save excel file to pdf file.  
                workbook.SaveToFile(pdfFileName, Spire.Xls.FileFormat.PDF);
                //UPDATE PRINTING STATUS AFTER PRINT
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                      var result = -1;
                      result = conn.ExecuteNonQuery(SP_MES_POREQUEST_DETAIL, CommandType.StoredProcedure,
                                         new string[]
                                         { "@Method","@PONumber","@PrintingStatus"},
                                         new object[]
                                         { "ChangePrintingStatus",poNumber,true}, transaction);
                    if(result > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                    }
                }


                //////////

            }
            Debug.WriteLine("PVN Test 4");
            Debug.WriteLine("GetPODetailData Run At " + DateTime.Now);

            return pdfFileName;
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
                            int updateMST = conn.ExecuteNonQuery(SP_MES_POREQUEST_MASTER, CommandType.StoredProcedure,
                               new string[] { "@Method", "@PONumber", "@ProjectCode", "@PartnerCode", "@UserPONumber", "@POStatus", "@TotalPrice", "@PurchaseDate", "@PurchaseUserId" },
                               new object[] {"UpdateMasterPO-Request",PORequest.PONumber,PORequest.ProjectCode,PORequest.PartnerCode,PORequest.UserPONumber,PORequest.StatusCode,totalPrice,DateTime.Now,UserId
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
                                                "@DeliverQty", "@LeadTimeType","@LeadTime","@Status","@PleigerRemark","@PORemark","@PartnerAcceptor","@PrintingStatus","@listDetails"
                                            },
                                            new object[]
                                            {
                                                "UpdateDetailPOChangePO",PORequest.PONumber,PORequest.ProjectCode,item.ItemCode,item.POQty,item.ItemPrice,item.Amt,item.Tax,totalPrice,
                                                item.DeliverQty,item.LeadTimeType,item.LeadTime,PORequest.StatusCode,item.PleigerRemark,item.PORemark,item.PartnerAcceptor,true,listDataSaveJs
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
                                            new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode", "@Status", "@PrintingStatus", "@DeliveryDate" ,"@PlanCompleteDate" ,"@PORemark"  },
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
                                            new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode", "@Status", "@PrintingStatus", "@DeliveryDate","@PlanCompleteDate","@PORemark" },
                                            new object[] {"UpdateDetailPO-Reject",PORequest.PONumber,PORequest.ProjectCode,
                                             item.ItemCode,PORequest.StatusCode,false,item.DeliveryDate,item.PlanCompleteDate,item.PORemark
                                            }, transaction);
                                    int resultUpdateRejectDtl =  conn.ExecuteNonQuery("SP_MES_PARTNER_GET_ALL", CommandType.StoredProcedure,
                                        new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode" ,"@PORemark"},
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
        public List<MES_SaleProject> SearchProjectCodeByParams(string projectCode, string itemCode, string itemName)
        {

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                arrParams[2] = "@ItemCode";
                arrParams[3] = "@ItemName";
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetListProjectCodeByStatus";
                arrParamsValue[1] = projectCode;
                arrParamsValue[2] = itemCode;
                //arrParamsValue[3] = itemName;
                
                if (!String.IsNullOrEmpty(itemName))
                {
                    arrParamsValue[3] = Regex.Replace(itemName, PleigerConstant.REGEX_REPLACE_DATA_SEARCH, "_");
                }
                else
                {
                    arrParamsValue[3] = itemName;
                }
                var data = conn.ExecuteQuery<MES_SaleProject>("SP_MES_SALEPROJECT", arrParams, arrParamsValue).ToList();
                int i = 1;
                data.ForEach(x => x.No = i++);
                return data;
            }
        }

        public Result UpdatePODetailPartner(string flag, MES_PORequest PORequest, List<MES_ItemPO> listDetails)
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
                                    new string[] { "@Method", "@PONumber", "@ProjectCode", "@ItemCode", "@DeliveryDate" ,"@PlanCompleteDate", "@PORemark" },
                                    new object[] { "UpdateItemPartnerInPurchaseDetail",PORequest.PONumber,PORequest.ProjectCode,
                                        item.ItemCode,item.DeliveryDate,item.PlanCompleteDate,item.PORemark}, transaction);
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

        #endregion


    }
}

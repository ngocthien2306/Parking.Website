using DevExtreme.AspNet.Mvc;
using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    public class MESProductStatusService : IMESProductStatusService
    {
        private const string SP_MES_PRODUCTION_STATUS = "SP_MES_PRODUCTION_STATUS";
        private const string SP_MES_SALEPROJECT_Test = "SP_MES_SALEPROJECT_Test";

        //public List<MES_ProductStatus> GetProductStatus()
        //{
        //    var result = new List<MES_ProductStatus>();
        //    using(var connect = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
        //    {
        //        var data = connect.ExecuteQuery<MES_ProductStatus>(
        //            SP_MES_PRODUCTION_STATUS, null, null);
        //        result = data.ToList();
        //    }
        //    return result;
        //}
        public List<MES_ProductStatus> GetProductStatus(DataSourceLoadOptions sourceLoadOptions, MES_ProductStatus productStatus)
        {
            var result = new List<MES_ProductStatus>();

            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[6];
                arrParams[0] = "@UserProjectCode";
                arrParams[1] = "@ItemCode";
                arrParams[2] = "@ProductLineCode";
                arrParams[3] = "@ItemName";
                arrParams[4] = "@ProjectOrderType";
                arrParams[5] = "@SaleOrderProjectCode";
                object[] arrParamsValue = new object[6];
                arrParamsValue[0] = productStatus.UserProjectCode;
                arrParamsValue[1] = productStatus.ItemCode;
                arrParamsValue[2] = productStatus.ProdcnLineCode;
                arrParamsValue[3] = productStatus.NameKor;
                arrParamsValue[4] = productStatus.ProjectOrderType;
                arrParamsValue[5] = productStatus.SalesOrderProjectCode;

                var data = conn.ExecuteQuery<MES_ProductStatus>(SP_MES_PRODUCTION_STATUS, arrParams, arrParamsValue);
                result = data.ToList();


            }
            int i = 1;
            result.ForEach(x => x.No = i++);

            return result;
        }
        public List<MES_ProductStatus> GetDataExportExcelICube(string jsonObj)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@listProjectCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "SalesProjectGetDataExportExcel";
                arrParamsValue[1] = jsonObj;
                var result = conn.ExecuteQuery<MES_ProductStatus>(SP_MES_SALEPROJECT_Test, arrParams, arrParamsValue);

                return result.ToList();
            }
        }

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
    } 

    /*
        public partial class DataItemSample
        {
        public static readonly IEnumerable<ProductTest> DataGrid = new[]
            {
                new ProductTest
                {
                    ID = 1,
                    UserProjectCode = "TEST1",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                 new ProductTest
                {
                    ID = 2,
                    UserProjectCode = "TEST2",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                 new ProductTest
                {
                    ID = 3,
                    UserProjectCode = "TEST3",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 4,
                    UserProjectCode = "TEST4",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 5,
                    UserProjectCode = "TEST5",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 6,
                    UserProjectCode = "TEST6",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 7,
                    UserProjectCode = "TEST7",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 8,
                    UserProjectCode = "TEST8",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 9,
                    UserProjectCode = "TEST9",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 10,
                    UserProjectCode = "TEST10",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                },
                new ProductTest
                {
                    ID = 11,
                    UserProjectCode = "TEST11",
                    ProductLineCode = "AB1212C",
                    ProductLineName = "HCM",
                    CodeItem = "121231313",
                    NameItem = "Table"
                }

            };

        }
        public partial class SampleData
        {
            public static readonly IEnumerable<Employee> DataGrid = new[]
            {
                new Employee
                {
                    ID = 1,
                    FirstName = "John",
                    LastName = "Heart",
                    Phone = "(213) 555-9392",
                    Prefix = "Mr.",
                    Position = "CEO",
                    BirthDate = DateTime.Parse("1964/03/16"),
                    HireDate = DateTime.Parse("1995/01/15"),
                    Notes = "John has been in the Audio/Video industry since 1990. He has led DevAv as its CEO since 2003.\r\n\r\nWhen not working hard as the CEO, John loves to golf and bowl. He once bowled a perfect game of 300.",
                    Email = "jheart@dx-email.com",
                    Address = "351 S Hill St.",
                    City = "Los Angeles"
                },
                new Employee
                {
                    ID = 2,
                    FirstName = "Olivia",
                    LastName = "Peyton",
                    Phone = "(310) 555-2728",
                    Prefix = "Mrs.",
                    Position = "Sales Assistant",
                    BirthDate = DateTime.Parse("1981/06/03"),
                    HireDate = DateTime.Parse("2012/05/14"),
                    Notes = "Olivia loves to sell. She has been selling DevAV products since 2012. \r\n\r\nOlivia was homecoming queen in high school. She is expecting her first child in 6 months. Good Luck Olivia.",
                    Email = "oliviap@dx-email.com",
                    Address = "807 W Paseo Del Mar",
                    City = "Los Angeles"
                },
                new Employee
                {
                     ID = 3,
                    FirstName = "Robert",
                    LastName = "Reagan",
                    Phone = "(818) 555-2387",
                    Prefix = "Mr.",
                    Position = "CMO",
                    BirthDate = DateTime.Parse("1974/09/07"),
                    HireDate = DateTime.Parse("2002/11/08"),
                    Notes = "Robert was recently voted the CMO of the year by CMO Magazine. He is a proud member of the DevAV Management Team.\r\n\r\nRobert is a championship BBQ chef, so when you get the chance ask him for his secret recipe.",
                    Address = "4 Westmoreland Pl.",
                    City = "Bentonville",
                }
            };
        }
    */

}

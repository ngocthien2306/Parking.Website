using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace InfrastructureCore.Helpers
{
    public class ExcelHelperTest
    {
        public DataTable ReadFromExcelfile(string path, string sheetName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Khởi tạo data table
            DataTable dt = new DataTable();
            // Load file excel và các setting ban đầu
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count <= 1)
                {
                    // Log - Không có sheet nào tồn tại trong file excel của bạn return null; } 
                    // Lấy Sheet đầu tiện trong file Excel để truy vấn 
                    // Truyền vào name của Sheet để lấy ra sheet cần, nếu name = null thì lấy sheet đầu tiên 
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName) ?? package.Workbook.Worksheets.FirstOrDefault();
                    // Đọc tất cả các header
                    foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                    {



                        dt.Columns.Add(firstRowCell.Text);
                    }
                    // Đọc tất cả data bắt đầu từ row thứ 2
                    for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        // Lấy 1 row trong excel để truy vấn
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        // tạo 1 row trong data table
                        var newRow = dt.NewRow();
                        foreach (var cell in row)
                        {
                            if (cell.Start.Column - 1 < dt.Columns.Count)
                            {
                                newRow[cell.Start.Column - 1] = cell.Text;
                            }
                            //cell.Style.Numberformat.NumFmtID;
                        }
                        dt.Rows.Add(newRow);
                    }
                }
                return dt;
               // return JsonConvert.SerializeObject(dt);
            }
        }

        public DataTable ReadFromExcelfile(string path, string sheetName, Type type)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Khởi tạo data table
            DataTable dt = new DataTable();
            // Load file excel và các setting ban đầu
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count <= 2)
                {
                    // Log - Không có sheet nào tồn tại trong file excel của bạn return null; } 
                    // Lấy Sheet đầu tiện trong file Excel để truy vấn 
                    // Truyền vào name của Sheet để lấy ra sheet cần, nếu name = null thì lấy sheet đầu tiên 
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault(x => x.Name == sheetName) ?? package.Workbook.Worksheets.FirstOrDefault();
                    // Đọc tất cả các header

                    if(type != null)
                    {
                        foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                        {
                            MemberInfo[] myMembers = type.GetMembers();
                            for (int i = 0; i < myMembers.Length; i++)
                            {
                                if (myMembers[i].CustomAttributes.Any() == true)
                                {
                                    foreach (var item in myMembers[i].CustomAttributes)
                                    {
                                        if (item.AttributeType.Name == "ColumNameAttribute" && firstRowCell.Text.Trim() == item.ConstructorArguments[0].Value.ToString())
                                        {
                                            dt.Columns.Add(myMembers[i].Name);
                                        }
                                    }
                                }
                            }
                            //dt.Columns.Add(firstRowCell.Text);
                        }
                    }
                    else
                    {
                        foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                        {
                            dt.Columns.Add(firstRowCell.Text);
                        }
                    }
                    // Đọc tất cả data bắt đầu từ row thứ 2
                    for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        // Lấy 1 row trong excel để truy vấn
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        // tạo 1 row trong data table
                        var newRow = dt.NewRow();
                        foreach (var cell in row)
                        {
                            if (cell.Start.Column - 1 < dt.Columns.Count)
                            {
                                newRow[cell.Start.Column - 1] = cell.Text;
                            }
                            //cell.Style.Numberformat.NumFmtID;
                        }

                        int columnHasData = 0;
                        
                        foreach (var item in newRow.ItemArray)
                        {
                            if(!string.IsNullOrEmpty(item.ToString()))
                            {
                                columnHasData++;
                            }
                        }

                        if(columnHasData != 0)
                        {
                            dt.Rows.Add(newRow);
                        }
                    }
                }
                return dt;
                // return JsonConvert.SerializeObject(dt);
            }
        }

        public static DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }
    }
}

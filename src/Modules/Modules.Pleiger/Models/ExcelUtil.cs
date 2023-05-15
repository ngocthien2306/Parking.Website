using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Modules.Pleiger.Models
{
    public class ExcelUtil
    {
        //        public List<MES_Purchase> ReadFromExcel(IFormFile myFile, string path,string sheetName)
        //        {
        //            DataTable dt = new DataTable();
        //            // Load file excel và các setting ban đầu
        //            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
        //            {
        //                if (package.Workbook.Worksheets.Count < 1) { // Không có sheet nào tồn tại trong file excel của bạn return null; } // 
        //                    //Lấy Sheet đầu tiện trong file Excel để truy vấn // Truyền vào name của Sheet để lấy ra sheet cần, nếu name = null thì lấy sheet đầu tiên 
        //                    ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault(x = &amp; gt; x.Name == sheetName)?? package.Workbook.Worksheets.FirstOrDefault();
        //                    // Đọc tất cả các header
        //                    foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
        //                    {
        //                        dt.Columns.Add(firstRowCell.Text);
        //                    }
        //                    // Đọc tất cả data bắt đầu từ row thứ 2
        //                    for (var rowNumber = 2; rowNumber & amp; lt;= workSheet.Dimension.End.Row; rowNumber++)
        //{
        //                        // Lấy 1 row trong excel để truy vấn
        //                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
        //                        // tạo 1 row trong data table
        //                        var newRow = dt.NewRow();
        //                        foreach (var cell in row)
        //                        {
        //                            newRow[cell.Start.Column - 1] = cell.Text;
        //                        }
        //                        dt.Rows.Add(newRow);
        //                    }
        //                }
        //                return dt;
        //            }
        //    }
    }
}

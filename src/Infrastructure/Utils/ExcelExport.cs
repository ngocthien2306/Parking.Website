﻿using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InfrastructureCore.Utils
{
    public static class ExcelExport
    {
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                #region Big freaking list of mime types
                // combination of values from Windows 7 Registry and 
                // from C:\Windows\System32\inetsrv\config\applicationHost.config
                // some added, including .7z and .dat
                
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".pdf", "application/pdf"}
                #endregion        
            };
        }

        public static void AppendContentToFile(string path, IFormFile content)
        {
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                content.CopyTo(stream);
                CheckMaxFileSize(stream);
            }
        }

        private static void CheckMaxFileSize(FileStream stream)
        {
            //if (stream.Length > 20000000)
            if (stream.Length > 50000000000)

                throw new Exception("File is too large");
        }

        public static void ProcessUploadedFile(string tempFilePath, string fileName, string tempPath)
        {
            // Check if the uploaded file is a valid image
            //string curDate = DateTime.Today.ToString("yyyyMM");
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", curDate);
            System.IO.File.Copy(tempFilePath, Path.Combine(tempPath, fileName));
        }

        public static void RemoveTempFilesAfterDelay(string path, TimeSpan delay)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
                //foreach (var file in dir.GetFiles("*.tmp").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                foreach (var file in dir.GetFiles("*.tmp"))
                    file.Delete();
        }


        public static string ExportDataToExcel(DataTable dt, string fileName, string templatePath, string savePath, string sheetName = "Sheet1")
        {
            var dateFormart = "yyyy-MM-dd";

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            string excelFileName = $"{fileName}{DateTime.Now.ToString("yyyyMMddhhmmss")}.xlsx"; ;
            FileInfo newExcelFile;
            FileInfo excelTemplate = new FileInfo(templatePath);

            var tempFilePath = Path.Combine(Directory.GetCurrentDirectory(), savePath);
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);

            using (var excel = new ExcelPackage(excelTemplate))
            {
                Stream excelStream = excel.Stream;
                ExcelWorksheet templateSheet = excel.Workbook.Worksheets[sheetName];

                templateSheet.Cells.LoadFromDataTable(dt, true);

                int colNumber = 1;

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.DataType == typeof(DateTime))
                    {
                        //workSheet.Column(colNumber).Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss AM/PM";
                        templateSheet.Column(colNumber).Style.Numberformat.Format = dateFormart;
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

                templateSheet.Cells.AutoFitColumns();

                newExcelFile = new FileInfo(tempFilePath + excelFileName);
                excel.SaveAs(newExcelFile);

                return newExcelFile.FullName;
            }
        }
    }
}
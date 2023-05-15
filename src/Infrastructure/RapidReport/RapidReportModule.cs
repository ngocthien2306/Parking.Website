using iTextSharp.text.pdf;
using jp.co.systembase.json;
using jp.co.systembase.report;
using jp.co.systembase.report.data;
using jp.co.systembase.report.renderer.pdf;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace InfrastructureCore.RapidReport
{
    public class RapidReportModule
    {
        [STAThread]
        public string Run(DataTable dataTable, string rrptFileName, string outputFileName, string curLang)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                //PurchaseOrder
                //Report report = new Report(Json.Read(Path.Combine("rrpt", rrptFileName + "_" + curLang + ".rrpt")));
                string A = Path.Combine("rrpt", rrptFileName + "_" + curLang + ".rrpt");
                Report report = new Report(Json.Read(Path.Combine("rrpt", rrptFileName + "_" + curLang + ".rrpt")));
                report.Fill(new ReportDataSource(dataTable));
                ReportPages pages = report.GetPages();

                string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "out", rrptFileName); // WMS26
                if (!Directory.Exists(tempPath))
                    Directory.CreateDirectory(tempPath);
                string fileOutputlink = tempPath + "\\" + outputFileName + DateTime.Now.ToString("yyyyMMddHHMMsss") + ".pdf";
                using (FileStream fs = new FileStream(Path.Combine("out", fileOutputlink), FileMode.Create))
                {
                    // C #
                    PdfRendererSetting setting = new PdfRendererSetting();
                    //korea
                    setting.FontMap["malgun"] = BaseFont.CreateFont(Path.Combine(Directory.GetCurrentDirectory() + "\\assets\\font\\malgun.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // vietnam
                    //setting.FontMap["micross"] = BaseFont.CreateFont("C:\\Windows\\Fonts\\micross.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // china
                    PdfRenderer renderer = new PdfRenderer(fs, setting);
                    //// set font when export data
                    //switch (curLang)
                    //{
                    //    case "en":
                    //        renderer.Setting.DefaultFont = BaseFont.CreateFont(Path.Combine(Directory.GetCurrentDirectory() + "\\assets\\font\\arial.ttf"),
                    //                BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    //        break;
                    //    case "ko":
                    //        renderer.Setting.DefaultFont = BaseFont.CreateFont(Path.Combine(Directory.GetCurrentDirectory() + "\\assets\\font\\malgun.ttf"),
                    //                BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    //        break;
                    //    case "ja":
                    //        //default mincho / gothic
                    //        renderer.Setting.DefaultFont = BaseFont.CreateFont(Path.Combine(Directory.GetCurrentDirectory() + "\\assets\\font\\msgothic.ttf"),
                    //                BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    //        break;
                    //    case "zh":
                    //        break;
                    //    default:
                    //        renderer.Setting.DefaultFont = BaseFont.CreateFont(Path.Combine(Directory.GetCurrentDirectory() + "\\assets\\font\\arial.ttf"),
                    //                BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    //        break;
                    //}

                    pages.Render(renderer);
                }

                sw.Stop();
                return fileOutputlink;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

using InfrastructureCore.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Modules.Common.Utils
{
    public class DownloadFileUtils : ControllerBase
    {
        public FileStreamResult GetFileStreamResult(string downloadExcelPath)
        {
            //TODO: NOT DONE
            if(!string.IsNullOrEmpty(downloadExcelPath))
            {
                var memory = new MemoryStream();
                FileStreamResult fileStreamResult;
                using (var stream = new FileStream(downloadExcelPath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                fileStreamResult = File(memory, ExcelExport.GetContentType(downloadExcelPath), downloadExcelPath.Remove(0, downloadExcelPath.LastIndexOf("\\") + 1));

                return fileStreamResult;
            }
            return null;
        }

        public ActionResult DownloadFile(string fileLink, string fileName)
        {
            if (fileName != null)
            {
                //byte[] data = TempData[fileGuid] as byte[];
                //return File(fileName, "application/vnd.ms-excel", fileName);
                string Files = fileLink;
                byte[] fileBytes = System.IO.File.ReadAllBytes(Files);
                System.IO.File.WriteAllBytes(Files, fileBytes);
                MemoryStream ms = new MemoryStream(fileBytes);
                return File(ms, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }
    }
}

using InfrastructureCore;
using Microsoft.AspNetCore.Http;
using Modules.Pleiger.CommonModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Modules.Pleiger.FileUpload.Services.IService
{
    public interface IUploadFileWithTemplateService
    {
        void ImportDataToTemplateExcelFile(string pageName, string filePath);

        string UploadFileDynamicForImportFromPopup(IFormFile myFile, string chunkMetadata, Type type);
        //Result SaveToDB_SaleProject_Excel(string filePath, string SPName, string UserID);
        Result SaveToDB_Model_Excel(string filePath, string SPName, string UserID, string checkPage); // dat add 2021-1-17
        List<MES_ItemPO> Getdata_Model_Excel(string filePath, string SPName, string UserID, string checkPage, string PartnerCode); // dat add 2021-1-17
        
    }
}
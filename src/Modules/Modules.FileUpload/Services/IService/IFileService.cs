using InfrastructureCore;
using Microsoft.AspNetCore.Http;
using Modules.Common.Models;
using System.Collections.Generic;

namespace Modules.FileUpload.Services.IService
{
    public interface IFileService
    {
        int InsertSYFileUpload(SYFileUpload file);
        List<SYFileUpload> GetListSYFileUpload();
        int UpdateSYFileUpload(SYFileUpload file);
        int DeleteSYFileUpload(string fileGuid, string spName, string fileMst, string BoardDocID);
        SYFileUpload GetSYFileUploadByID(string fileGuid);
        List<SYFileUpload> GetListSYFileUploadByID(string fileGuid);
        int InsertSYFileUploadMaster(string fileID, string fileGuid, string filePath, int siteID);
        List<SYFileUploadMaster> GetSYFileUploadMasterByID(string fileId);

        Result SaveFile(IFormFile file, string destinationPath, string directory);
        SYFileUploadMaster GetSYFileUploadMasterByFileGuid(string fileGuid);
        Result InsertSYFileUploadPartner(string idFile, string FileGuid, string pagID, string spName);

    }
}

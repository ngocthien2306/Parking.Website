using InfrastructureCore;
using InfrastructureCore.DataAccess;
using Modules.UIRender.Models;
using InfrastructureCore.Models.Identity;
using Microsoft.AspNetCore.Http;
using Modules.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Modules.UIRender.Services.IService
{
    public partial interface IDynamicPageService
    {
        #region Get info page
        Task<List<PageElement>> GetInforPageLayout(string PageID);

        SYPageLayout GetInfoPage(int PageID);

        List<SYPageRelationship> GetRelationship(int PageID);
        #endregion

        #region Tool bar action
        Task<List<SYToolbarActions>> GetToolbarActionsWithPageID(int PAG_ID);

        List<SYToolbarActions> GetToolbarActionsNormalWithPageID(int PAG_ID);
        #endregion

        #region View component
        Task<List<SYPageLayout>> GetPageLayoutWithType(string type);
        #endregion

        #region Dynamic Render
        dynamic InitModelGridDynamic(string pelID, int pagID);

        SYPageLayElements GetPageElementsWithPELID(string pelID);
        #endregion

        #region Execute SP Dynamic Render
        dynamic ExecuteProc2(string procName, ICollection<SPParameter> paras, string connectionType);
        int ExecuteProc2Count(string procName, ICollection<SPParameter> paras, string connectionType);
        Result ExecuteProCRUD(string procName, ICollection<SPParameter> paras);
        #endregion
        object ExecuteSave(string objPostData, SYLoggedUser info);
        string GetJSPath(int pagID);

        #region EXCEL EXPORT
        string ExportExcelDynamic(DataTable dt, string fileName,String spname);

        #endregion

        #region
        Result UploadFileDynamicForImport(IFormFile myFile, Type type, string Lang, List<SPParameter> spParam, string SPName, string UserID);

        string UploadFileDynamicForImportFromPopup(IFormFile myFile, string chunkMetadata, Type type);

        Result SaveToDB_DynamicData_Excel(string filePath, string SPName, List<SPParameter> spParam, string UserID);

        List<SPParameter> GetListParam(int ActID, int PageID);

        #endregion

        DynamicPopupModel GetMenuImportAction(int PageID, int PageAct, string PageKey);
        void ImportDataToTemplateExcelFile(string pageName, string filePath);
    }
}

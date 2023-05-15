using InfrastructureCore;
using Modules.Admin.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Pleiger.Production.Services.IService
{
    public interface IMESMaterialIssueService
    {
        public List<MESMaterialIssue> searchMaterialIssue(string lang, string MaterialIssueNo, string StartDate, string EndDate, string UseTeam, string ProductionProjectCode, string MaterialIssueStatus);
        public MESMaterialIssue getMaterialIssueByNo(string MaterialIssueNo, string lang);
        public object saveMaterialIssue(string MaterialIssueNo, string MaterialIssueDate, string UseTeam, string MaterialIssueStatus, string BOMKey, string ItemCode, string ProductionProjectCode, string MaterialIssueCreator, string MaterialIssueComment, string CreateDate);
        public string updateStatus(string MaterialIssueNo, string MaterialIssueStatus);
        public List<DynamicCombobox> getProjectCodeCombobox();
        public List<MESIssueItemPart> GetBOMItem(string ItemCode);
        public string savePartList(string MaterialIssueNo, string Data, string ItemToDelete, string SlipNumber, string UserId);
        string UpdateItemCodeInWH(string Data, string id);
        public List<MESIssueItemPart> getMaterialIssuePartList(string MaterialIssueNo);
        public string partListExcelFileToJson(string filePath, string langCode);
        public string getItemNameByItemCode(string itemCode, string lang);

        // Thien add 2022-01-13
        Result UpdateItemToWH(string filePath, string id, string lang,string materialIssueNo);
    }
}

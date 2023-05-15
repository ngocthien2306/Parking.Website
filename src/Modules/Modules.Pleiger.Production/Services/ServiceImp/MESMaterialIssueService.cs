using InfrastructureCore;
using InfrastructureCore.DAL;
using InfrastructureCore.Helpers;
using InfrastructureCore.Utils;
using Modules.Admin.Models;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.Production.Model;
using Modules.Pleiger.Production.Services.IService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Modules.Pleiger.Production.Services.ServiceImp
{
    class MESMaterialIssueService : IMESMaterialIssueService
    {
        private const string SP_MES_MATERIALISSUE = "SP_MES_MATERIALISSUE";
        private const string SP_MES_MATERIALISSUE_PROJECT_CODE_COMBOBOX = "SP_MES_MATERIALISSUE_PROJECT_CODE_COMBOBOX";
        private const string SP_MES_MATERIALISSUE_IMPORT_PART_LIST = "SP_MES_MATERIALISSUE_IMPORT_PART_LIST";
        private const string SP_MES_MATERIALISSUE_IMPORT_PART_LIST_UPDATEWH = "SP_MES_MATERIALISSUE_IMPORT_PART_LIST_UPDATEWH";

        public MESMaterialIssue getMaterialIssueByNo(string MaterialIssueNo, string lang)
        {
            var rs = searchMaterialIssue(lang, MaterialIssueNo, "", "", "", "", "");
            if (rs.Count == 0) return new MESMaterialIssue();
            return rs[0];
        }

        public List<MESMaterialIssue> searchMaterialIssue(string lang, string MaterialIssueNo, string StartDate, string EndDate, string UseTeam, string ProductionProjectCode, string MaterialIssueStatus)
        {
            var result = new List<MESMaterialIssue>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[8];
                arrParams[0] = "@Method";
                arrParams[1] = "@Lang";
                arrParams[2] = "@MaterialIssueNo";
		        arrParams[3] = "@StartDate";
		        arrParams[4] = "@EndDate";
		        arrParams[5] = "@UseTeam";
		        arrParams[6] = "@MaterialIssueStatus";
		        arrParams[7] = "@ProductionProjectCode";
                object[] arrParamsValue = new string[8];
                arrParamsValue[0] = "search";
                arrParamsValue[1] = lang;
                arrParamsValue[2] = MaterialIssueNo;
                arrParamsValue[3] = StartDate;
                arrParamsValue[4] = EndDate;
                arrParamsValue[5] = UseTeam;
                arrParamsValue[6] = MaterialIssueStatus;
                arrParamsValue[7] = ProductionProjectCode;
                var data = conn.ExecuteQuery<MESMaterialIssue>(
                    SP_MES_MATERIALISSUE, arrParams, arrParamsValue);
                result = data.ToList();
            }
            return result;
        }

        public object saveMaterialIssue(string MaterialIssueNo, string MaterialIssueDate, string UseTeam, string MaterialIssueStatus, 
            string BOMKey, string ItemCode, string ProductionProjectCode, string MaterialIssueCreator, string MaterialIssueComment,string CreateDate)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[11];
                arrParams[0] = "@Method";
                arrParams[1] = "@MaterialIssueNo";
                arrParams[2] = "@MaterialIssueDate";
                arrParams[3] = "@UseTeam";
                arrParams[4] = "@MaterialIssueStatus";
                arrParams[5] = "@BOMKey";
                arrParams[6] = "@ItemCode";
                arrParams[7] = "@ProductionProjectCode";
                arrParams[8] = "@MaterialIssueCreator";
                arrParams[9] = "@MaterialIssueComment";
                arrParams[10] = "@CreateDate";
                object[] arrParamsValue = new string[11];
                arrParamsValue[0] = "saveData";
                arrParamsValue[1] = MaterialIssueNo;
                arrParamsValue[2] = MaterialIssueDate;
                arrParamsValue[3] = UseTeam;
                arrParamsValue[4] = MaterialIssueStatus;
                arrParamsValue[5] = BOMKey;
                arrParamsValue[6] = ItemCode;
                arrParamsValue[7] = ProductionProjectCode;
                arrParamsValue[8] = MaterialIssueCreator;
                arrParamsValue[9] = MaterialIssueComment;
                arrParamsValue[10] = CreateDate;
                var result = conn.ExecuteScalar<object>(SP_MES_MATERIALISSUE, arrParams, arrParamsValue);
                return result;
            }
        }

        public string updateStatus(string MaterialIssueNo, string MaterialIssueStatus)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@MaterialIssueNo";
                arrParams[2] = "@MaterialIssueStatus";
                object[] arrParamsValue = new string[3];
                arrParamsValue[0] = "updateStatus";
                arrParamsValue[1] = MaterialIssueNo;
                arrParamsValue[2] = MaterialIssueStatus;
                var result = conn.ExecuteScalar<string>(SP_MES_MATERIALISSUE, arrParams, arrParamsValue);
                return result;
            }
        }

        public List<DynamicCombobox> getProjectCodeCombobox()
        {
            var result = new List<DynamicCombobox>();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var data = conn.ExecuteQuery<DynamicCombobox>(SP_MES_MATERIALISSUE_PROJECT_CODE_COMBOBOX, null, null);
                result = data.ToList();
            }
            return result;
        }

        public List<MESIssueItemPart> GetBOMItem(string ItemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemCode";
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "getBOMItem";
                arrParamsValue[1] = ItemCode;
                var result = conn.ExecuteQuery<MESIssueItemPart>(SP_MES_MATERIALISSUE, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        public string savePartList(string MaterialIssueNo, string Data, string ItemToDelete, string SlipNumber, string UserId)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[5];
                arrParams[0] = "@MaterialIssueNo";
                arrParams[1] = "@ListItem";
                arrParams[2] = "@ItemToDelete";
                arrParams[3] = "@SlipNumber";
                arrParams[4] = "@UserId";
                object[] arrParamsValue = new string[5];
                arrParamsValue[0] = MaterialIssueNo;
                arrParamsValue[1] = Data;
                arrParamsValue[2] = ItemToDelete;
                arrParamsValue[3] = SlipNumber;
                arrParamsValue[4] = UserId;
                var result = conn.ExecuteScalar<string>(SP_MES_MATERIALISSUE_IMPORT_PART_LIST, arrParams, arrParamsValue);
                return result;
            }
        }

        // Thien ADD 2022-01-13
        public string UpdateItemCodeInWH(string Data, string id)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ListItem";
                arrParams[2] = "@WHCode";

                object[] arrParamsValue = new string[3];
                arrParamsValue[0] = "UpdateItemCodeInWH";
                arrParamsValue[1] = Data;
                arrParamsValue[2] = id;

                var result = conn.ExecuteScalar<string>(SP_MES_MATERIALISSUE_IMPORT_PART_LIST_UPDATEWH, arrParams, arrParamsValue);
                return result;
            }
        }
        public List<MESIssueItemPart> getMaterialIssuePartList(string MaterialIssueNo)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@MaterialIssueNo";
                object[] arrParamsValue = new string[2];
                arrParamsValue[0] = "getPartList";
                arrParamsValue[1] = MaterialIssueNo;
                var result = conn.ExecuteQuery<MESIssueItemPart>(SP_MES_MATERIALISSUE, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        public string partListExcelFileToJson(string filePath, string langCode)
        {
            Type template;
            if(langCode == "/ko")
            {
                template = typeof(ItemPartImportTemplateKor);
            }
            else
            {
                template = typeof(ItemPartImportTemplateEng);
            }

            ExcelHelperTest excelHelperTest = new ExcelHelperTest();
            DataTable dt = excelHelperTest.ReadFromExcelfile(filePath, null, template);

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
             
            return JsonConvert.SerializeObject(rows);
        }

        // Thien ADD 2022-01-13
        public Result UpdateItemToWH(string filePath, string id, string lang, string materialIssueNo)
        {
            try
            {
                Result result = new Result();
                ExcelHelperTest excelHelperTest = new ExcelHelperTest();
                // Get data from excecl to DataTable
                DataTable dt = new DataTable();
                if (lang == "/en")
                {
                    dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_MeterialToUpdateWH_EN));
                }
                else
                {
                    dt = excelHelperTest.ReadFromExcelfile(filePath, null, typeof(MES_MeterialToUpdateWH_KO));
                }
                // Add DataTable to Dataset

                dt.Rows.Remove(dt.Rows[dt.Rows.Count - 1]);

                int sendingTimes = (dt.Rows.Count / 10) + 1;
                for (int i = 0; i < sendingTimes; i++)
                {
                    string jsonObjTest;
                    DataTable dtInsert;
                    if (i == 0)
                    {
                        dtInsert = dt.AsEnumerable().Skip(0).Take(10).CopyToDataTable();
                        jsonObjTest = JsonConvert.SerializeObject(dtInsert);
                    }
                    else
                    {
                        dtInsert = dt.AsEnumerable().Skip(10 * i).Take(10).CopyToDataTable();
                        jsonObjTest = JsonConvert.SerializeObject(dtInsert);
                    }

                    if (!string.IsNullOrEmpty(jsonObjTest))
                    {
                        using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                        {
                            using (var transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    var resultInsDtl = 0;
                                    resultInsDtl = conn.ExecuteNonQuery(SP_MES_MATERIALISSUE_IMPORT_PART_LIST_UPDATEWH, CommandType.StoredProcedure,
                                            new string[] { "@ListItem", "@WHCode", "@Method", "@MaterialIssueNo" },
                                            new object[] { jsonObjTest, id, "UpdateItemCodeInWH", materialIssueNo }, transaction
                                            );
                                    transaction.Commit();
                                    result.Success = true;
                                    result.Message = MessageCode.MD0004;

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    LogWriter log = new LogWriter("Update batch data failed");
                                    log.LogWrite(ex.Message);
                                    return new Result
                                    {
                                        Success = false,
                                        Message = MessageCode.MD0005 + ex.ToString(),
                                    };
                                }

                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getItemNameByItemCode(string itemCode, string lang)
        {
            string result = null;
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@Lang";
                arrParams[2] = "@ItemCode";
                object[] arrParamsValue = new string[3];
                arrParamsValue[0] = "getItemName";
                arrParamsValue[1] = lang;
                arrParamsValue[2] = itemCode;
                var data = conn.ExecuteScalar<string>(SP_MES_MATERIALISSUE, arrParams, arrParamsValue);
                result = data;
            }
            return result;
        }
    }
}

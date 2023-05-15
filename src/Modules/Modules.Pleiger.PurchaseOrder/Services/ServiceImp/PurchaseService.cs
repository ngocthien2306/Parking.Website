using InfrastructureCore;
using InfrastructureCore.DAL;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.Models;
using Modules.Pleiger.CommonModels;
using Modules.Pleiger.PurchaseOrder.Services.IService;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Modules.Pleiger.PurchaseOrder.Services.ServiceImp
{

    public class PurchaseService : IPurchaseService
    {
        private string SP_GET_LIST_AND_SEARCH_INORDER_STATUS = "SP_GET_LIST_AND_SEARCH_INORDER_STATUS";
        //private string SP_MES_CHECK_USER_ROLE = "SP_MES_CHECK_USER_ROLE";
        private string SP_EDIT_DATA_LIST_PO = "SP_EDIT_DATA_LIST_PO";
        private string SP_MES_COMMON_CODE = "SP_MES_COMMON_CODE";

        
       
        #region get data and search func
        [HttpGet]
        public List<MES_Purchase> SearchAll(string StartPurchaseDate, string EndPurchaseDate, string ItemCode, string ItemName, string ProjectCode, 
            string PONumber, string PartnerCode,string ProjectName, string UserPONumber, string POStatus,string SalesClassification)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParams = new string[12];
                arrParams[0] = "@DIV";
                arrParams[1] = "@PONumber";
                arrParams[2] = "@ItemCode";
                arrParams[3] = "@ItemName";
                arrParams[4] = "@ProjectCode";
                arrParams[5] = "@StartDate";
                arrParams[6] = "@EndDate";
                arrParams[7] = "@Status";
                arrParams[8] = "@PartnerCode";
                //arrParams[9] = "@UserProjectCode";
                arrParams[9] = "@ProjectName";
                arrParams[10] = "@UserPONumber";
                arrParams[11] = "@SalesClassification";
                

                var arrParamValues = new object[12];
                arrParamValues[0] = "search";
                arrParamValues[1] = PONumber != null ? PONumber : null;
                arrParamValues[2] = ItemCode != null ? ItemCode : null;
                arrParamValues[3] = ItemName != null ? ItemName : null;
                arrParamValues[4] = ProjectCode != null ? ProjectCode : null;
                arrParamValues[5] = StartPurchaseDate !=null ? StartPurchaseDate : null;
                arrParamValues[6] = EndPurchaseDate != null ? EndPurchaseDate : null;
                arrParamValues[7] = POStatus != null ? POStatus : null;
                arrParamValues[8] = PartnerCode != null ? PartnerCode : null; 
                arrParamValues[9] = ProjectName != null ? ProjectName : null; 
                arrParamValues[10] = UserPONumber != null ? UserPONumber : null; 
                arrParamValues[11] = SalesClassification != null ? SalesClassification : null;


                var result = conn.ExecuteQuery<MES_Purchase>(SP_GET_LIST_AND_SEARCH_INORDER_STATUS, arrParams, arrParamValues).ToList();
                
                return result.ToList();
            }
        }
        [HttpGet]
        public List<MES_Purchase> GetDataPurchaseOrderList
        (string startDate,string endDate,string userPONumber, string projectName,string poStatus,string itemCode,string itemName,string Remark1,string partnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParams = new string[10];
                arrParams[0] = "@DIV";
                arrParams[1] = "@StartDate";
                arrParams[2] = "@EndDate";
                arrParams[3] = "@UserPONumber";
                arrParams[4] = "@ProjectName";
                arrParams[5] = "@Status";
                arrParams[6] = "@ItemCode";
                arrParams[7] = "@ItemName";
                arrParams[8] = "@Remark1";
                arrParams[9] = "@PartnerCode";             
                var arrParamValues = new object[10];
                arrParamValues[0] = "search";
                arrParamValues[1] = startDate != null ? startDate : null;
                arrParamValues[2] = endDate != null ? endDate : null;
                arrParamValues[3] = userPONumber != null ? userPONumber : null;
                arrParamValues[4] = projectName != null ? projectName : null;
                arrParamValues[5] = poStatus != null ? poStatus : null;
                arrParamValues[6] = itemCode != null ? itemCode : null;
                arrParamValues[7] = itemName != null ? itemName : null;
                arrParamValues[8] = Remark1 != null ? Remark1 : null;
                arrParamValues[9] = partnerCode != null ? partnerCode : null;

                var result = conn.ExecuteQuery<MES_Purchase>(SP_GET_LIST_AND_SEARCH_INORDER_STATUS, arrParams, arrParamValues).ToList();

                return result.ToList();
            }
        }
        #endregion


        [HttpPut]
        public Result Update_Data_MES_PurchaseOrderList(string flag, List<MES_Purchase> listPurchaseOrder)
        {
            var result = new Result();
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                using (var transaction = conn.BeginTransaction())
                {
                   try
                    {
                        if (!string.IsNullOrEmpty(flag) && flag.Equals("UPDATED")) {
                            foreach (var item in listPurchaseOrder)
                            {
                                if (!string.IsNullOrEmpty(item.State) && item.State.Equals("UPDATED"))
                                {
                                    var updateResult = conn.ExecuteNonQuery(SP_EDIT_DATA_LIST_PO,CommandType.StoredProcedure,
                                       new string[] { "@DIV", "@PONumber", "@ItemCode", "@LeadTime", "@LeadTimeType", "@PlanCompleteDate" },
                                       new object[] { "UPDATE", item.PONumber, item.ItemCode, item.LeadTime, item.LeadTimeType, item.PlanCompleteDate },
                                       transaction);
                                }
                            }
                            transaction.Commit();
                            result.Success = true;
                            result.Message = MessageCode.MD0004;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new Result
                        {
                            Success = false,
                            Message = "Save data not success! + Exception: " + ex.ToString(),
                        };
                    }

                }
            }
            return result;
        }        
    }   
}

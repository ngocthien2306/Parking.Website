using InfrastructureCore;
using InfrastructureCore.DAL;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.Models;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class PurchaseService : IPurchaseService
    {
        private string SP_GET_LIST_AND_SEARCH_INORDER_STATUS = "SP_GET_LIST_AND_SEARCH_INORDER_STATUS";
        private string SP_MES_CHECK_USER_ROLE = "SP_MES_CHECK_USER_ROLE";
        private string SP_EDIT_DATA_LIST_PO = "SP_EDIT_DATA_LIST_PO";
        private string SP_MES_COMMON_CODE = "SP_MES_COMMON_CODE";

        public CHECKRESULT CheckUserEmployee(string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParams = new string[2];
                arrParams[0] = "@DIV";
                arrParams[1] = "@UserCode";
                var arrParamValues = new object[2];
                arrParamValues[0] = "CheckUserEmployee";
                arrParamValues[1] = UserCode;
                var result = conn.ExecuteQuery<CHECKRESULT>(SP_MES_CHECK_USER_ROLE, arrParams, arrParamValues).ToList();
                if (result != null || result.Count != 0)
                {
                    return result.FirstOrDefault();
                }
                return new CHECKRESULT();
            }
        }

        public List<CHECKRESULT> CheckUserRole_Partner(string UserCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var ArrParams= new string[1];
                ArrParams[0] = "@UserCode";
                var ArrValues= new string[1] ;
                ArrValues[0] = UserCode;
                var result = conn.ExecuteQuery<CHECKRESULT>(SP_MES_CHECK_USER_ROLE, ArrParams, ArrValues).ToList();
                if(result ==null || result.Count == 0)
                {
                    return result;
                }
                if(result!=null && result.First().CheckResult > 0)
                {

                    return result;
                }
                return result;
            }
        }

        public CHECKRESULT CheckUserType(string UserId)
        {
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
                {
                    var arrParams = new string[2];
                    arrParams[0] = "@DIV";
                    arrParams[1] = "@UserId";  
                    var arrParamValues = new object[2];
                    arrParamValues[0] = "CheckUserType";
                    arrParamValues[1] = UserId;
                    var result = conn.ExecuteQuery<CHECKRESULT>(SP_MES_CHECK_USER_ROLE, arrParams, arrParamValues).ToList();
                    if (result != null || result.Count != 0)
                    {
                          return result.FirstOrDefault();
                    }          
                    return new CHECKRESULT();
                }
        }
       
        #region get data and search func
        [HttpGet]
        public List<MES_Purchase> SearchAll(string StartPurchaseDate, string EndPurchaseDate, string ItemCode, string ItemName, string ProjectCode, 
            string PONumber, string PartnerCode,string UserProjectCode ,string UserPONumber)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var arrParams = new string[10];
                arrParams[0] = "@DIV";
                arrParams[1] = "@PONumber";
                arrParams[2] = "@ItemCode";
                arrParams[3] = "@ItemName";
                arrParams[4] = "@ProjectCode";
                arrParams[5] = "@StartDate";
                arrParams[6] = "@EndDate";
               // arrParams[7] = "@Status";
                arrParams[7] = "@PartnerCode";
                arrParams[8] = "@UserProjectCode";
                arrParams[9] = "@UserPONumber";
                

                var arrParamValues = new object[10];
                arrParamValues[0] = "search";
                arrParamValues[1] = PONumber != null ? PONumber : null;
                arrParamValues[2] = ItemCode != null ? ItemCode : null;
                arrParamValues[3] = ItemName != null ? ItemName : null;
                arrParamValues[4] = ProjectCode != null ? ProjectCode : null;
                arrParamValues[5] = StartPurchaseDate !=null ? StartPurchaseDate : null;
                arrParamValues[6] = EndPurchaseDate != null ? EndPurchaseDate : null;
                //  arrParamValues[7] = Status != null ? model.Status : null;
                arrParamValues[7] = PartnerCode != null ? PartnerCode : null; 
                arrParamValues[8] = UserProjectCode != null ? UserProjectCode : null; 
                arrParamValues[9] = UserPONumber != null ? UserPONumber : null; 


                var result = conn.ExecuteQuery<MES_Purchase>(SP_GET_LIST_AND_SEARCH_INORDER_STATUS, arrParams, arrParamValues).ToList();
                if (result.Count > 0)
                {
                    int i = 1;
                    result.ForEach(x => x.No = i++);
                }
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

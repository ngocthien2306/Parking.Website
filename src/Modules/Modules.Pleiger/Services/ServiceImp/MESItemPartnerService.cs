using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESItemPartnerService : IMESItemPartnerService
    {
        private string SP_Name = "SP_MES_ITEMPARTNER";

        #region "Get Data"

        // Get list Partner by ItemCode
        public List<MES_ItemPartner> GetListPartnerByItem(string itemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListPartnerByItem";
                arrParamsValue[1] = itemCode;
                var data = conn.ExecuteQuery<MES_ItemPartner>(SP_Name, arrParams, arrParamsValue);

                return data.ToList();
            }
        }

        // Get Item by itemCode and partnerCode
        public MES_ItemPartner GetItemDetail(string itemCode, string partnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {

                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemCode";
                arrParams[2] = "@PartnerCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetItemDetail";
                arrParamsValue[1] = itemCode;
                arrParamsValue[2] = partnerCode;
                var result = conn.ExecuteQuery<MES_ItemPartner>(SP_Name, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }

        // Get list Item supplied by Partner
        public List<MES_ItemPartner> GetListItemSupply(string partnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@PartnerCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListItemByPartner";
                arrParamsValue[1] = partnerCode;
                var data =  conn.ExecuteQuery<MES_ItemPartner>(SP_Name, arrParams, arrParamsValue);
                return data.ToList();
            }
        }
         public  MES_ItemPartner GetItemPartnerByParams(string partnerCode, string itemCode)
         {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@PartnerCode";
                arrParams[2] = "@ItemCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetItemPartnerByParam";
                arrParamsValue[1] = partnerCode;
                arrParamsValue[2] = itemCode;
                var data = conn.ExecuteQuery<MES_ItemPartner>(SP_Name, arrParams, arrParamsValue);
                if(data.ToList().Count > 0)
                {
                    return data.ToList()[0] ?? null;
                }
                else
                {
                    return new MES_ItemPartner();
                }
                
            }
        }
        public object getListMonetaryUnit()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
               
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListMonetaryUnit";
               
                var data = conn.ExecuteQuery<MES_ItemMonetaryUnit>(SP_Name, arrParams, arrParamsValue);
                if (data.ToList().Count > 0)
                {
                    return data;
                }
                else
                {
                    return null;
                }

            }
        }

        public List<MES_ItemPartner> SearchItemByPartner(string partnerCode, string itemName,string itemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@PartnerCode";
                arrParams[2] = "@ItemName";
                arrParams[3] = "@ItemCode";
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "SearchListItemByPartner";
                arrParamsValue[1] = partnerCode;
                arrParamsValue[2] = itemName;
                arrParamsValue[3] = itemCode;
                var data = conn.ExecuteQuery<MES_ItemPartner>(SP_Name, arrParams, arrParamsValue);
                return data.ToList();
            }
        }

        #endregion
    }
}

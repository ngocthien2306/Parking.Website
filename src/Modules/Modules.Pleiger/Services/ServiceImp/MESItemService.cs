using InfrastructureCore;
using InfrastructureCore.DAL;
using Modules.Pleiger.Models;
using Modules.Pleiger.Services.IService;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESItemService : IMESItemService
    {
        private const string SP_MES_ITEM = "SP_MES_ITEM";

        #region "Get Data"

        // Get Item detail
        public MES_Item GetDetail(string itemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetDetailMaterial";
                arrParamsValue[1] = itemCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);

                return result.FirstOrDefault();
            }
        }

        // Get list Material
        public List<MES_Item> GetListData()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListMaterial";
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
        // Get list Material By ItemClassCode
        public List<MES_Item> GetListMaterialByItemClassCode(string ItemClassCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemClassCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetListMaterialByItemClassCode";
                arrParamsValue[1] = ItemClassCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);
                return result.ToList();
            }
        }
     

        public List<MES_Item> GetListData(int pageSize, int itemSkip)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@PageNumber";
                arrParams[2] = "@PageSize";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetListMaterialNew";
                arrParamsValue[1] = itemSkip;
                arrParamsValue[2] = pageSize;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);
                return result.ToList();
            }
        }

        // Get list Product, half product
        public List<MES_Item> GetListFinishItem()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetListFinishItem";
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        // Quan add 2020/09/14
        // Get list Item Material in Grid
        public List<MES_Item> GetListItemMaterialNotexist(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ProjectCode";
                object[] arrParamsValue = new object[2];             
                arrParamsValue[0] = "GetListItemMaterialNotexist";
                arrParamsValue[1] = projectCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        
        // Get list ItemCode By Category
        public List<MES_Item> GetItemCodeClassByCategory()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[1];
                arrParams[0] = "@Method";
                object[] arrParamsValue = new object[1];
                arrParamsValue[0] = "GetItemCodeClassByCategory";
                var result = conn.ExecuteQuery<MES_Item>("SP_MES_ITEMCLASS", arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        // Get list Material In wareHouse
        public List<MES_Item> GetItemInWareHouse(string WareHouseCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@WareHouseCode";
                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetItemInWareHouse";
                arrParamsValue[1] = WareHouseCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        public List<MES_Item> SearchItemInWareHouse(string WareHouseCode,string ItemCode, string ItemName)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[4];
                arrParams[0] = "@Method";
                arrParams[1] = "@WareHouseCode";
                arrParams[2] = "@ItemCode";
                arrParams[3] = "@ItemName";
                object[] arrParamsValue = new object[4];
                arrParamsValue[0] = "GetItemInWareHouse";
                arrParamsValue[1] = WareHouseCode;
                arrParamsValue[2] = ItemCode;
                arrParamsValue[3] = ItemName;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        
        public List<MES_Item> GetItemInWareHousebyItemCode(string WareHouseCode,string ItemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[3];
                arrParams[0] = "@Method";
                arrParams[1] = "@WareHouseCode";
                arrParams[2] = "@ItemCode";
                object[] arrParamsValue = new object[3];
                arrParamsValue[0] = "GetItemInWareHousebyItemCode";
                arrParamsValue[1] = WareHouseCode;
                arrParamsValue[2] = ItemCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue);

                return result.ToList();
            }
        }
        #endregion

        #region "Validate"
        public bool IsItemCodeMstExisted(string itemCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM,
                    new string[] { "@Method", "@ItemCode" },
                    new object[] { "IsItemCodeMstExisted", itemCode }).ToList().FirstOrDefault();
                return result != null ? false : true;
            }
        }

        public List<MES_Item> getItemsByItemClassCode(string itemClassCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@ItemClassCode";

                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetItemByItemClassCode";
                arrParamsValue[1] = itemClassCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue).ToList();
                return result;

            }
        }

        public List<MES_Item> GetItemsByCategoryCode(string categoryCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                string[] arrParams = new string[2];
                arrParams[0] = "@Method";
                arrParams[1] = "@Category";

                object[] arrParamsValue = new object[2];
                arrParamsValue[0] = "GetItemsByCategoryCode";
                arrParamsValue[1] = categoryCode;
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_ITEM, arrParams, arrParamsValue).ToList();
                return result;

            }
        }

        #endregion
    }
}

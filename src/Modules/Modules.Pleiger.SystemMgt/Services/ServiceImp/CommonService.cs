using InfrastructureCore;
using InfrastructureCore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modules.Pleiger.SystemMgt.Services.IService;
using Modules.Pleiger.CommonModels;

namespace Modules.Pleiger.SystemMgt.Services.ServiceImp
{
    public class CommonService : ICommonService
    {
        #region STORE PROCEDURE
        private const string SP_MES_COMMON_ITEM = "SP_MES_COMMON_ITEM";
        #endregion


        #region MES_ComCode Detail

        public List<MES_ComCodeDtls> GetAllComCodeDTL(string lang)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ComCodeDtls>(SP_MES_COMMON_ITEM,
                    new string[] { "@METHOD", "@Lang" },
                    new object[] { "GetAllComCodeDTL", lang }).ToList();

                return result;
            }
        }

        #endregion

        public List<MES_Item> GetAllItem(string lang)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_COMMON_ITEM,
                    new string[] { "@METHOD", "@Lang" },
                    new object[] { "GetAllItem", lang }).ToList();

                return result;
            }
        }

        public List<MES_Item> GetItemRaw(string lang)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Item>(SP_MES_COMMON_ITEM,
                    new string[] { "@METHOD", "@Lang" },
                    new object[] { "GetItemRaw", lang }).ToList();

                return result;
            }
        }
    }
}

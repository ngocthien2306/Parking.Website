using Modules.Pleiger.Services.IService;
using InfrastructureCore;
using Modules.Pleiger.Models;
using InfrastructureCore.DAL;
using System.Linq;
using System.Collections.Generic;

namespace Modules.Pleiger.Services.ServiceImp
{
    public class MESPartnerService : IMESPartnerService
    {
        private const string SP_MES_PARTNER = "SP_MES_PARTNER";

        #region "MES_ComCode Master"

        public MES_Partner GetPartnerDetailByPartnerCode(string PartnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_PARTNER,
                    new string[] { "@DIV", "@PartnerCode" },
                    new object[] { "SelectPartnerDetail", PartnerCode }).ToList().FirstOrDefault();
                return result;
            }
        }
        public List<MES_Partner> GetPartnerByPartnerCode(string PartnerCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_PARTNER,
                    new string[] { "@DIV", "@PartnerCode" },
                    new object[] { "SelectPartnerDetail", PartnerCode }).ToList();
                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }
        #endregion
        public List<MES_Partner> GetPartnerDetailByPartnerType(string PartnerType)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_PARTNER,
                    new string[] { "@DIV", "@PartnerType" },
                    new object[] { "SelectPartnerType", PartnerType }).ToList();
                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
                
            }
        }
        public List<MES_Partner> GetPartnerDetailByTwoPartnerType(string PartnerType1, string PartnerType2)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_PARTNER,
                    new string[] { "@DIV", "@PartnerType1", "@PartnerType2" },
                    new object[] { "SelectPartnerType2", PartnerType1, PartnerType2 }).ToList();
                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;

            }
        }
        // ACCT02/ACCT03
        public List<MES_Partner> GetAllPartner()
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_Partner>(SP_MES_PARTNER,
                    new string[] { "@DIV" },
                    new object[] { "GetAllPartnerRegister" }).ToList();
                int no = 1;
                result.ForEach(x =>
                {
                    x.NO = no++;
                });

                return result;
            }
        }

        
        public List<MES_ItemPartner> getListPartner_ByProjectCode(string projectCode)
        {
            using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1))
            {
                var result = conn.ExecuteQuery<MES_ItemPartner>("SP_MES_PARTNER_GET_ALL",
                    new string[] { "@Method", "@ProjectCode" },
                    new object[] { "GetListPartnerByProjectCode",projectCode }).ToList();
             
                return result;
            }
        }
       
        #region "Insert - Update - Delete"
        #endregion
    }
}

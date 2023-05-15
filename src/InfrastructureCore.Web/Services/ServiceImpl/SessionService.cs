using InfrastructureCore.DAL;
using InfrastructureCore.Models.Site;
using InfrastructureCore.Web.Services.IService;
using System.Linq;

namespace InfrastructureCore.Web.Services.ServiceImpl
{
    public class SessionService : ISessionService
    {
        private readonly string _sitecode ;
        public SessionService()
        {
        }
        public SessionService(string sitecode)
        {
            _sitecode = sitecode;
        }
        public int GetSessionTimeoutFromSite()
        {
            try
            {
                int sessionTimeOut = 0;
                using (var conn = DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection))
                {
                    var result = conn.ExecuteQuery<SYSite>("SP_Web_SY_Site",
                        new string[] { "@Method", "@SiteCode" },
                        new object[] { "GetSessionTimeout", string.IsNullOrEmpty(_sitecode) ? "S0001" : _sitecode }).ToList();
                    sessionTimeOut = result != null ? (int)result[0].SessionTimeOut : 120; // default 120 minutes
                }
                return sessionTimeOut;
            }
            catch (System.Exception e)
            {
                throw e;
            }
         
        }
    }
}

using InfrastructureCore;
using InfrastructureCore.Configuration;

namespace Modules.Common.Utils
{
    public class ConnectionUtils
    {
        public static DbConnectionInfo GetConnectionStringByConnectionType(string connectionType)
        {
            DbConnectionInfo connection = new DbConnectionInfo();
            switch (connectionType)
            {
                case "G013C000":
                    connection = GlobalConfiguration.DbConnections.FrameworkConnection;
                    break;
                case "G013C001":
                    connection = GlobalConfiguration.DbConnections.DbConnection1;
                    break;
                case "G013C002":
                    connection = GlobalConfiguration.DbConnections.DbConnection2;
                    break;
                case "G013C003":
                    connection = GlobalConfiguration.DbConnections.DbConnection3;
                    break;
                case "G013C004":
                    connection = GlobalConfiguration.DbConnections.DbConnection4;
                    break;
                case "G013C005":
                    connection = GlobalConfiguration.DbConnections.DbConnection5;
                    break;
                default: break;
            }
            return connection;
        }
    }
}

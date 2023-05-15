using InfrastructureCore.Configuration;
using InfrastructureCore.DAL.Engines;
using InfrastructureCore.DataAccess.Engines;

namespace InfrastructureCore.DAL
{
    public class DataConnectionFactory
    {
        public static IDataConnection GetConnection(DbConnectionInfo connectionInfo)
        {
            if (connectionInfo.DbType == DbmsTypes.Mssql)
            {
                return new MsDbConnection(connectionInfo.ConnectionString);
            }
            else if (connectionInfo.DbType == DbmsTypes.MySql)
            {
                return new MySqlDbConnection(connectionInfo.ConnectionString);
            }

            return null;
        }
    }
}

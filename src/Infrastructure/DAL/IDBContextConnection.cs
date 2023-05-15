using InfrastructureCore.Configuration;
using InfrastructureCore.DAL.Engines;
using InfrastructureCore.DataAccess;
using System.Data;

namespace InfrastructureCore.DAL
{
    public interface IDBContextConnection
    {
        /// <summary>
        /// DBContextConnection implement
        /// </summary>
        /// <returns>IDbConnection</returns>
        IDataConnection GetConnection(DbConnectionEnum connection);

        /// <summary>
        /// Create proceduce parameter and passing data to it
        /// </summary>
        /// <param name="parameterInfo">parameter of proceduce's information</param>
        /// <param name="param">parameter value</param>
        /// <returns>IDataParameter</returns>
        IDataParameter CreateDataParameter(SPInfor info, SPParameter p, DbmsTypes connection);

        /// <summary>
        /// Create parameter
        /// </summary>
        /// <returns></returns>
        IObjectParameter CreateParameters(DbmsTypes connection);

        /// <summary>
        /// Create Data Adapter
        /// </summary>
        /// <returns>IDbDataAdapter</returns>
        IDbDataAdapter CreateDataAdapter(DbmsTypes connection);

        /// <summary>
        /// Get Type of database
        /// </summary>
        /// <returns>DbContextType</returns>
        DbmsTypes GetDbContextType(DbmsTypes connection);

        /// <summary>
        /// DBContextConnection implement
        /// </summary>
        /// <returns>IDbConnection</returns>
        IDbConnection GetDbConnection(DbmsTypes connection);
    }
}

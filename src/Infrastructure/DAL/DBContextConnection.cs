using InfrastructureCore.Configuration;
using InfrastructureCore.DAL.Engines;
using InfrastructureCore.DataAccess;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.SqlClient;

namespace InfrastructureCore.DAL
{
    public class DBContextConnection : IDBContextConnection
    {
        private AppConfig _appConfig;
        //private DbmsTypes _dbContextType = DbmsTypes.Oracle;

        /// <summary>
        /// Constructor
        /// </summary>
        public DBContextConnection()
        {
            _appConfig = new AppConfig();
        }

        public IDbDataAdapter CreateDataAdapter(DbmsTypes connection)
        {
            switch (connection)
            {
                //Return initializes a new instance of MSSQ's parameter when type of database is MsSql
                case DbmsTypes.Mssql: return new SqlDataAdapter();

                //Return initializes a new instance of Oracle's parameter when type of database is Oracle
                case DbmsTypes.Oracle: return new OracleDataAdapter();
                default: return new OracleDataAdapter();
            }
        }

        public IDataParameter CreateDataParameter(SPInfor parameterInfo, SPParameter param, DbmsTypes connection)
        {
            switch (connection)
            {
                // Return new SqlParameter if type of databse is MsSql
                case DbmsTypes.Mssql:
                    SqlParameter mssqlparam = null;
                    if (parameterInfo.data_type.Contains("numeric"))
                    {
                        mssqlparam = new SqlParameter(parameterInfo.argument_name, !string.IsNullOrEmpty(param.Value) ? Int32.Parse(param.Value) : 0);
                    }
                    //  else if (parameterInfo.data_type.Contains("date"))
                    //{
                    //    mssqlparam = new SqlParameter(parameterInfo.argument_name, DateTime.Now);
                    //  }
                    else
                    {
                        mssqlparam = new SqlParameter(parameterInfo.argument_name, !string.IsNullOrEmpty(param.Value) ? (param.Value) : string.Empty);
                    }
                    if (parameterInfo.in_out != "IN")
                    {
                        mssqlparam.Direction = ParameterDirection.Output;
                        mssqlparam.Size = 1000;
                    }
                    return mssqlparam;

                // Return new OracleParameter if type of databse is Oracle
                case DbmsTypes.Oracle:
                    var oraclepram = new OracleParameter();
                    oraclepram.Direction = parameterInfo.in_out.ToUpper() == "IN" ? System.Data.ParameterDirection.Input : parameterInfo.in_out.ToUpper() == "OUT" ? System.Data.ParameterDirection.Output : ParameterDirection.InputOutput;
                    oraclepram.OracleDbType = Utilities.GetOracleDbType(parameterInfo.data_type.ToUpper());
                    oraclepram.ParameterName = parameterInfo.argument_name;
                    oraclepram.Value = string.IsNullOrEmpty(param.Value) && parameterInfo.in_out.ToUpper() != "OUT" && parameterInfo.data_type.ToUpper() != "REF CURSOR" ? "" : param.Value;
                    oraclepram.Size = parameterInfo.in_out.ToUpper() != "IN" && parameterInfo.data_type.ToUpper() != "REF CURSOR" ? 1000 : 0;
                    return oraclepram;

                // default return new OracleParameter
                default:
                    {
                        var pramdefault = new OracleParameter();
                        return pramdefault;
                    }
            }
        }

        public IObjectParameter CreateParameters(DbmsTypes connection)
        {
            //  switch (_dbContextType)
            //   {
            //Return initializes a new instance of MSSQ's parameter when type of database is MsSql
            //  case DbContextType.MsSql:
            return new MSSQLDParameter();

            //Return initializes a new instance of Oracle's parameter when type of database is Oracle
            //  case DbContextType.Oracle: return new OracleDParameter();
            //  default: return new OracleDParameter();
            // }
        }

        public IDataConnection GetConnection(DbConnectionEnum connection)
        {
            switch (connection)
            {
                case DbConnectionEnum.Framework:
                    return DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.FrameworkConnection);
                case DbConnectionEnum.Connection1:
                    return DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection1);
                case DbConnectionEnum.Connection2:
                    return DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection2);
                case DbConnectionEnum.Connection3:
                    return DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection3);
                case DbConnectionEnum.Connection4:
                    return DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection4);
                case DbConnectionEnum.Connection5:
                default:
                    return DataConnectionFactory.GetConnection(GlobalConfiguration.DbConnections.DbConnection5);
            }
        }

        public IDbConnection GetDbConnection(DbmsTypes connection)
        {
            switch (connection)
            {
                // initializes a new instance of the System.Data.SqlClient.SqlConnection class when
                //     setting dbtype is.MsSql
                case DbmsTypes.Mssql: return new SqlConnection(_appConfig.ConnectionString);

                // initializes a new instance of the Oracle.ManagedDataAccess.Client.OracleConnection class when
                //     setting dbtype is.MsSql
                case DbmsTypes.Oracle: return new OracleConnection(_appConfig.ConnectionString);

                // default initializes a new instance of the Oracle.ManagedDataAccess.Client.OracleConnection class when
                //     setting dbtype is.MsSql
                default: return new OracleConnection(_appConfig.ConnectionString);
            }
        }

        public DbmsTypes GetDbContextType(DbmsTypes connection)
        {
            return connection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace InfrastructureCore.Configuration
{
    public class DbConnectionsManager
    {
        public DbConnectionInfo FrameworkConnection { get; set; }
        public DbConnectionInfo DbConnection1 { get; set; }
        public DbConnectionInfo DbConnection2 { get; set; }
        public DbConnectionInfo DbConnection3 { get; set; }
        public DbConnectionInfo DbConnection4 { get; set; }
        public DbConnectionInfo DbConnection5 { get; set; }
    }

    public class DbConnectionInfo
    {
        public DbmsTypes DbType { get; set; }
        public string ConnectionString { get; set; }
    }

    public enum DbConnectionEnum
    {
        Framework,
        Connection1,
        Connection2,
        Connection3,
        Connection4,
        Connection5
    }



    public enum DbmsTypes
    {
        Mssql,
        Oracle,
        MySql,
        MariaDb,
        PostgreSql,
        Sqlite
    }
}

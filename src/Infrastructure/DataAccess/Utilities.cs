using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace InfrastructureCore.DataAccess
{
    /// <summary>
    /// Common class using in entries proj
    /// </summary>
    public class Utilities
    {

        /// <summary>
        /// Get Oracle DbType
        /// </summary>
        /// <param name="data_type"></param>
        /// <returns>OracleDbType</returns>
        public static OracleDbType GetOracleDbType(object data_type)
        {
            //if (data_type == null && !(data_type is OracleDbType)) return OracleDbType.NVarchar2;
            switch (data_type.ToString().ToUpper())
            {
                case "VARCHAR2": return OracleDbType.Varchar2;
                case "NVARCHAR2": return OracleDbType.NVarchar2;
                case "REF CURSOR": return OracleDbType.RefCursor;
                case "REFCURSOR": return OracleDbType.RefCursor;
                case "NUMBER": return OracleDbType.Int32;
                default: return OracleDbType.NVarchar2;
            }
        }

        /// <summary>
        /// Get MSSQL DbType
        /// </summary>
        /// <param name="data_type"></param>
        /// <returns>SqlDbType</returns>
        public static SqlDbType GetMSSQLDbType(object data_type)
        {
            //if (data_type == null && !(data_type is OracleDbType)) return OracleDbType.NVarchar2;
            switch (data_type.ToString().ToUpper())
            {
                case "INT": return SqlDbType.Int;
                case "NVARCHAR": return SqlDbType.NVarChar;
                case "VARCHAR": return SqlDbType.VarChar;
                default: return SqlDbType.VarChar;
            }
        }

    }
}

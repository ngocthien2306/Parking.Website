using Dapper;
using InfrastructureCore.Configuration;
using InfrastructureCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;

namespace InfrastructureCore.DAL
{
    /// <summary>
    /// DBContextConnection extension
    /// </summary>
    public static class DBContextConnectionExtension
    {

        /// <summary>
        /// Get list parameter of proceduce
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="spname">proceduce name</param>
        /// <returns></returns>
        public static IEnumerable<SPInfor> GetSPInfor(this IDBContextConnection dbConnection, string spname)
        {
            IEnumerable<SPInfor> res = null;
            var conn = dbConnection.GetDbConnection(DbmsTypes.Mssql);

            try
            {

                //dyParam.Add("EMPCURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

                var dyParam = dbConnection.CreateParameters(DbmsTypes.Mssql);

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                if (conn.State == ConnectionState.Open)
                {
                    string query = "";
                    //==========================================Oracle--------------------------------------------//
                    if (dbConnection.GetDbContextType(DbmsTypes.Mssql) == DbmsTypes.Oracle) query = $"SELECT argument_name, data_type, defaulted, default_value, position, sequence, in_out FROM SYS.ALL_ARGUMENTS where object_name  =  UPPER('{spname}') and package_name is null order by position asc";
                    //==========================================MSSQL--------------------------------------------//
                    else if (dbConnection.GetDbContextType(DbmsTypes.Mssql) == DbmsTypes.Mssql) query = $"select 'argument_name' = name, 'data_type' = type_name(user_type_id), 'defaulted' = '', 'default_value' = '', 'Length'   = max_length, 'Prec' = case when type_name(system_type_id) = 'uniqueidentifier' then precision else OdbcPrec(system_type_id, max_length, precision) end, 'position'  = parameter_id, 'sequence' = parameter_id, 'in_out' = case when is_output = 1 then 'OUT' else 'IN' end, 'Collation' = convert(sysname, case when system_type_id in (35, 99, 167, 175, 231, 239) then ServerProperty('collation') end) from sys.parameters where object_id = object_id('{spname}')";

                    res = SqlMapper.Query<SPInfor>(conn, query, param: dyParam, commandType: CommandType.Text);

                }
                conn.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }

            return res ?? new List<SPInfor>();
        }
    }
}
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace InfrastructureCore.DataAccess
{
    public class MSSQLDParameter : IObjectParameter
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly List<SqlParameter> mssqlParameters = new List<SqlParameter>();

        public void Add(string name, object dbType, ParameterDirection direction = ParameterDirection.Input, object value = null, int? size = null)
        {
            SqlParameter sqlParameter = new SqlParameter(name, value);
            sqlParameter.SqlDbType = Utilities.GetMSSQLDbType(dbType);
            if (size.HasValue) sqlParameter.Size = size.Value;
            sqlParameter.Direction = direction;
            mssqlParameters.Add(sqlParameter);

            //return sqlParameter;
        }

        public void Add(string name, object dbType, ParameterDirection direction)
        {
            var _dbType = (SqlDbType)dbType;
            var sqlParameter = new SqlParameter(name, _dbType);
            sqlParameter.Direction = direction;
            mssqlParameters.Add(sqlParameter);
            //return sqlParameter;
        }

        public void AddParam(string name, object value = null, ParameterDirection direction = ParameterDirection.Input, int? size = null, object dbType = null)
        {
            SqlParameter sqlParameter = new SqlParameter(name, value);
            if (dbType == null) sqlParameter.SqlDbType = SqlDbType.NVarChar;
            if (size.HasValue) sqlParameter.Size = size.Value;
            sqlParameter.Direction = direction;
            mssqlParameters.Add(sqlParameter);

            //return sqlParameter;
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var oracleCommand = command as SqlCommand;

            if (oracleCommand != null)
            {
                oracleCommand.Parameters.AddRange(mssqlParameters.ToArray());
            }
        }

        public IDbDataParameter GetOracleParameterByName(string name)
        {
            return this.mssqlParameters.FirstOrDefault(x => x.ParameterName == $"@{name}");
        }

        public IList<IDbDataParameter> GetParams()
        {
            return mssqlParameters as IList<IDbDataParameter>;
        }


        //public SqlParameter GetOracleParameterByName<SqlParameter>(string name)
        //{
        //    return this.mssqlParameters.FirstOrDefault(x => x.ParameterName == name);
        //}

        //public List<SqlParameter> GetParams()
        //{
        //    return this.mssqlParameters ?? new List<SqlParameter>();
        //}
    }
}

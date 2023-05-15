using Dapper;
using InfrastructureCore.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace InfrastructureCore.DataAccess
{
    public class ObjectParameters : SqlMapper.IDynamicParameters
    {
        DbmsTypes _dbContextType = DbmsTypes.Oracle;
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();
        private readonly List<SqlParameter> mssqlParameters = new List<SqlParameter>();

        public ObjectParameters()
        {
            _dbContextType = DbmsTypes.Oracle;
        }

        public ObjectParameters(DbmsTypes dbContextType)
        {
            _dbContextType = dbContextType;
        }

        public void Add(string name, OracleDbType dbType, ParameterDirection direction, object value = null, int? size = null)
        {
            OracleParameter oracleParameter;
            if (size.HasValue)
            {
                oracleParameter = new OracleParameter(name, dbType, size.Value, value, direction);
            }
            else
            {
                oracleParameter = new OracleParameter(name, dbType, value, direction);
            }

            oracleParameters.Add(oracleParameter);
        }

        public void AddParam(string name, object value = null, ParameterDirection direction = ParameterDirection.Input, int? size = null, object dbType = null)
        {
            switch (_dbContextType)
            {
                case DbmsTypes.Oracle:
                    OracleParameter oracleParameter;
                    if (dbType == null) dbType = OracleDbType.NVarchar2;
                    OracleDbType oracleDbTypeTemp = (OracleDbType)dbType;
                    if (size.HasValue)
                    {

                        oracleParameter = new OracleParameter(name, oracleDbTypeTemp, size.Value, value, direction);
                    }
                    else
                    {
                        oracleParameter = new OracleParameter(name, oracleDbTypeTemp, value, direction);
                    }

                    oracleParameters.Add(oracleParameter);
                    break;
                case DbmsTypes.Mssql:
                    SqlParameter sqlParameter = new SqlParameter(name, value);
                    if (dbType == null) sqlParameter.SqlDbType = SqlDbType.NVarChar;
                    if (size.HasValue) sqlParameter.Size = size.Value;
                    mssqlParameters.Add(sqlParameter);
                    break;
            }
        }

        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction)
        {
            var oracleParameter = new OracleParameter(name, oracleDbType, direction);
            oracleParameters.Add(oracleParameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);
            if (_dbContextType == DbmsTypes.Oracle)
            {
                var oracleCommand = command as OracleCommand;

                if (oracleCommand != null)
                {
                    oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
                }
            }
            else if (_dbContextType == DbmsTypes.Mssql)
            {
                var mssqlCommand = command as SqlCommand;

                if (mssqlCommand != null)
                {
                    mssqlCommand.Parameters.AddRange(mssqlParameters.ToArray());
                }
            }
        }

        public List<OracleParameter> GetOracleParameters()
        {
            return this.oracleParameters;
        }

        public List<SqlParameter> GetMSSQLParameters()
        {
            return this.mssqlParameters;
        }

        public OracleParameter GetOracleParameterByName(string name)
        {
            return this.oracleParameters.FirstOrDefault(x => x.ParameterName == name);
        }

        public SqlParameter GetMSSQLParameterByName(string name)
        {
            return this.mssqlParameters.FirstOrDefault(x => x.ParameterName == name);
        }
    }
}

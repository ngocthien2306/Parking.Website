using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace InfrastructureCore.DataAccess
{
    public class OracleDParameter : IObjectParameter
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

        public void Add(string name, object dbType = null, ParameterDirection direction = ParameterDirection.Input, object value = null, int? size = null)
        {
            OracleParameter oracleParameter = new OracleParameter();
            oracleParameter.ParameterName = name;
            oracleParameter.OracleDbType = Utilities.GetOracleDbType(dbType);
            oracleParameter.Direction = direction;
            if (value != null)
            {
                oracleParameter.Value = value;
            }

            if (size.HasValue)
            {
                oracleParameter.Size = size.Value; ;
            }

            oracleParameters.Add(oracleParameter);
        }

        //public void Add(string name, object oracleDbType, ParameterDirection direction)
        //{
        //    var oracleParameter = new OracleParameter();
        //    oracleParameter.ParameterName = name;
        //    oracleParameter.Direction = direction;
        //    if(oracleDbType != null && oracleDbType is OracleDbType)
        //    {
        //        var dbType = (OracleDbType)oracleDbType;
        //        oracleParameter.OracleDbType = dbType;
        //    }
        //    oracleParameters.Add(oracleParameter);
        //    //return oracleParameter;
        //}

        public void AddParam(string name, object value = null, ParameterDirection direction = ParameterDirection.Input, int? size = null, object dbType = null)
        {
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
            oracleParameter.Direction = direction;

            oracleParameters.Add(oracleParameter);
            //return oracleParameter;
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var oracleCommand = command as OracleCommand;

            if (oracleCommand != null)
            {
                oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
            }
        }

        public IDbDataParameter GetOracleParameterByName(string name)
        {
            return this.oracleParameters.FirstOrDefault(x => x.ParameterName == name);
        }

        public IList<IDbDataParameter> GetParams()
        {
            return oracleParameters as IList<IDbDataParameter>;
        }


    }
}

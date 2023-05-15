using Dapper;
using System.Collections.Generic;
using System.Data;

namespace InfrastructureCore.DataAccess
{
    public interface IObjectParameter : SqlMapper.IDynamicParameters
    {
        void AddParam(string name, object value = null, ParameterDirection direction = ParameterDirection.Input, int? size = null, object dbType = null);

        /// <summary>
        /// Add new parameter to the end of the IList parameters
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="dbType">DbType</param>
        /// <param name="direction">Parameter Direction</param>
        /// <param name="value">value</param>
        /// <param name="size">size</param>
        void Add(string name, object dbType, ParameterDirection direction, object value = null, int? size = null);
        //void Add(string name, object oracleDbType, ParameterDirection direction);

        /// <summary>
        /// Get parameter by name
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <returns>IDbDataParameter</returns>
        IDbDataParameter GetOracleParameterByName(string name);

        /// <summary>
        /// Get the list parameter 
        /// </summary>
        /// <returns></returns>
        IList<IDbDataParameter> GetParams();
    }
}

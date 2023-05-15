using InfrastructureCore.Extensions;
using InfrastructureCore.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InfrastructureCore.DAL.Engines
{

    public class MySqlDbConnection : MySqlDacBase, IDataConnection
    {
        #region Variable
        public static string DefaultConnectionString = string.Empty;
        private const int INTERVAL_SCHEDULE = 3000;
        private const int CONN_TIMES = 3;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor, provide SubSystemType to indicate which ConnectionString in machine.config 
        /// to use
        /// </summary>
        /// <param name="susSystemType"></param>
        public MySqlDbConnection()
            : base(DefaultConnectionString)
        {
        }

        public MySqlDbConnection(string connectionString)
            : base(connectionString)
        {

        }
        #endregion

        #region Connection
        public MySqlTransaction BeginMySqlTransaction()
        {
            MySqlTransaction trans = null;
            int connTimes = CONN_TIMES + 2;

            while (connTimes > 0)
            {
                try
                {
                    if (_connection.State == ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    trans = _connection.BeginTransaction();
                    break;
                }
                catch// (System.Exception ex)
                {
                    if (_connection != null)
                    {
                        if (_connection.State == ConnectionState.Open)
                        {
                            _connection.Close();
                        }
                    }
                    connTimes--;
                    System.Threading.Thread.Sleep(INTERVAL_SCHEDULE);
                }
            }

            return trans;
        }
        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Return Dataset with 1 table
        /// </summary>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string storedProcName, string[] paramNames, object[] paramValues)
        {
            return ExecuteQuery(storedProcName, CommandType.StoredProcedure, paramNames, paramValues, null);
        }
        public async Task<DataSet> ExecuteQueryAsync(string storedProcName, string[] paramNames, object[] paramValues)
        {
            return await ExecuteQueryAsync(storedProcName, CommandType.StoredProcedure, paramNames, paramValues, null);
        }
        public DataSet ExecuteQuery(string storedProcName, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            return ExecuteQuery(storedProcName, CommandType.StoredProcedure, paramNames, paramValues, trans);
        }
        public async Task<DataSet> ExecuteQueryAsync(string storedProcName, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            return await ExecuteQueryAsync(storedProcName, CommandType.StoredProcedure, paramNames, paramValues, trans);
        }
        public DataSet ExecuteQueryText(string storedProcName, string[] paramNames, object[] paramValues)
        {
            return ExecuteQuery(storedProcName, CommandType.Text, paramNames, paramValues, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeOutInSeconds"></param>
        /// <param name="commandType"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string storedProcName, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            return ExecuteQuery(storedProcName, commandType, paramNames, paramValues, null);
        }
        public async Task<DataSet> ExecuteQueryAsync(string storedProcName, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            return await ExecuteQueryAsync(storedProcName, commandType, paramNames, paramValues, null);
        }
        /// <summary>
        /// Return Dataset with 1 table
        /// </summary>
        /// <param name="timeOutInSeconds">Timeout in seconds</param>
        /// <param name="storedProcName">Stored Procedure Name</param>
        /// <param name="parameters">list of parameters including name & value</param>
        /// <returns></returns>
        public DataSet ExecuteQuery(string storedProcName, CommandType commandType, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            DataSet dsReturn = null;
            DateTime startExecute = DateTime.Now;

            SetSqlCommand(SqlCommandType.SelectCommand, storedProcName, commandType);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    MySqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand, paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            if (trans != null)
            {
                loadCommand.Transaction = trans;
            }
            dsReturn = ExecuteFill();
            return dsReturn;
        }
        public async Task<DataSet> ExecuteQueryAsync(string storedProcName, CommandType commandType, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            DataSet dsReturn = null;
            DateTime startExecute = DateTime.Now;

            SetSqlCommand(SqlCommandType.SelectCommand, storedProcName, commandType);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    MySqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand, paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            if (trans != null)
            {
                loadCommand.Transaction = trans;
            }
            dsReturn = await ExecuteFillAsync().ConfigureAwait(true);
            return dsReturn;
        }
        public IEnumerable<T> ExecuteQuery<T>(string commandText, string[] paramNames, object[] paramValues)
        {
            var ds = ExecuteQuery(commandText, paramNames, paramValues);
            return ds.FromDataSet<T>();
        }
        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string commandText, string[] paramNames, object[] paramValues)
        {
            var ds = await ExecuteQueryAsync(commandText, paramNames, paramValues);
            return ds.FromDataSet<T>();
        }
        // call SP
        public IEnumerable<T> ExecuteQuery<T>(string commandText, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            //var ds = ExecuteQuery(commandText, CommandType.StoredProcedure, paramNames, paramValues);
            var ds = ExecuteQuery(commandText, commandType, paramNames, paramValues);
            return ds.FromDataSet<T>();
        }
        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string commandText, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            //var ds = ExecuteQuery(commandText, CommandType.StoredProcedure, paramNames, paramValues);
            var ds = await ExecuteQueryAsync(commandText, commandType, paramNames, paramValues);
            return ds.FromDataSet<T>();
        }
        #endregion

        #region ExecuteQuerySingle
        public T ExecuteQuerySingle<T>(string commandText, string[] paramNames, object[] paramValues)
        {
            var dr = ExecuteQueryDataRow(commandText, CommandType.Text, paramNames, paramValues);
            return dr.FromDataRow<T>();
        }

        public T ExecuteQuerySingle<T>(string commandText, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            var dr = ExecuteQueryDataRow(commandText, commandType, paramNames, paramValues);
            return dr.FromDataRow<T>();
        }
        #endregion

        #region ExecuteScalar
        public T ExecuteScalar<T>(string commandText, string[] paramNames, object[] paramValues)
        {
            return (T)ExecuteScalar(commandText, CommandType.StoredProcedure, paramNames, paramValues, null);
        }

        public T ExecuteScalar<T>(string commandText, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            return (T)ExecuteScalar(commandText, commandType, paramNames, paramValues, null);
        }

        public T ExecuteScalar<T>(string commandText, CommandType commandType, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            return (T)ExecuteScalar(commandText, commandType, paramNames, paramValues, trans);
        }
        #endregion

        #region ExecuteUpdateData

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public bool ExecuteUpdateData2(ref DataSet changedDS, string storedProcName, string[] paramNames, string tableName, bool useTransaction)
        {
            bool boolData = false;
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    string pType = GetMySqlDbTypeName(paramNames[i], out paramName);
                    paramDirection = paramName[0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramName[0] != '@' ? paramName.Substring(paramName.IndexOf("@")) : paramName;

                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        paramDirection, paramName.Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    string pType = GetMySqlDbTypeName(paramNames[i], out paramName);
                    paramDirection = paramName[0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramName[0] != '@' ? paramName.Substring(paramName.IndexOf("@")) : paramName;

                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        paramDirection, paramName.Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    string pType = GetMySqlDbTypeName(paramNames[i], out paramName);
                    paramDirection = paramName[0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramName[0] != '@' ? paramName.Substring(paramName.IndexOf("@")) : paramName;

                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        paramDirection, paramName.Substring(1), pType);
                }
            }

            boolData = UpdateData(changedDS, tableName, useTransaction);
            return boolData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="realParamName"></param>
        /// <returns></returns>
        private string GetMySqlDbTypeName(string paramName, out string realParamName)
        {
            realParamName = paramName;
            string MySqlDbTypeName = string.Empty;
            int count = Regex.Matches(Regex.Escape(paramName), "@").Count;
            if (count == 2)
            {
                realParamName = paramName.Substring(0, paramName.LastIndexOf('@'));
                MySqlDbTypeName = paramName.Substring(paramName.LastIndexOf('@') + 1);
            }

            return MySqlDbTypeName;
        }

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public bool ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames, string tableName, bool useTransaction)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");

            string paramName = string.Empty;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            return UpdateData(changedDS, tableName, useTransaction);
        }

        /// <summary>
        ///	
        /// </summary>
        public bool ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames, string[] paramTypes, int[] paramSizes, string tableName, bool useTransaction)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.InsertCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, (((string)paramNames[i])).Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.UpdateCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, ((string)paramNames[i]).Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.DeleteCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, ((string)paramNames[i]).Substring(1));
                }
            }

            return UpdateData(changedDS, tableName, useTransaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramTypeName"></param>
        /// <returns></returns>
        private MySqlDbType GetParamTypeByName(string paramTypeName)
        {
            switch (paramTypeName.ToLower())
            {
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                    return MySqlDbType.VarChar;
                case "bit":
                    return MySqlDbType.Bit;
                case "decimal":
                    return MySqlDbType.Decimal;
                case "numeric":
                    return MySqlDbType.Float;
                case "integer":
                    return MySqlDbType.Int32;
                case "float":
                    return MySqlDbType.Float;
                case "double":
                    return MySqlDbType.Double;
                case "bigint":
                    return MySqlDbType.Int64;
                case "binary":
                    return MySqlDbType.Binary;
                //case "image":
                //    return MySqlDbType.Image;
                case "int":
                    return MySqlDbType.Int16;
                //case "money":
                //    return MySqlDbType.Money;
                case "ntext":
                    return MySqlDbType.Text;
                //case "nvarchar":
                //    return MySqlDbType.NVarChar;
                //case "real":
                //    return MySqlDbType.Real;
                //case "smalldatetime":
                //    return MySqlDbType.SmallDateTime;
                case "datetime":
                case "date":
                    return MySqlDbType.DateTime;
                //case "smallint":
                //    return MySqlDbType.SmallInt;
                //case "smallmoney":
                //    return MySqlDbType.SmallMoney;
                case "text":
                    return MySqlDbType.Text;
                //case "timestamp":
                //    return MySqlDbType.Timestamp;
                //case "tinyInt":
                //    return MySqlDbType.TinyInt;
                //case "variant":
                default:
                    return MySqlDbType.String;
            }
        }

        #endregion

        #region ExecuteUpdateData

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public Result ExecuteUpdateData2(ref DataSet changedDS, string storedProcName, string[] paramNames, string tableName, MySqlTransaction trans)
        {
            Result boolData = new Result();

            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        paramDirection, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        paramDirection, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        paramDirection, paramName.Substring(1));
                }
            }

            boolData = UpdateData(changedDS, tableName, trans);

            return boolData;
        }

        /// <summary>
        /// Update multi-rows to database
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="tableName"></param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public Result ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames, string tableName, MySqlTransaction trans)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");

            string paramName = string.Empty;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.InsertCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.UpdateCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramName = paramNames[i];
                    AddSqlParameter(SqlCommandType.DeleteCommand, paramName,
                        ParameterDirection.Input, paramName.Substring(1));
                }
            }

            return UpdateData(changedDS, tableName, trans);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changedDS"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramTypes"></param>
        /// <param name="paramSizes"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames, string[] paramTypes, int[] paramSizes, string tableName)
        {

            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.InsertCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, (((string)paramNames[i])).Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.UpdateCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, ((string)paramNames[i]).Substring(1));
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    AddSqlParameter(SqlCommandType.DeleteCommand, paramNames[i], GetParamTypeByName(paramTypes[i]), paramSizes[i],
                        ParameterDirection.Input, ((string)paramNames[i]).Substring(1));
                }
            }

            return UpdateData(changedDS, tableName, false);
        }

        /// <summary>
        /// 2020-05-28 Minh Custom
        ///	Update data Execute SP
        /// </summary>
        public Result ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames, string[] paramTypes, int[] paramSizes, string tableName, MySqlTransaction trans)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");
            ParameterDirection paramDirection = ParameterDirection.Input;

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    MySqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.InsertCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    MySqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.UpdateCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    MySqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.DeleteCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            return UpdateData(changedDS, tableName, trans);
        }

        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        public int ExecuteNonQuery(string commandText, string[] paramNames, object[] paramValues)
        {
            return ExecuteNonQuery(commandText, CommandType.StoredProcedure, paramNames, paramValues, (MySqlTransaction)null);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            return ExecuteNonQuery(commandText, commandType, paramNames, paramValues, (MySqlTransaction)null);
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            SetSqlCommand(SqlCommandType.SelectCommand, commandText, commandType);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    MySqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand, paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            return ExecuteNonQuery(trans);
        }
        #endregion

        #region ExecuteScalar - Trans

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            return ExecuteScalar(commandText, CommandType.Text, paramNames, paramValues, trans);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, CommandType commandType, string[] paramNames, object[] paramValues, MySqlTransaction trans)
        {
            object objReturn = null;
            SetSqlCommand(SqlCommandType.SelectCommand, commandText, commandType);

            string paramName = string.Empty;
            ParameterDirection paramDirection = ParameterDirection.InputOutput;
            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    paramDirection = paramNames[i][0] != '@' ? ParameterDirection.InputOutput : ParameterDirection.Input;
                    paramName = paramNames[i][0] != '@' ? paramNames[i].Substring(paramNames[i].IndexOf("@")) : paramNames[i];

                    MySqlParameter param = AddSqlParameter(SqlCommandType.SelectCommand,
                        paramName, paramValues[i]);
                    param.Direction = paramDirection;
                }
            }

            objReturn = ExecuteScalar(trans);
            return objReturn;
        }

        #region ExecuteQueryDataRow

        /// <summary>
        /// Return a single Row or null
        /// </summary>
        /// <param name="subSystemType"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataRow ExecuteQueryDataRow(string storedProcName, string[] paramNames, object[] paramValues)
        {
            DataSet ds = ExecuteQuery(storedProcName, paramNames, paramValues);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        /// <summary>
        /// Return a single Row or null
        /// </summary>
        /// <param name="timeOutInSeconds">Timeout in seconds</param>
        /// <param name="subSystemType"></param>
        /// <param name="storedProcName"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public DataRow ExecuteQueryDataRow(string storedProcName, CommandType commandType, string[] paramNames, object[] paramValues)
        {
            DataSet ds = ExecuteQuery(storedProcName, commandType, paramNames, paramValues, null);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        #endregion



        public T ExecuteScalar<T>(string commandText, CommandType commandType, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            throw new NotImplementedException();
        }

        public Result ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames, string[] paramTypes, int[] paramSizes, string tableName, SqlTransaction trans)
        {
            throw new NotImplementedException();
        }

        public SqlTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public Result ExecuteUpdateData(DataSet changedDS, string storedProcName, string[] paramNames, string[] paramTypes, int[] paramSizes, string tableName, DbTransaction trans)
        {
            SetSqlCommand(SqlCommandType.InsertCommand, storedProcName);
            AddSqlParameter(SqlCommandType.InsertCommand, "@actionID", MySqlDbType.VarChar, 6, "INSERT");
            ParameterDirection paramDirection = ParameterDirection.Input;

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    MySqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.InsertCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.UpdateCommand, storedProcName);
            AddSqlParameter(SqlCommandType.UpdateCommand, "@actionID", MySqlDbType.VarChar, 6, "UPDATE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    MySqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.UpdateCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            SetSqlCommand(SqlCommandType.DeleteCommand, storedProcName);
            AddSqlParameter(SqlCommandType.DeleteCommand, "@actionID", MySqlDbType.VarChar, 6, "DELETE");

            if (paramNames != null)
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    MySqlDbType pType = GetParamTypeByName(paramTypes[i]);

                    AddSqlParameter(SqlCommandType.DeleteCommand, paramNames[i], paramDirection, (((string)paramNames[i])).Substring(1), pType);
                }
            }

            return UpdateData(changedDS, tableName, trans);
        }

        public object ExecuteQuery2(string commandText, string[] paramNames, object[] paramValues)
        {
            throw new NotImplementedException();
        }

        List<List<MapFieldDBModel>> IDataConnection.ExecuteQuery2(string commandText, string[] paramNames, object[] paramValues)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ExecuteQuery<T>(string commandText, string[] paramNames, object[] paramValues, SqlTransaction trans)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}